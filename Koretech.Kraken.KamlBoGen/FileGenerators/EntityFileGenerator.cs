using System.Text;
using Koretech.Kraken.KamlBoGen.FileGenerators.Shared;
using SharedModel = Koretech.Kraken.KamlBoModel.Model;

namespace Koretech.Kraken.KamlBoGen.FileGenerators
{
    internal class EntityFileGenerator : FileGenerator
    {
        private const string EntitiesPath = "Entities";

        private readonly PropertyTypeMapper _typeMapper;
        private readonly CodeSnippetBuilder _snippets;

        public EntityFileGenerator(DirectoryInfo outputRootDirectory) : base(outputRootDirectory)
        {
            generatePath = EntitiesPath;
            _typeMapper = new PropertyTypeMapper();
            _snippets = new CodeSnippetBuilder(_typeMapper);
        }

        /// <summary>
        /// Creates the subdirectory for storing entity files in a specific domain if it doesn't already exist.
        /// </summary>
        public void CreateDomainSubdirectory(SharedModel.KamlBoDomain domain)
        {
            ArgumentNullException.ThrowIfNull(domain);
            CreateDomainDirectory(domain.Name);
        }

        protected override void DoGenerate(SharedModel.KamlBoDomain domain)
        {
            ArgumentNullException.ThrowIfNull(domain);

            foreach (SharedModel.KamlBoEntity entity in domain.Entities)
            {
                EntityFileModel fileModel = BuildEntityFileModel(entity);
                WriteEntityFile(fileModel);
            }
        }

        private EntityFileModel BuildEntityFileModel(SharedModel.KamlBoEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            string domainName = entity.Domain.Name;
            string namespaceName = $"Koretech.Domains.{domainName}s.Entities";
            string domainDirectoryPath = CreateDomainDirectory(domainName).FullName;
            string outputFilePath = Path.Combine(domainDirectoryPath, $"{entity.Name}Entity_Gen.cs");

            return new EntityFileModel(
                entity.Name,
                namespaceName,
                outputFilePath,
                BuildUsingStatements(entity).ToList(),
                BuildPropertyModels(entity).ToList(),
                BuildRelationModels(entity).ToList());
        }

        private IEnumerable<string> BuildUsingStatements(SharedModel.KamlBoEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return entity.Relations
                .Where(r => r.IsCrossDomain && !string.IsNullOrWhiteSpace(r.TargetDomain))
                .Select(r => $"Koretech.Domains.{r.TargetDomain}s.Entities")
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(ns => ns, StringComparer.OrdinalIgnoreCase);
        }

        private IEnumerable<EntityPropertyModel> BuildPropertyModels(SharedModel.KamlBoEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            foreach (SharedModel.KamlBoEntityProperty property in entity.Properties)
            {
                string? defaultValue = property.IsPartitionProperty ? entity.PartitionValue : null;
                string? comment = property.IsPartitionProperty ? "Partition property" : null;

                yield return new EntityPropertyModel(
                    property.Name ?? string.Empty,
                    _snippets.PropertyDeclaration(property, defaultValue, comment));
            }
        }

        private IEnumerable<EntityRelationModel> BuildRelationModels(SharedModel.KamlBoEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            foreach (SharedModel.KamlBoEntityRelation relation in entity.Relations)
            {
                string targetEntityType = $"{relation.TargetEntity}Entity";
                string relationType = relation.IsToOwner ? "owner" : "child";
                string relationComment = $"Navigation property to {relationType} {targetEntityType}";
                bool isCollection = relation.IsToMany;

                string declaration = isCollection
                    ? _snippets.CollectionNavigation(targetEntityType, relation.Name, relationComment)
                    : _snippets.ReferenceNavigation(targetEntityType, relation.Name, relationComment);

                yield return new EntityRelationModel(
                    relation.Name,
                    declaration,
                    relation.IsCrossDomain,
                    isCollection,
                    relation.IsToOwner);
            }
        }

