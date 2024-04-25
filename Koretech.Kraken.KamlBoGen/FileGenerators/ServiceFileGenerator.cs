using Koretech.Kraken.KamlBoGen.KamlBoModel;

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
        public void CreateDomainSubdirectory(KamlBoDomain domain)
        {
            outputRootDirectory.GetDirectories(servicesPath).Single().CreateSubdirectory(domain.Name);
        }

        /// <summary>
        /// Generates the two classes that make up a service based on a KAML BO specification.
        /// </summary>
        /// <param name="domain">The root of the KAML BO specification - the domain</param>
        protected override void DoGenerate(KamlBoDomain domain)
        {
            string entityFullPath = Path.Combine(outputRootDirectory.FullName, servicesPath);

            CreateServiceClassFile(domain, entityFullPath);
            CreateServiceInterfaceFile(domain, entityFullPath);
        }

        /// <summary>
        /// Generates a service class from a KAML BO specification.
        /// </summary>
        /// <param name="domain">model of the KAML BO specification root - the domain</param>
        private void CreateServiceClassFile(KamlBoDomain domain, string entityFullPath)
        {
            string domainName = domain.Name;
            string domainPackageName = domainName + "s";
            string primaryBusinessObjectName = domain.PrimaryEntityName;
            string sourceFileName = Path.Combine(entityFullPath, $"{domainName}Service_Gen.cs");
            {
                File.Delete(sourceFileName);
            }
            var writer = File.CreateText(sourceFileName);
            writer.Write(GetFileHeader());
            writer.WriteLine();
            writer.WriteLine($"using Koretech.Domains.{domainPackageName}.BusinessObjects;");
            writer.WriteLine($"using Koretech.Domains.{domainPackageName}.Repositories;");
            writer.WriteLine();
            writer.WriteLine($"namespace Koretech.Domains.{domainPackageName}");
            writer.WriteLine("{");
            writer.WriteLine($"\tinternal partial class {domainName}Service : I{domainName}Service");
            writer.WriteLine("\t{");
            writer.WriteLine($"\t\tprivate {domainName}Repository _repository;");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic {domainName}Service({domainName}Repository repository)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\t_repository = repository;");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic async Task<IEnumerable<{primaryBusinessObjectName}>> GetAllAsync()");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\treturn await _repository.GetAllAsync();");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic async Task<{primaryBusinessObjectName}?> GetByPrimaryKeyAsync({GetPrimaryKeyAsParameters(domain.PrimaryEntity, true)})");
            writer.WriteLine("\t\t{");
            writer.WriteLine($"\t\t\treturn await _repository.GetByPrimaryKeyAsync({GetPrimaryKeyAsParameters(domain.PrimaryEntity, false)});");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Insert({primaryBusinessObjectName} businessObject)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\t_repository.Insert(businessObject);");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Update({primaryBusinessObjectName} businessObject)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\t_repository.Update(businessObject);");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Delete({primaryBusinessObjectName} businessObject)");
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
        /// <param name="domain">model of the KAML BO specification root - the domain</param>
        private void CreateServiceInterfaceFile(KamlBoDomain domain, string entityFullPath)
        {
            string domainName = domain.Name;
            string domainPackageName = domainName + "s";
            string primaryBusinessObjectName = domain.PrimaryEntityName;
            string sourceFileName = Path.Combine(entityFullPath, $"I{domainName}Service_Gen.cs");
            {
                File.Delete(sourceFileName);
            }
            var writer = File.CreateText(sourceFileName);
            writer.Write(GetFileHeader());
            writer.WriteLine();
            writer.WriteLine($"using Koretech.Domains.{domainPackageName}.BusinessObjects;");
            writer.WriteLine($"using Koretech.Domains.{domainPackageName}.Repositories;");
            writer.WriteLine();
            writer.WriteLine($"namespace Koretech.Domains.{domainPackageName}");
            writer.WriteLine("{");
            writer.WriteLine($"\tpublic partial interface I{domainName}Service");
            writer.WriteLine("\t{");
            writer.WriteLine($"\t\tpublic Task<IEnumerable<{domainName}>> GetAllAsync();");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic Task<{primaryBusinessObjectName}?> GetByPrimaryKeyAsync({GetPrimaryKeyAsParameters(domain.PrimaryEntity, true)});");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Insert({primaryBusinessObjectName} businessObject);");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Update({primaryBusinessObjectName} businessObject);");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Delete({primaryBusinessObjectName} businessObject);");
            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Flush();
            writer.Close();
            Console.WriteLine($"File {sourceFileName} generated.");
        }
    }
}
