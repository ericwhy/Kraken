using System.Text;
using SharedModel = Koretech.Kraken.KamlBoModel.Model;

namespace Koretech.Kraken.KamlBoGen.FileGenerators
{
    internal class ServiceFileGenerator : FileGenerator
    {
        private const string ServicesPath = "";

        public ServiceFileGenerator(DirectoryInfo outputRootDirectory) : base(outputRootDirectory)
        {
            generatePath = ServicesPath;
        }

        protected override void DoGenerate(SharedModel.KamlBoDomain domain)
        {
            ArgumentNullException.ThrowIfNull(domain);

            SharedModel.KamlBoEntity primaryEntity = domain.GetEntity(domain.PrimaryEntityName)
                ?? throw new InvalidOperationException(
                    $"Could not find primary entity '{domain.PrimaryEntityName}' in domain '{domain.Name}'.");

            ServiceFileModel fileModel = BuildServiceFileModel(domain, primaryEntity);
            WriteServiceClassFile(fileModel);
            WriteServiceInterfaceFile(fileModel);
        }

        private ServiceFileModel BuildServiceFileModel(SharedModel.KamlBoDomain domain, SharedModel.KamlBoEntity primaryEntity)
        {
            string domainName = domain.Name;
            string domainPackageName = $"{domainName}s";
            string entityTypeName = $"{primaryEntity.Name}Entity";
            string namespaceName = $"Koretech.Domains.{domainPackageName}";
            string outputDirectoryPath = Path.Combine(outputRootDirectory.FullName, ServicesPath);

            return new ServiceFileModel(
                domainName,
                entityTypeName,
                namespaceName,
                Path.Combine(outputDirectoryPath, $"{domainName}Service_Gen.cs"),
                Path.Combine(outputDirectoryPath, $"I{domainName}Service_Gen.cs"),
                BuildUsingStatements(domainPackageName).ToList(),
                primaryEntity.Key.AsParameters(true, entity: primaryEntity),
                primaryEntity.Key.AsParameters(false));
        }

        private IEnumerable<string> BuildUsingStatements(string domainPackageName)
        {
            return new[]
            {
                $"Koretech.Domains.{domainPackageName}.Entities",
                $"Koretech.Domains.{domainPackageName}.Repositories"
            };
        }

        private void WriteServiceClassFile(ServiceFileModel fileModel)
        {
            if (File.Exists(fileModel.ClassOutputFilePath))
            {
                File.Delete(fileModel.ClassOutputFilePath);
            }

            string content = BuildServiceClassContent(fileModel);
            File.WriteAllText(fileModel.ClassOutputFilePath, content);

            Console.WriteLine($"File {fileModel.ClassOutputFilePath} generated.");
        }

        private void WriteServiceInterfaceFile(ServiceFileModel fileModel)
        {
            if (File.Exists(fileModel.InterfaceOutputFilePath))
            {
                File.Delete(fileModel.InterfaceOutputFilePath);
            }

            string content = BuildServiceInterfaceContent(fileModel);
            File.WriteAllText(fileModel.InterfaceOutputFilePath, content);

            Console.WriteLine($"File {fileModel.InterfaceOutputFilePath} generated.");
        }

        private string BuildServiceClassContent(ServiceFileModel fileModel)
        {
            StringBuilder content = new();
            content.Append(GetFileHeader());
            content.AppendLine();

            AppendUsingStatements(content, fileModel);
            content.AppendLine($"namespace {fileModel.NamespaceName}");
            content.AppendLine("{");
            content.AppendLine($"\tinternal partial class {fileModel.DomainName}Service : I{fileModel.DomainName}Service");
            content.AppendLine("\t{");
            content.AppendLine($"\t\tprivate readonly {fileModel.DomainName}Repository _repository;");
            content.AppendLine();
            content.AppendLine($"\t\tpublic {fileModel.DomainName}Service({fileModel.DomainName}Repository repository)");
            content.AppendLine("\t\t{");
            content.AppendLine("\t\t\t_repository = repository;");
            content.AppendLine("\t\t}");
            content.AppendLine();
            content.AppendLine($"\t\tpublic async Task<IEnumerable<{fileModel.EntityTypeName}>> GetAllAsync()");
            content.AppendLine("\t\t{");
            content.AppendLine("\t\t\treturn await _repository.GetAllAsync();");
            content.AppendLine("\t\t}");
            content.AppendLine();
            content.AppendLine($"\t\tpublic async Task<{fileModel.EntityTypeName}?> GetByPrimaryKeyAsync({fileModel.KeyParametersWithTypes})");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\treturn await _repository.GetByPrimaryKeyAsync({fileModel.KeyParameters});");
            content.AppendLine("\t\t}");
            content.AppendLine();
            content.AppendLine($"\t\tpublic void Insert({fileModel.EntityTypeName} entity)");
            content.AppendLine("\t\t{");
            content.AppendLine("\t\t\t_repository.Insert(entity);");
            content.AppendLine("\t\t}");
            content.AppendLine();
            content.AppendLine($"\t\tpublic void Update({fileModel.EntityTypeName} entity)");
            content.AppendLine("\t\t{");
            content.AppendLine("\t\t\t_repository.Update(entity);");
            content.AppendLine("\t\t}");
            content.AppendLine();
            content.AppendLine($"\t\tpublic void Delete({fileModel.EntityTypeName} entity)");
            content.AppendLine("\t\t{");
            content.AppendLine("\t\t\t_repository.Delete(entity);");
            content.AppendLine("\t\t}");
            content.AppendLine("\t}");
            content.AppendLine("}");

            return content.ToString();
        }

        private string BuildServiceInterfaceContent(ServiceFileModel fileModel)
        {
            StringBuilder content = new();
            content.Append(GetFileHeader());
            content.AppendLine();

            AppendUsingStatements(content, fileModel);
            content.AppendLine($"namespace {fileModel.NamespaceName}");
            content.AppendLine("{");
            content.AppendLine($"\tpublic partial interface I{fileModel.DomainName}Service");
            content.AppendLine("\t{");
            content.AppendLine($"\t\tTask<IEnumerable<{fileModel.EntityTypeName}>> GetAllAsync();");
            content.AppendLine();
            content.AppendLine($"\t\tTask<{fileModel.EntityTypeName}?> GetByPrimaryKeyAsync({fileModel.KeyParametersWithTypes});");
            content.AppendLine();
            content.AppendLine($"\t\tvoid Insert({fileModel.EntityTypeName} entity);");
            content.AppendLine();
            content.AppendLine($"\t\tvoid Update({fileModel.EntityTypeName} entity);");
            content.AppendLine();
            content.AppendLine($"\t\tvoid Delete({fileModel.EntityTypeName} entity);");
            content.AppendLine("\t}");
            content.AppendLine("}");

            return content.ToString();
        }

        private void AppendUsingStatements(StringBuilder content, ServiceFileModel fileModel)
        {
            foreach (string usingStatement in fileModel.UsingStatements)
            {
                content.AppendLine($"using {usingStatement};");
            }

            content.AppendLine();
        }

        private sealed record ServiceFileModel(
            string DomainName,
            string EntityTypeName,
            string NamespaceName,
            string ClassOutputFilePath,
            string InterfaceOutputFilePath,
            IReadOnlyList<string> UsingStatements,
            string KeyParametersWithTypes,
            string KeyParameters);
    }
}
