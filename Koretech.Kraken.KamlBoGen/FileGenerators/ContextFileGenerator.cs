using Koretech.Kraken.Kaml;

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
        /// <param name="entity">All KAML BO specifications for the domain</param>
        public void CreateContextFile(List<KamlBoEntity> entities)
        {
            _ = DomainRoot ?? throw new InvalidOperationException($"DomainRoot must be set before calling {nameof(CreateContextFile)}");

            string domainName = DomainRoot.Name;
            IEnumerable<string> ownedEntityNames = entities
                .Where(e => !e.IsDomainPrimary)
                .Select(e => e.Name);

            string sourceFileName = Path.Combine(
                Path.Combine(outputRootDirectory.FullName, contextsPath), $"{domainName}Context.cs");
            if (File.Exists(sourceFileName))
            {
                File.Delete(sourceFileName);
            }

            var writer = File.CreateText(sourceFileName);
            writer.WriteLine("//");
            writer.WriteLine("// Created by Kraken KAML BO Generator");
            writer.WriteLine("//");
            writer.WriteLine();
            writer.WriteLine("using Koretech.Kraken.Data.Configurations;");
            writer.WriteLine("using Koretech.Kraken.Entities.KsUser;");
            writer.WriteLine("using Microsoft.EntityFrameworkCore;");
            writer.WriteLine();
            writer.WriteLine("namespace Koretech.Kraken.Data");
            writer.WriteLine("{");
            writer.WriteLine($"\tpublic class {domainName}Context : DbContext");
            writer.WriteLine("\t{");
            writer.WriteLine($"\t\tpublic {domainName}Context() {{ }}");
            writer.WriteLine("\t\t");
            writer.WriteLine($"\t\tpublic {domainName}Context(DbContextOptions<{domainName}Context> options) : base(options) {{ }}");
            writer.WriteLine();

            writer.WriteLine($"\t\tpublic virtual DbSet<{domainName}Entity> {domainName}s {{ get; set; }}"); //TODO: Fix plural on property naming
            foreach (string objectName in ownedEntityNames)
            {
                writer.WriteLine($"\t\tpublic virtual DbSet<{objectName}Entity> {objectName}s {{ get; set; }}");
            }
            writer.WriteLine();

            if (ScopeFunction != null)
            {
                writer.WriteLine("\t\t#region Scope Functions");
                writer.WriteLine();
                writer.WriteLine($"\t\tpublic IQueryable<{domainName}Entity> {domainName}EntityScope(string userId, string objectId, string methodName, int? scopeOverride)");
                writer.WriteLine($"\t\t\t=> FromExpression(() => {domainName}EntityScope(userId, objectId, methodName, scopeOverride));");
                writer.WriteLine();
                writer.WriteLine("\t\t#endregion Scope Functions");
                writer.WriteLine();
            }

            writer.WriteLine("\t\tprotected override void OnModelCreating(ModelBuilder modelBuilder)");
            writer.WriteLine("\t\t{");
            writer.WriteLine($"\t\t\tnew {domainName}EntityTypeConfiguration().Configure(modelBuilder.Entity<{domainName}Entity>());");
            foreach (string objectName in ownedEntityNames)
            {
                writer.WriteLine($"\t\t\tnew {objectName}EntityTypeConfiguration().Configure(modelBuilder.Entity<{objectName}Entity>());");
            }
            writer.WriteLine();
            writer.WriteLine("\t\t\tmodelBuilder.HasDefaultSchema(\"ks\");");
            writer.WriteLine($"\t\t\tmodelBuilder.HasDbFunction(typeof({domainName}Context).GetMethod(nameof({domainName}EntityScope)))");
            writer.WriteLine($"\t\t\t\t.HasName(\"{ScopeFunction}\");");
            writer.WriteLine("\t\t}");

            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Flush();
            writer.Close();
            Console.WriteLine($"File {sourceFileName} generated.");
        }
    }
}
