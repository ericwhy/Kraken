using Koretech.Kraken.KamlBoGen.KamlBoModel;
using System.Reflection.Emit;

namespace Koretech.Kraken.KamlBoGen.FileGenerators
{
    internal class ContextFileGenerator : FileGenerator
    {
        private const string contextsPath = "Contexts";

        public string? ScopeFunction { get; set; }

        public ContextFileGenerator(DirectoryInfo outputRootDirectory) : base(outputRootDirectory)
        {
            generatePath = contextsPath;
        }

        /// <summary>
        /// Generates an EF context class from a KAML BO specification.
        /// </summary>
        /// <param name="domain">model of the KAML BO specification for the domain</param>
        protected override void DoGenerate(KamlBoDomain domain)
        {
            string domainName = domain.Name;
            string domainPackageName = domainName + "s";
            string primaryEntityName = domain.PrimaryEntityName;
            IEnumerable<string> ownedEntityNames = domain.Entities
                .Where(e => !e.IsDomainPrimary)
                .Select(e => e.Name);

            string sourceFileName = Path.Combine(
                Path.Combine(outputRootDirectory.FullName, contextsPath), $"{domainName}Context.cs");
            if (File.Exists(sourceFileName))
            {
                File.Delete(sourceFileName);
            }

            var writer = File.CreateText(sourceFileName);
            writer.Write(GetFileHeader());
            writer.WriteLine();
            writer.WriteLine($"using Koretech.Domains.{domainPackageName}.Entities;");
            writer.WriteLine($"using Koretech.Domains.{domainPackageName}.EntityConfigurations;");
            writer.WriteLine("using Microsoft.EntityFrameworkCore;");
            writer.WriteLine();
            writer.WriteLine($"namespace Koretech.Domains.{domainPackageName}.Repositories");
            writer.WriteLine("{");
            writer.WriteLine($"\tpublic class {domainName}Context : DbContext");
            writer.WriteLine("\t{");
            writer.WriteLine($"\t\tpublic {domainName}Context() {{ }}");
            writer.WriteLine("\t\t");
            writer.WriteLine($"\t\tpublic {domainName}Context(DbContextOptions<{domainName}Context> options) : base(options) {{ }}");
            writer.WriteLine();

            writer.WriteLine($"\t\tpublic virtual DbSet<{primaryEntityName}Entity> {primaryEntityName}s {{ get; set; }}"); //TODO: Fix plural on property naming
            foreach (string ownedEntityName in ownedEntityNames)
            {
                writer.WriteLine($"\t\tpublic virtual DbSet<{ownedEntityName}Entity> {ownedEntityName}s {{ get; set; }}");
            }
            writer.WriteLine();

            if (ScopeFunction != null)
            {
                writer.WriteLine("\t\t// Scope Function");
                writer.WriteLine($"\t\tpublic IQueryable<{primaryEntityName}Entity> {domainName}EntityScope(string userId, string objectId, string methodName, int? scopeOverride)");
                writer.WriteLine($"\t\t\t=> FromExpression(() => {domainName}EntityScope(userId, objectId, methodName, scopeOverride));");
                writer.WriteLine();
            }

            writer.WriteLine("\t\tprotected override void OnModelCreating(ModelBuilder modelBuilder)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t\t// Configure entity types");
            writer.WriteLine($"\t\t\tmodelBuilder.ApplyConfigurationsFromAssembly(typeof({primaryEntityName}Entity).Assembly);");
            //writer.WriteLine($"\t\t\tnew {primaryEntityName}EntityTypeConfiguration().Configure(modelBuilder.Entity<{primaryEntityName}Entity>());");
            //foreach (string ownedEntityName in ownedEntityNames)
            //{
            //    writer.WriteLine($"\t\t\tnew {ownedEntityName}EntityTypeConfiguration().Configure(modelBuilder.Entity<{ownedEntityName}Entity>());");
            //}
            writer.WriteLine();
            writer.WriteLine("\t\t\tmodelBuilder.HasDefaultSchema(\"ks\");");
            writer.WriteLine($"\t\t\tmodelBuilder.HasDbFunction(typeof({domainName}Context).GetMethod(nameof({domainName}EntityScope))!)");
            writer.WriteLine($"\t\t\t\t.HasName(\"{ScopeFunction}\");");

            foreach (KamlBoEntity entity in domain.Entities)
            {
                foreach (KamlEntityRelation relation in entity.Relations)
                {
                    string? targetDomainName = relation.IsCrossDomain ? relation.TargetDomain : "this";

                    writer.WriteLine();
                    writer.WriteLine($"\t\t\t// Relation '{relation.Name}' from {entity.Name} to {relation.TargetEntity} in {targetDomainName} domain");
                    writer.WriteLine($"\t\t\t// Cardinality: {relation.TypeName}");

                    string hasMethodName = (relation.IsToOne || relation.IsToOwnerOne) ? "HasOne" : "HasMany";
                    string withMethodName = (relation.IsToOne || relation.IsToOwnerOne) ? "WithMany" : "WithOne";

                    writer.WriteLine($"\t\t\tmodelBuilder.Entity<{entity.Name}Entity>()");
                    writer.WriteLine($"\t\t\t\t.{hasMethodName}(entity => entity.{relation.Name})");
                    writer.WriteLine($"\t\t\t\t.{withMethodName}()");
                    if (relation.KeyMap.Count == 1)
                    { 
                        writer.WriteLine($"\t\t\t\t.HasForeignKey(target => target.{relation.KeyMap.Values.First()});");
                    }
                    else if (relation.KeyMap.Count == 0)
                    {
                        throw new Exception($"Cannot generate relationship because no key mapping was found for relation {relation.Name} on entity {entity.Name}");
                    }
                    else
                    {
                        string keyList = string.Empty;
                        bool firstTime = true;
                        foreach (string key in relation.KeyMap.Values)
                        {
                            keyList = $"{keyList}{(firstTime ? string.Empty : ", ")}target.{key}";
                            writer.WriteLine($"\t\t\t\t.HasForeignKey(target => new {{ {keyList} }});");
                            firstTime = false;
                        }

                    }
                }
            }

            writer.WriteLine("\t\t}");
            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Flush();
            writer.Close();
            Console.WriteLine($"File {sourceFileName} generated.");
        }
    }
}
