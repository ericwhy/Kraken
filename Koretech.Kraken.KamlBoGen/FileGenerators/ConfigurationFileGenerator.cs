using Koretech.Kraken.KamlBoGen.KamlBoModel;

namespace Koretech.Kraken.KamlBoGen.FileGenerators
{
    internal class ConfigurationFileGenerator : FileGenerator
    {
        private const string configurationsPath = "Configurations";

        public ConfigurationFileGenerator(DirectoryInfo outputDirectory) : base(outputDirectory) 
        { 
            generatePath = configurationsPath;
        }

        /// <summary>
        /// Generates an EF entity type configuration class from a KAML BO specification.
        /// </summary>
        /// <param name="domain">root of the model of the KAML BO specification, i.e. domain</param>
        protected override void DoGenerate(KamlBoDomain domain)
        {
            foreach (KamlBoEntity entity in domain.Entities)
            {
                CreateConfigurationFile(entity);
            }
        }

        /// <summary>
        /// Generates an EF entity type configuration class from a KAML BO specification.
        /// </summary>
        /// <param name="entity">model of the KAML BO specification for a BO</param>
        private void CreateConfigurationFile(KamlBoEntity entity) 
        { 
            string domainName = entity.Domain.Name;
            string domainPackageName = domainName + "s";
            string entityName = entity.Name;
            string tableName = entity.TableName;
            string sourceFileName = Path.Combine(
                Path.Combine(outputRootDirectory.FullName, configurationsPath), $"{entityName}EntityTypeConfiguration.cs");
            if (File.Exists(sourceFileName))
            {
                File.Delete(sourceFileName);
            }

            var writer = File.CreateText(sourceFileName);
            writer.Write(GetFileHeader());
            writer.WriteLine();
            writer.WriteLine($"using Koretech.Domains.{domainPackageName}.Entities;");
            writer.WriteLine("using Microsoft.EntityFrameworkCore;");
            writer.WriteLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
            writer.WriteLine();
            writer.WriteLine($"namespace Koretech.Domains.{domainPackageName}.EntityConfigurations");
            writer.WriteLine("{");
            writer.WriteLine($"\tpublic class {entityName}EntityTypeConfiguration : IEntityTypeConfiguration<{entityName}Entity>");
            writer.WriteLine("\t{");
            writer.WriteLine($"\t\tpublic void Configure(EntityTypeBuilder<{entityName}Entity> typeBuilder)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\ttypeBuilder");
            writer.WriteLine($"\t\t\t\t.ToTable<{entityName}Entity>(\"{tableName}\", \"ks\");");
            writer.WriteLine();
            string keyString = string.Empty;
            foreach (KamlEntityProperty property in entity.Properties.Where(p => p.IsKey))
            {
                if (keyString.Length > 0) 
                {
                    keyString += ", ";
                }
                keyString += "e." + property.Name;
            }
            writer.WriteLine($"\t\t\ttypeBuilder.HasKey(e => new {{ {keyString} }})");
            writer.WriteLine($"\t\t\t\t.HasName(\"{tableName}_PK\");");
            writer.WriteLine();

            // Properties
            foreach (KamlEntityProperty property in entity.Properties)
            {
                if (string.Equals(property.DataType, SqlType.Character.TypeName, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteStringColumn(property, writer);
                }
                else if (string.Equals(property.DataType, SqlType.DateTime.TypeName, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteDateTimeColumn(property, writer);
                }
                else if (string.Equals(property.DataType, SqlType.YesNo.TypeName, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteYesNoColumn(property, writer);
                }
                else if (string.Equals(property.DataType, SqlType.Integer.TypeName, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteIntegerColumn(property, writer);
                }
                else if (string.Equals(property.DataType, SqlType.UniqueIdentifier.TypeName, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteUuidColumn(property, writer);
                }
                else if (string.Equals(property.DataType, SqlType.Bytes.TypeName, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteBytesColumn(property, writer);
                }
                else if (string.Equals(property.DataType, SqlType.Byte.TypeName, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteByteColumn(property, writer);
                }
                else
                {
                    Console.WriteLine($"{entityName} has property {property.Name} with unknown type {property.DataType}");
                }
            }

            writer.WriteLine("\t\t}");
            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Flush();
            writer.Close();
            Console.WriteLine($"File {sourceFileName} generated.");
        }

        private void WriteStringColumn(KamlEntityProperty property, StreamWriter writer)
        {
            writer.WriteLine($"\t\t\ttypeBuilder.Property(e => e.{property.Name})");
            if (property.Length > 0)
            {
                writer.WriteLine($"\t\t\t\t.HasMaxLength({property.Length})");
            }
            writer.WriteLine("\t\t\t\t.IsUnicode(false)");
            writer.WriteLine($"\t\t\t\t.HasColumnName(\"{property.Column}\");");
            writer.WriteLine();
        }

        private void WriteDateTimeColumn(KamlEntityProperty property, StreamWriter writer)
        {
            writer.WriteLine($"\t\t\ttypeBuilder.Property(e => e.{property.Name})");
            writer.WriteLine($"\t\t\t\t.HasDefaultValueSql(\"(getdate())\")");
            writer.WriteLine($"\t\t\t\t.HasColumnType(\"datetime\")");
            writer.WriteLine($"\t\t\t\t.HasColumnName(\"{property.Column}\");");
            writer.WriteLine();
        }

        private void WriteIntegerColumn(KamlEntityProperty property, StreamWriter writer)
        {
            writer.WriteLine($"\t\t\ttypeBuilder.Property(e => e.{property.Name})");
            writer.WriteLine($"\t\t\t\t.HasColumnName(\"{property.Column}\");");
            writer.WriteLine();
        }

        private void WriteUuidColumn(KamlEntityProperty property, StreamWriter writer)
        {
            writer.WriteLine($"\t\t\ttypeBuilder.Property(e => e.{property.Name})");
            writer.WriteLine($"\t\t\t\t.HasColumnType(\"uniqueidentifier\")");
            writer.WriteLine($"\t\t\t\t.HasColumnName(\"{property.Column}\");");
            writer.WriteLine();
        }

        private void WriteBytesColumn(KamlEntityProperty property, StreamWriter writer)
        {
            writer.WriteLine($"\t\t\ttypeBuilder.Property(e => e.{property.Name})");
            writer.WriteLine($"\t\t\t\t.HasColumnType(\"varbinary\")");
            writer.WriteLine($"\t\t\t\t.HasMaxLength({property.Length})");
            writer.WriteLine($"\t\t\t\t.HasColumnName(\"{property.Column}\");");
            writer.WriteLine();
        }

        private void WriteByteColumn(KamlEntityProperty property, StreamWriter writer)
        {
            writer.WriteLine($"\t\t\ttypeBuilder.Property(e => e.{property.Name})");
            writer.WriteLine($"\t\t\t\t.HasColumnType(\"byte\")");
            writer.WriteLine($"\t\t\t\t.HasColumnName(\"{property.Column}\");");
            writer.WriteLine();
        }

        private void WriteYesNoColumn(KamlEntityProperty property, StreamWriter writer)
        {
            writer.WriteLine($"\t\t\ttypeBuilder.Property(e => e.{property.Name})");
            writer.WriteLine($"\t\t\t\t.HasMaxLength(1)");
            writer.WriteLine("\t\t\t\t.IsUnicode(false)");
            writer.WriteLine($"\t\t\t\t.HasDefaultValueSql(\"('N')\")");
            writer.WriteLine($"\t\t\t\t.IsFixedLength()");
            writer.WriteLine($"\t\t\t\t.HasColumnName(\"{property.Column}\");");
            writer.WriteLine();
        }
    }
}
