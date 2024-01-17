using Koretech.Kraken.Kaml;

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
        /// This should only be called once per domain, for the root entity.
        /// </summary>
        /// <param name="rootEntity">KAML BO specification for the domain root</param>
        public void CreateRepositoryFile(KamlBoEntity rootEntity)
        {
            _ = DomainRoot ?? throw new InvalidOperationException($"DomainRoot must be set before calling {nameof(CreateRepositoryFile)}");

            string businessObjectName = rootEntity.Name;
            string entityName = businessObjectName + "Entity";
            string domainName = DomainRoot.Name;
            string entityFullPath = Path.Combine(outputRootDirectory.FullName, repositoriesPath);
            string sourceFileName = Path.Combine(entityFullPath, $"{businessObjectName}Repository.cs");
            {
                File.Delete(sourceFileName);
            }
            var writer = File.CreateText(sourceFileName);
            writer.Write(GetFileHeader());
            writer.WriteLine();
            writer.WriteLine($"using Koretech.Kraken.BusinessObjects.{businessObjectName};");
            writer.WriteLine("using Koretech.Kraken.Contexts;");
            writer.WriteLine($"using Koretech.Kraken.Entities.{businessObjectName};");
            writer.WriteLine("using Microsoft.EntityFrameworkCore;");
            writer.WriteLine();
            writer.WriteLine($"namespace Koretech.Kraken.Repositories");
            writer.WriteLine("{");
            writer.WriteLine($"\tinternal class {businessObjectName}Repository : Repository<{entityName}>");
            writer.WriteLine("\t{");
            writer.WriteLine($"\t\tpublic {businessObjectName}Repository({businessObjectName}Context dbContext) : base(dbContext) {{ }}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic async Task<IEnumerable<{businessObjectName}>> GetAllAsync()");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\tvar entities = await FindAll()");
            writer.WriteLine("\t\t\t\t.ToListAsync();");
            writer.WriteLine($"\t\t\tIList<{businessObjectName}> businessObjects = {businessObjectName}.NewInstance(entities);");
            writer.WriteLine("\t\t\treturn businessObjects;");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic async Task<{businessObjectName}?> GetByPrimaryKeyAsync({GetPrimaryKeyAsParameters(rootEntity, true)})");
            writer.WriteLine("\t\t{");
            writer.WriteLine($"\t\t\t{entityName}? entity = await FindByCondition({GetPrimaryKeyAsCondition(rootEntity)})");
            writer.WriteLine("\t\t\t\t.FirstOrDefaultAsync();");
            writer.WriteLine($"\t\t\t{businessObjectName}? businessObject = (entity != null) ? {businessObjectName}.NewInstance(entity) : null;");
            writer.WriteLine("\t\t\treturn businessObject;");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Insert({businessObjectName} businessObject)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\tInsert(businessObject.Entity);");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Update({businessObjectName} businessObject)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\tUpdate(businessObject.Entity);");
            writer.WriteLine("\t\t}");
            writer.WriteLine();
            writer.WriteLine($"\t\tpublic void Delete({businessObjectName} businessObject)");
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
                    result += $"e.{property.Name}.Equals(keyValue)";
                }
            }
            return result;
        }
    }
}
