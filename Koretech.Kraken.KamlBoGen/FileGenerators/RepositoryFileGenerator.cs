using Koretech.Kraken.KamlBoGen.KamlBoModel;

namespace Koretech.Kraken.KamlBoGen.FileGenerators
{
    internal class RepositoryFileGenerator : FileGenerator
    {

        private const string repositoriesPath = "Repositories";

        public RepositoryFileGenerator(DirectoryInfo outputRootDirectory) : base(outputRootDirectory)
        {
            generatePath = repositoriesPath;
        }

        /// <summary>
        /// Generates a repository class from a KAML BO specification.
        /// </summary>
        /// <param name="domain">The root of the KAML BO specification - the domain</param>
        protected override void DoGenerate(KamlBoDomain domain)
        {
            string primaryEntityName = domain.PrimaryEntityName;
            string entityName = primaryEntityName + "Entity";
            string domainName = domain.Name;
            string domainPackageName = domainName + "s";
            string entityFullPath = Path.Combine(outputRootDirectory.FullName, repositoriesPath);
            string sourceFileName = Path.Combine(entityFullPath, $"{domainName}Repository_Gen.cs");
            {
                File.Delete(sourceFileName);
            }
            var writer = File.CreateText(sourceFileName);
            writer.Write(GetFileHeader());
            writer.WriteLine();
            writer.WriteLine($"using Koretech.Domains.Repositories;");
            writer.WriteLine($"using Koretech.Domains.{domainPackageName}.BusinessObjects;");
            writer.WriteLine($"using Koretech.Domains.{domainPackageName}.Entities;");
            writer.WriteLine("using Microsoft.EntityFrameworkCore;");
            writer.WriteLine();
            writer.WriteLine($"namespace Koretech.Domains.{domainPackageName}.Repositories");
            writer.WriteLine("{");
            writer.WriteLine($"\tinternal partial class {domainName}Repository : Repository<{entityName}>");
            writer.WriteLine("\t{");
            writer.WriteLine($"\t\tpublic {domainName}Repository({domainName}Context dbContext) : base(dbContext) {{ }}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic async Task<IEnumerable<{primaryEntityName}>> GetAllAsync()");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\tvar entities = await FindAll()");
            writer.WriteLine("\t\t\t\t.ToListAsync();");
            writer.WriteLine($"\t\t\tIList<{primaryEntityName}> businessObjects = {primaryEntityName}.NewInstance(entities);");
            writer.WriteLine("\t\t\treturn businessObjects;");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic async Task<{primaryEntityName}?> GetByPrimaryKeyAsync({GetPrimaryKeyAsParameters(domain.PrimaryEntity, true)})");
            writer.WriteLine("\t\t{");
            writer.WriteLine($"\t\t\t{entityName}? entity = await FindByCondition({GetPrimaryKeyAsCondition(domain.PrimaryEntity)})");
            writer.WriteLine("\t\t\t\t.FirstOrDefaultAsync();");
            writer.WriteLine($"\t\t\t{primaryEntityName}? businessObject = (entity != null) ? {primaryEntityName}.NewInstance(entity) : null;");
            writer.WriteLine("\t\t\treturn businessObject;");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Insert({primaryEntityName} businessObject)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\tInsert(businessObject.Entity);");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Update({primaryEntityName} businessObject)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\tUpdate(businessObject.Entity);");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Delete({primaryEntityName} businessObject)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\tDelete(businessObject.Entity);");
            writer.WriteLine("\t\t}");
            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Flush();
            writer.Close();
            Console.WriteLine($"File {sourceFileName} generated.");
        }

        /// <summary>
        /// Gets a string containing a lambda expression for matching the primary key(s) of the given entity.
        /// </summary>
        /// <param name="entity">Representation of the entity from KAMLBO</param>
        /// <returns>a string</returns>
        private string GetPrimaryKeyAsCondition(KamlBoEntity entity)
        {
            string result = string.Empty;
            foreach (KamlEntityProperty property in entity.Properties)
            {
                if (property.IsKey)
                {
                    if (string.IsNullOrEmpty(result))
                    {
                        result = "e => ";
                    }
                    else
                    {
                        result += " && ";
                    }
                    result += $"e.{property.Name}.Equals({property.Name})";
                }
            }
            return result;
        }
    }
}
