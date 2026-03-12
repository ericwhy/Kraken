using System.Text;
using Koretech.Tools.KrakenGenerator.FileGenerators.Shared;
using SharedModel = Koretech.Tools.KamlBoModel.Model;

namespace Koretech.Tools.KrakenGenerator.FileGenerators
{
    internal class ConfigurationFileGenerator : FileGenerator
    {
        private const string ConfigurationsPath = "Configurations";

        private readonly PropertyTypeMapper _typeMapper;

        public ConfigurationFileGenerator(DirectoryInfo outputDirectory) : base(outputDirectory)
        {
            generatePath = ConfigurationsPath;
            _typeMapper = new PropertyTypeMapper();
        }

        protected override void DoGenerate(SharedModel.KamlBoDomain domain)
        {
            ArgumentNullException.ThrowIfNull(domain);

            foreach (SharedModel.KamlBoEntity entity in domain.Entities)
            {
                ConfigurationFileModel fileModel = BuildConfigurationFileModel(entity);
                WriteConfigurationFile(fileModel);
            }
        }

        private ConfigurationFileModel BuildConfigurationFileModel(SharedModel.KamlBoEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            string domainName = entity.Domain.Name;
            string namespaceName = $"Koretech.Domains.{domainName}s.EntityConfigurations";
            string outputFilePath = Path.Combine(
                outputRootDirectory.FullName,
                ConfigurationsPath,
                $"{entity.Name}EntityTypeConfiguration.cs");

            string keyLambda = entity.Key.AsLambda();
            if (string.IsNullOrWhiteSpace(keyLambda))
            {
                throw new InvalidOperationException(
                    $"Did not find a primary key for table '{entity.TableName}' while generating a configuration for '{entity.Name}'.");
            }

            return new ConfigurationFileModel(
                entity.Name,
                entity.TableName,
                namespaceName,
                outputFilePath,
                BuildUsingStatements(domainName).ToList(),
                keyLambda,
                BuildPropertyModels(entity).ToList());
        }

        private IEnumerable<string> BuildUsingStatements(string domainName)
        {
            return new[]
            {
                $"Koretech.Domains.{domainName}s.Entities",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.EntityFrameworkCore.Metadata.Builders"
            };
        }

        private IEnumerable<ConfigurationPropertyModel> BuildPropertyModels(SharedModel.KamlBoEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            foreach (SharedModel.KamlBoEntityProperty property in entity.Properties)
            {
                yield return new ConfigurationPropertyModel(
                    property.Name ?? string.Empty,
                    BuildPropertyColumnConfiguration(property));
            }
        }

        private string BuildPropertyColumnConfiguration(SharedModel.KamlBoEntityProperty property)
        {
            ArgumentNullException.ThrowIfNull(property);

            StringBuilder builder = new();
            builder.AppendLine($"\t\t\ttypeBuilder.Property(e => e.{property.Name})");
            AppendPropertySpecifications(builder, property);
            builder.AppendLine($"\t\t\t\t.HasColumnName(\"{property.Column}\");");

            return builder.ToString().TrimEnd();
        }

        private void AppendPropertySpecifications(StringBuilder builder, SharedModel.KamlBoEntityProperty property)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(property);

            string dataType = property.DataType?.ToLowerInvariant() ?? string.Empty;

            if (_typeMapper.IsStringType(property))
            {
                if (property.Length > 0)
                {
                    builder.AppendLine($"\t\t\t\t.HasMaxLength({property.Length})");
                }

                builder.AppendLine("\t\t\t\t.IsUnicode(false)");
                return;
            }

            if (dataType is "date" or "datetime")
            {
                builder.AppendLine("\t\t\t\t.HasDefaultValueSql(\"(getdate())\")");
            }

            if (dataType == "yesno")
            {
                builder.AppendLine("\t\t\t\t.HasMaxLength(1)");
                builder.AppendLine("\t\t\t\t.IsUnicode(false)");
                builder.AppendLine("\t\t\t\t.HasDefaultValueSql(\"('N')\")");
                builder.AppendLine("\t\t\t\t.IsFixedLength()");
                return;
            }