        private void WriteEntityFile(EntityFileModel fileModel)
        {
            ArgumentNullException.ThrowIfNull(fileModel);

            if (File.Exists(fileModel.OutputFilePath))
            {
                File.Delete(fileModel.OutputFilePath);
            }

            string content = BuildEntityFileContent(fileModel);
            File.WriteAllText(fileModel.OutputFilePath, content);

            Console.WriteLine($"File {fileModel.OutputFilePath} generated.");
        }

        private string BuildEntityFileContent(EntityFileModel fileModel)
        {
            ArgumentNullException.ThrowIfNull(fileModel);

            StringBuilder content = new();
            content.Append(GetFileHeader());
            content.AppendLine();

            AppendUsingStatements(content, fileModel);
            AppendNamespaceAndClassHeader(content, fileModel);
            AppendPropertyDeclarations(content, fileModel);
            AppendRelationDeclarations(content, fileModel);
            AppendClassFooter(content);

            return content.ToString();
        }

        private void AppendUsingStatements(StringBuilder content, EntityFileModel fileModel)
        {
            ArgumentNullException.ThrowIfNull(content);
            ArgumentNullException.ThrowIfNull(fileModel);

            foreach (string usingNamespace in fileModel.UsingStatements)
            {
                content.AppendLine($"using {usingNamespace};");
            }

            if (fileModel.UsingStatements.Count > 0)
            {
                content.AppendLine();
            }

            content.AppendLine();
        }

        private void AppendNamespaceAndClassHeader(StringBuilder content, EntityFileModel fileModel)
        {
            ArgumentNullException.ThrowIfNull(content);
            ArgumentNullException.ThrowIfNull(fileModel);

            content.AppendLine($"namespace {fileModel.NamespaceName}");
            content.AppendLine("{");
            content.AppendLine($"\tpublic partial class {fileModel.EntityName}Entity");
            content.AppendLine("\t{");
            content.AppendLine("\t");
        }

        private void AppendPropertyDeclarations(StringBuilder content, EntityFileModel fileModel)
        {
            ArgumentNullException.ThrowIfNull(content);
            ArgumentNullException.ThrowIfNull(fileModel);

            foreach (EntityPropertyModel property in fileModel.Properties)
            {
                content.AppendLine(property.Declaration);
                content.AppendLine();
            }
        }

        private void AppendRelationDeclarations(StringBuilder content, EntityFileModel fileModel)
        {
            ArgumentNullException.ThrowIfNull(content);
            ArgumentNullException.ThrowIfNull(fileModel);

            foreach (EntityRelationModel relation in fileModel.Relations)
            {
                if (relation.IsCrossDomain)
                {
                    content.AppendLine("\t\t// This is a cross-domain relationship.");
                }

                content.AppendLine(relation.Declaration);
                content.AppendLine();
            }
        }

        private void AppendClassFooter(StringBuilder content)
        {
            ArgumentNullException.ThrowIfNull(content);

            content.AppendLine("\t}");
            content.AppendLine("}");
        }

        private DirectoryInfo CreateDomainDirectory(string domainName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(domainName);

            string entityDirectoryPath = Path.Combine(outputRootDirectory.FullName, EntitiesPath, domainName);
            return Directory.CreateDirectory(entityDirectoryPath);
        }

        private sealed record EntityFileModel(
            string EntityName,
            string NamespaceName,
            string OutputFilePath,
            IReadOnlyList<string> UsingStatements,
            IReadOnlyList<EntityPropertyModel> Properties,
            IReadOnlyList<EntityRelationModel> Relations);

        private sealed record EntityPropertyModel(
            string PropertyName,
            string Declaration);

        private sealed record EntityRelationModel(
            string RelationName,
            string Declaration,
            bool IsCrossDomain,
            bool IsCollection,
            bool IsOwnerRelation);
    }
}
