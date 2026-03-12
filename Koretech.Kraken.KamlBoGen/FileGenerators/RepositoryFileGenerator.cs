using System.Text;
using SharedModel = Koretech.Tools.KamlBoModel.Model;

namespace Koretech.Kraken.KamlBoGen.FileGenerators
{
    internal class RepositoryFileGenerator : FileGenerator
    {
        private const string RepositoriesPath = "Repositories";

        public RepositoryFileGenerator(DirectoryInfo outputRootDirectory) : base(outputRootDirectory)
        {
            generatePath = RepositoriesPath;
        }

        protected override void DoGenerate(SharedModel.KamlBoDomain domain)
        {
            ArgumentNullException.ThrowIfNull(domain);

            SharedModel.KamlBoEntity primaryEntity = domain.PrimaryEntity
                ?? throw new InvalidOperationException(
                    $"Could not find primary entity '{domain.PrimaryEntityName}' in domain '{domain.Name}'.");

            RepositoryFileModel fileModel = BuildRepositoryFileModel(domain, primaryEntity);
            WriteRepositoryFile(fileModel);
        }

        private RepositoryFileModel BuildRepositoryFileModel(SharedModel.KamlBoDomain domain, SharedModel.KamlBoEntity primaryEntity)
        {
            string domainName = domain.Name;
            string entityTypeName = $"{primaryEntity.Name}Entity";
            string domainPackageName = $"{domainName}s";
            string namespaceName = $"Koretech.Domains.{domainPackageName}.Repositories";
            string outputFilePath = Path.Combine(outputRootDirectory.FullName, RepositoriesPath, $"{domainName}Repository_Gen.cs");

            return new RepositoryFileModel(
                domainName,
                entityTypeName,
                namespaceName,
                outputFilePath,
                BuildUsingStatements(domainPackageName).ToList(),
                primaryEntity.Key.AsParameters(true, entity: primaryEntity),
                primaryEntity.Key.AsCondition(),
                BuildIncludeExpressions(primaryEntity).ToList());
        }

        private IEnumerable<string> BuildUsingStatements(string domainPackageName)
        {
            return new[]
            {
                "Koretech.Domains.Repositories",
                $"Koretech.Domains.{domainPackageName}.Entities",
                "Microsoft.EntityFrameworkCore"
            };
        }

        private IEnumerable<string> BuildIncludeExpressions(SharedModel.KamlBoEntity primaryEntity)
        {
            ArgumentNullException.ThrowIfNull(primaryEntity);

            if (primaryEntity.CompositeEntity == null)
            {
                yield break;
            }

            foreach (SharedModel.KamlBoEntityRelation relation in primaryEntity.ChildRelations)
            {
                if (relation.TargetEntity.Equals(primaryEntity.CompositeEntity.Name, StringComparison.OrdinalIgnoreCase))
                {
                    yield return relation.Name;
                }
            }
        }

        private void WriteRepositoryFile(RepositoryFileModel fileModel)
        {
            ArgumentNullException.ThrowIfNull(fileModel);

            if (File.Exists(fileModel.OutputFilePath))
            {
                File.Delete(fileModel.OutputFilePath);
            }

            string content = BuildRepositoryFileContent(fileModel);
            File.WriteAllText(fileModel.OutputFilePath, content);

            Console.WriteLine($"File {fileModel.OutputFilePath} generated.");
        }

        private string BuildRepositoryFileContent(RepositoryFileModel fileModel)
        {
            StringBuilder content = new();
            content.Append(GetFileHeader());
            content.AppendLine();

            AppendUsingStatements(content, fileModel);
            AppendNamespaceAndClassHeader(content, fileModel);
            AppendGetAllMethod(content, fileModel);
            AppendGetByPrimaryKeyMethod(content, fileModel);
            AppendCommandMethods(content, fileModel);
            AppendClassFooter(content);

            return content.ToString();
        }

        private void AppendUsingStatements(StringBuilder content, RepositoryFileModel fileModel)
        {
            foreach (string usingStatement in fileModel.UsingStatements)
            {
                content.AppendLine($"using {usingStatement};");
            }

            content.AppendLine();
        }

        private void AppendNamespaceAndClassHeader(StringBuilder content, RepositoryFileModel fileModel)
        {
            content.AppendLine($"namespace {fileModel.NamespaceName}");
            content.AppendLine("{");
            content.AppendLine($"\tinternal partial class {fileModel.DomainName}Repository : Repository<{fileModel.EntityTypeName}>");
            content.AppendLine("\t{");
            content.AppendLine($"\t\tpublic {fileModel.DomainName}Repository({fileModel.DomainName}Context dbContext) : base(dbContext) {{ }}");
            content.AppendLine();
        }

        private void AppendGetAllMethod(StringBuilder content, RepositoryFileModel fileModel)
        {
            content.AppendLine($"\t\tpublic async Task<IEnumerable<{fileModel.EntityTypeName}>> GetAllAsync()");
            content.AppendLine("\t\t{");
            content.AppendLine("\t\t\treturn await FindAll()");
            AppendIncludeExpressions(content, fileModel);
            content.AppendLine("\t\t\t\t.ToListAsync();");
            content.AppendLine("\t\t}");
            content.AppendLine();
        }

        private void AppendGetByPrimaryKeyMethod(StringBuilder content, RepositoryFileModel fileModel)
        {
            content.AppendLine($"\t\tpublic async Task<{fileModel.EntityTypeName}?> GetByPrimaryKeyAsync({fileModel.KeyParametersWithTypes})");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\treturn await FindByCondition({fileModel.KeyCondition})");
            AppendIncludeExpressions(content, fileModel);
            content.AppendLine("\t\t\t\t.FirstOrDefaultAsync();");
            content.AppendLine("\t\t}");
            content.AppendLine();
        }

        private void AppendCommandMethods(StringBuilder content, RepositoryFileModel fileModel)
        {
            content.AppendLine($"\t\tpublic void Insert({fileModel.EntityTypeName} entity)");
            content.AppendLine("\t\t{");
            content.AppendLine("\t\t\tbase.Insert(entity);");
            content.AppendLine("\t\t}");
            content.AppendLine();
            content.AppendLine($"\t\tpublic void Update({fileModel.EntityTypeName} entity)");
            content.AppendLine("\t\t{");
            content.AppendLine("\t\t\tbase.Update(entity);");
            content.AppendLine("\t\t}");
            content.AppendLine();
            content.AppendLine($"\t\tpublic void Delete({fileModel.EntityTypeName} entity)");
            content.AppendLine("\t\t{");
            content.AppendLine("\t\t\tbase.Delete(entity);");
            content.AppendLine("\t\t}");
            content.AppendLine();
        }

        private void AppendClassFooter(StringBuilder content)
        {
            content.AppendLine("\t}");
            content.AppendLine("}");
        }

        private void AppendIncludeExpressions(StringBuilder content, RepositoryFileModel fileModel)
        {
            foreach (string includeExpression in fileModel.IncludeExpressions)
            {
                content.AppendLine($"\t\t\t\t.Include(entity => entity.{includeExpression})");
            }
        }

        private sealed record RepositoryFileModel(
            string DomainName,
            string EntityTypeName,
            string NamespaceName,
            string OutputFilePath,
            IReadOnlyList<string> UsingStatements,
            string KeyParametersWithTypes,
            string KeyCondition,
            IReadOnlyList<string> IncludeExpressions);
    }
}