            if (dataType is "decimal" or "numeric" or "money" && property.Length > 0)
            {
                builder.AppendLine($"\t\t\t\t.HasPrecision({property.Length})");
            }

            string? columnType = _typeMapper.GetEfColumnType(property);
            if (!string.IsNullOrWhiteSpace(columnType))
            {
                builder.AppendLine($"\t\t\t\t.HasColumnType(\"{columnType}\")");
            }

            if ((dataType is "bytes" or "varbinary") && property.Length > 0)
            {
                builder.AppendLine($"\t\t\t\t.HasMaxLength({property.Length})");
            }
        }

        private void WriteConfigurationFile(ConfigurationFileModel fileModel)
        {
            ArgumentNullException.ThrowIfNull(fileModel);

            if (File.Exists(fileModel.OutputFilePath))
            {
                File.Delete(fileModel.OutputFilePath);
            }

            string content = BuildConfigurationFileContent(fileModel);
            File.WriteAllText(fileModel.OutputFilePath, content);

            Console.WriteLine($"File {fileModel.OutputFilePath} generated.");
        }

        private string BuildConfigurationFileContent(ConfigurationFileModel fileModel)
        {
            ArgumentNullException.ThrowIfNull(fileModel);

            StringBuilder content = new();
            content.Append(GetFileHeader());
            content.AppendLine();

            AppendUsingStatements(content, fileModel);
            AppendNamespaceAndClassHeader(content, fileModel);
            AppendTableAndKeyConfiguration(content, fileModel);
            AppendPropertyConfigurations(content, fileModel);
            AppendClassFooter(content);

            return content.ToString();
        }

        private void AppendUsingStatements(StringBuilder content, ConfigurationFileModel fileModel)
        {
            foreach (string usingStatement in fileModel.UsingStatements)
            {
                content.AppendLine($"using {usingStatement};");
            }

            content.AppendLine();
        }

        private void AppendNamespaceAndClassHeader(StringBuilder content, ConfigurationFileModel fileModel)
        {
            content.AppendLine($"namespace {fileModel.NamespaceName}");
            content.AppendLine("{");
            content.AppendLine($"\tpublic class {fileModel.EntityName}EntityTypeConfiguration : IEntityTypeConfiguration<{fileModel.EntityName}Entity>");
            content.AppendLine("\t{");
            content.AppendLine($"\t\tpublic void Configure(EntityTypeBuilder<{fileModel.EntityName}Entity> typeBuilder)");
            content.AppendLine("\t\t{");
        }

        private void AppendTableAndKeyConfiguration(StringBuilder content, ConfigurationFileModel fileModel)
        {
            content.AppendLine("\t\t\t// UseSqlOutputClause tells EF to use less efficient update method due to a trigger in the table.");
            content.AppendLine("\t\t\t// TODO: Make this configurable via KAMLBO once trigger handling is modeled.");
            content.AppendLine("\t\t\ttypeBuilder");
            content.AppendLine($"\t\t\t\t.ToTable<{fileModel.EntityName}Entity>(\"{fileModel.TableName}\", \"ks\", tb => tb.UseSqlOutputClause(false));");
            content.AppendLine();
            content.AppendLine($"\t\t\ttypeBuilder.HasKey(e => new {{ {fileModel.KeyLambda} }});");
            content.AppendLine();
        }

        private void AppendPropertyConfigurations(StringBuilder content, ConfigurationFileModel fileModel)
        {
            foreach (ConfigurationPropertyModel property in fileModel.Properties)
            {
                content.AppendLine(property.Configuration);
                content.AppendLine();
            }
        }

        private void AppendClassFooter(StringBuilder content)
        {
            content.AppendLine("\t\t}");
            content.AppendLine("\t}");
            content.AppendLine("}");
        }

        private sealed record ConfigurationFileModel(
            string EntityName,
            string TableName,
            string NamespaceName,
            string OutputFilePath,
            IReadOnlyList<string> UsingStatements,
            string KeyLambda,
            IReadOnlyList<ConfigurationPropertyModel> Properties);

        private sealed record ConfigurationPropertyModel(
            string PropertyName,
            string Configuration);
    }
}
