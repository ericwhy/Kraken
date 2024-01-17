using Koretech.Kraken.Kaml;

namespace Koretech.Kraken.KamlBoGen.FileGenerators
{
    internal class ServiceFileGenerator : FileGenerator
    {
        private const string servicesPath = "";

        public ServiceFileGenerator(DirectoryInfo outputRootDirectory) : base(outputRootDirectory)
        {
            generatePath = servicesPath;
        }

        /// <summary>
        /// Creates the subdirectory for storing entityBO files in a specific domain if it doesn't already exist.
        /// </summary>
        public void CreateDomainSubdirectory()
        {
            outputRootDirectory.GetDirectories(servicesPath).Single().CreateSubdirectory(DomainRoot.Name);
        }

        /// <summary>
        /// Generates the two classes that make up a service based on a KAML BO specification.
        /// This should only be called once per domain, for the root entity.
        /// </summary>
        /// <param name="rootEntity">KAML BO specification for the root of the domain</param>
        public void CreateServiceFile(KamlBoEntity rootEntity)
        {
            _ = DomainRoot ?? throw new InvalidOperationException($"DomainRoot must be set before calling {nameof(CreateServiceFile)}");

            string businessObjectName = rootEntity.Name;
            string entityName = businessObjectName + "Entity";
            string domainName = DomainRoot.Name;
            string entityFullPath = Path.Combine(outputRootDirectory.FullName, servicesPath);

            CreateServiceClassFile(rootEntity, domainName, entityName, businessObjectName, entityFullPath);
            CreateServiceInterfaceFile(rootEntity, domainName, entityName, businessObjectName, entityFullPath);
        }

        /// <summary>
        /// Generates a service class from a KAML BO specification.
        /// This should only be called once per domain, for the root entity.
        /// </summary>
        /// <param name="rootEntity">KAML BO specification for the domain root</param>
        private void CreateServiceClassFile(KamlBoEntity rootEntity, string domainName, string entityName, string businessObjectName, string entityFullPath)
        {
            string sourceFileName = Path.Combine(entityFullPath, $"{businessObjectName}Service.cs");
            {
                File.Delete(sourceFileName);
            }
            var writer = File.CreateText(sourceFileName);
            writer.Write(GetFileHeader());
            writer.WriteLine();
            writer.WriteLine($"using Koretech.Infrastructure.Services.{businessObjectName}.BusinessObjects;");
            writer.WriteLine($"using Koretech.Infrastructure.Services.{businessObjectName}.Repositories;");
            writer.WriteLine();
            writer.WriteLine($"namespace Koretech.Infrastructure.Services.{businessObjectName}");
            writer.WriteLine("{");
            writer.WriteLine($"\tinternal class {businessObjectName}Service : I{businessObjectName}Service");
            writer.WriteLine("\t{");
            writer.WriteLine($"\t\tprivate {businessObjectName}Repository _repository;");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic {businessObjectName}Service({businessObjectName}Repository repository)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\t_repository = repository;");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic async Task<IEnumerable<{businessObjectName}>> GetAllAsync()");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\treturn await _repository.GetAllAsync();");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic async Task<{businessObjectName}?> GetByPrimaryKeyAsync({GetPrimaryKeyAsParameters(rootEntity, true)})");
            writer.WriteLine("\t\t{");
            writer.WriteLine($"\t\t\treturn await _repository.GetByPrimaryKeyAsync({GetPrimaryKeyAsParameters(rootEntity, false)});");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Insert({businessObjectName} businessObject)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\t_repository.Insert(businessObject);");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Update({businessObjectName} businessObject)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\t_repository.Update(businessObject);");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Delete({businessObjectName} businessObject)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\t_repository.Delete(businessObject);");
            writer.WriteLine("\t\t}");
            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Flush();
            writer.Close();
            Console.WriteLine($"File {sourceFileName} generated.");
        }

        /// <summary>
        /// Generates a service interface from a KAML BO specification.
        /// This should only be called once per domain, for the root entity.
        /// </summary>
        /// <param name="rootEntity">KAML BO specification for the domain root</param>
        private void CreateServiceInterfaceFile(KamlBoEntity rootEntity, string domainName, string entityName, string businessObjectName, string entityFullPath)
        {
            string sourceFileName = Path.Combine(entityFullPath, $"I{businessObjectName}Service.cs");
            {
                File.Delete(sourceFileName);
            }
            var writer = File.CreateText(sourceFileName);
            writer.Write(GetFileHeader());
            writer.WriteLine();
            writer.WriteLine($"using Koretech.Infrastructure.Services.{businessObjectName}.BusinessObjects;");
            writer.WriteLine($"using Koretech.Infrastructure.Services.{businessObjectName}.Repositories;");
            writer.WriteLine();
            writer.WriteLine($"namespace Koretech.Infrastructure.Services.{businessObjectName}");
            writer.WriteLine("{");
            writer.WriteLine($"\tpublic interface I{businessObjectName}Service");
            writer.WriteLine("\t{");
            writer.WriteLine($"\t\tpublic {businessObjectName}Service({businessObjectName}Repository repository);");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic async Task<IEnumerable<{businessObjectName}>> GetAllAsync();");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic async Task<{businessObjectName}?> GetByPrimaryKeyAsync({GetPrimaryKeyAsParameters(rootEntity, true)});");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Insert({businessObjectName} businessObject);");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Update({businessObjectName} businessObject);");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Delete({businessObjectName} businessObject);");
            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Flush();
            writer.Close();
            Console.WriteLine($"File {sourceFileName} generated.");
        }
    }
}
