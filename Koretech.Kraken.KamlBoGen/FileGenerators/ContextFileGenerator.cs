using System.Text;
using SharedModel = Koretech.Kraken.KamlBoModel.Model;

namespace Koretech.Kraken.KamlBoGen.FileGenerators
{
    internal class ContextFileGenerator : FileGenerator
    {
        private const string ContextsPath = "Contexts";

        public ContextFileGenerator(DirectoryInfo outputRootDirectory) : base(outputRootDirectory)
        {
            generatePath = ContextsPath;
        }

        protected override void DoGenerate(SharedModel.KamlBoDomain domain)
        {
            ArgumentNullException.ThrowIfNull(domain);

            ContextFileModel fileModel = BuildContextFileModel(domain);
            WriteContextFile(fileModel);
        }

        private ContextFileModel BuildContextFileModel(SharedModel.KamlBoDomain domain)
        {
            string domainPackageName = domain.Name + "s";
            string primaryEntityName = domain.PrimaryEntityName;
            string namespaceName = $"Koretech.Domains.{domainPackageName}.Repositories";
            string outputFilePath = Path.Combine(outputRootDirectory.FullName, ContextsPath, $"{domain.Name}Context.cs");
            List<RelationConfigurationModel> relationModels = BuildRelationModels(domain).ToList();
            HashSet<string> configuredNavigationKeys = relationModels
                .SelectMany(r => r.ConfiguredNavigationKeys)
                .ToHashSet(StringComparer.Ordinal);

            return new ContextFileModel(
                domain.Name,
                primaryEntityName,
                namespaceName,
                outputFilePath,
                BuildUsingStatements(domainPackageName).ToList(),
                BuildDbSets(domain).ToList(),
                BuildConfigurationRegistrations(domain).ToList(),
                BuildScopeFunctionModel(domain),
                relationModels,
                BuildIgnoredNavigationModels(domain, configuredNavigationKeys).ToList());
        }

        private IEnumerable<string> BuildUsingStatements(string domainPackageName)
        {
            return new[]
            {
                $"Koretech.Domains.{domainPackageName}.Entities",
                $"Koretech.Domains.{domainPackageName}.EntityConfigurations",
                "Microsoft.EntityFrameworkCore"
            };
        }

        private IEnumerable<DbSetModel> BuildDbSets(SharedModel.KamlBoDomain domain)
        {
            foreach (SharedModel.KamlBoEntity entity in domain.Entities)
            {
                yield return new DbSetModel(entity.Name, entity.Name + "s");
            }
        }

        private IEnumerable<string> BuildConfigurationRegistrations(SharedModel.KamlBoDomain domain)
        {
            foreach (SharedModel.KamlBoEntity entity in domain.Entities)
            {
                yield return $"\t\t\tmodelBuilder.ApplyConfiguration(new {entity.Name}EntityTypeConfiguration());";
            }
        }

        private ScopeFunctionModel? BuildScopeFunctionModel(SharedModel.KamlBoDomain domain)
        {
            if (string.IsNullOrWhiteSpace(domain.ScopeFunction))
            {
                return null;
            }

            string methodName = $"{domain.Name}EntityScope";
            return new ScopeFunctionModel(methodName, domain.ScopeFunction);
        }

        private IEnumerable<RelationConfigurationModel> BuildRelationModels(SharedModel.KamlBoDomain domain)
        {
            foreach (SharedModel.KamlBoEntity entity in domain.Entities)
            {
                foreach (SharedModel.KamlBoEntityRelation relation in entity.OwnerRelations)
                {
                    yield return BuildRelationConfiguration(entity, relation);
                }
            }
        }

        private RelationConfigurationModel BuildRelationConfiguration(
            SharedModel.KamlBoEntity entity,
            SharedModel.KamlBoEntityRelation relation)
        {
            string targetDomainName = relation.IsCrossDomain ? relation.TargetDomain : "the same";
            string relationshipBuilderMethod = "HasOne";
            string inverseBuilderMethod = relation.IsToOwnerOne ? "WithOne" : "WithMany";
            string inverseNavigation = GetInverseNavigationExpression(relation);
            string foreignKeyExpression = GetForeignKeyExpression(entity.Name, relation, relation.IsToOwnerOne);
            List<string> configuredNavigationKeys = new() { GetNavigationKey(entity.Name, relation.Name) };
            if (relation.IsTargetLoaded && relation.InverseRelation != null)
            {
                configuredNavigationKeys.Add(GetNavigationKey(relation.TargetEntity, relation.InverseRelation.Name));
            }

            return new RelationConfigurationModel(
                relation.Name,
                entity.Name,
                relation.TargetEntity,
                targetDomainName,
                relation.TypeName,
                $"\t\t\tmodelBuilder.Entity<{entity.Name}Entity>()",
                $"\t\t\t\t.{relationshipBuilderMethod}(entity => entity.{relation.Name})",
                $"\t\t\t\t.{inverseBuilderMethod}({inverseNavigation})",
                foreignKeyExpression,
                configuredNavigationKeys);
        }

        private IEnumerable<IgnoredNavigationModel> BuildIgnoredNavigationModels(
            SharedModel.KamlBoDomain domain,
            HashSet<string> configuredNavigationKeys)
        {
            foreach (SharedModel.KamlBoEntity entity in domain.Entities)
            {
                foreach (SharedModel.KamlBoEntityRelation relation in entity.Relations)
                {
                    string navigationKey = GetNavigationKey(entity.Name, relation.Name);
                    if (!configuredNavigationKeys.Contains(navigationKey))
                    {
                        yield return new IgnoredNavigationModel(entity.Name, relation.Name);
                    }
                }
            }
        }

        private string GetInverseNavigationExpression(SharedModel.KamlBoEntityRelation relation)
        {
            if (!relation.IsTargetLoaded || relation.Target == null || relation.InverseRelation == null)
            {
                return string.Empty;
            }

            return $"target => target.{relation.InverseRelation.Name}";
        }

        private string GetForeignKeyExpression(string entityName, SharedModel.KamlBoEntityRelation relation, bool isOneToOne)
        {
            if (relation.KeyMap.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Cannot generate relationship because no key mapping was found for relation {relation.Name} on entity {entityName}.");
            }

            if (relation.KeyMap.Count == 1)
            {
                string foreignKeyProperty = relation.KeyMap.Keys.First();
                return isOneToOne
                    ? $"\t\t\t\t.HasForeignKey<{entityName}Entity>(entity => entity.{foreignKeyProperty});"
                    : $"\t\t\t\t.HasForeignKey(entity => entity.{foreignKeyProperty});";
            }

            string keyList = string.Join(", ", relation.KeyMap.Keys.Select(key => $"entity.{key}"));
            return isOneToOne
                ? $"\t\t\t\t.HasForeignKey<{entityName}Entity>(entity => new {{ {keyList} }});"
                : $"\t\t\t\t.HasForeignKey(entity => new {{ {keyList} }});";
        }

        private void WriteContextFile(ContextFileModel fileModel)
        {
            if (File.Exists(fileModel.OutputFilePath))
            {
                File.Delete(fileModel.OutputFilePath);
            }

            string content = BuildContextFileContent(fileModel);
            File.WriteAllText(fileModel.OutputFilePath, content);

            Console.WriteLine($"File {fileModel.OutputFilePath} generated.");
        }

        private string BuildContextFileContent(ContextFileModel fileModel)
        {
            StringBuilder content = new();
            content.Append(GetFileHeader());
            content.AppendLine();

            AppendUsingStatements(content, fileModel);
            AppendNamespaceAndClassHeader(content, fileModel);
            AppendConstructors(content, fileModel);
            AppendDbSets(content, fileModel);
            AppendScopeFunction(content, fileModel);
            AppendOnModelCreating(content, fileModel);
            AppendClassFooter(content);

            return content.ToString();
        }

        private void AppendUsingStatements(StringBuilder content, ContextFileModel fileModel)
        {
            foreach (string usingStatement in fileModel.UsingStatements)
            {
                content.AppendLine($"using {usingStatement};");
            }

            content.AppendLine();
        }

        private void AppendNamespaceAndClassHeader(StringBuilder content, ContextFileModel fileModel)
        {
            content.AppendLine($"namespace {fileModel.NamespaceName}");
            content.AppendLine("{");
            content.AppendLine($"\tpublic class {fileModel.DomainName}Context : DbContext");
            content.AppendLine("\t{");
        }

        private void AppendConstructors(StringBuilder content, ContextFileModel fileModel)
        {
            content.AppendLine($"\t\tpublic {fileModel.DomainName}Context() {{ }}");
            content.AppendLine("\t\t");
            content.AppendLine($"\t\tpublic {fileModel.DomainName}Context(DbContextOptions<{fileModel.DomainName}Context> options) : base(options) {{ }}");
            content.AppendLine();
        }

        private void AppendDbSets(StringBuilder content, ContextFileModel fileModel)
        {
            foreach (DbSetModel dbSet in fileModel.DbSets)
            {
                content.AppendLine($"\t\tpublic virtual DbSet<{dbSet.EntityName}Entity> {dbSet.PropertyName} {{ get; set; }}");
            }

            content.AppendLine();
        }

        private void AppendScopeFunction(StringBuilder content, ContextFileModel fileModel)
        {
            if (fileModel.ScopeFunction == null)
            {
                return;
            }

            content.AppendLine("\t\t// Scope Function");
            content.AppendLine($"\t\tpublic IQueryable<{fileModel.PrimaryEntityName}Entity> {fileModel.ScopeFunction.MethodName}(string userId, string objectId, string methodName, int? scopeOverride)");
            content.AppendLine($"\t\t\t=> FromExpression(() => {fileModel.ScopeFunction.MethodName}(userId, objectId, methodName, scopeOverride));");
            content.AppendLine();
        }

        private void AppendOnModelCreating(StringBuilder content, ContextFileModel fileModel)
        {
            content.AppendLine("\t\tprotected override void OnModelCreating(ModelBuilder modelBuilder)");
            content.AppendLine("\t\t{");
            content.AppendLine("\t\t\t// Configure entity types");
            foreach (string registration in fileModel.ConfigurationRegistrations)
            {
                content.AppendLine(registration);
            }

            content.AppendLine();
            content.AppendLine("\t\t\tmodelBuilder.HasDefaultSchema(\"ks\");");
            if (fileModel.ScopeFunction != null)
            {
                content.AppendLine($"\t\t\tmodelBuilder.HasDbFunction(typeof({fileModel.DomainName}Context).GetMethod(nameof({fileModel.ScopeFunction.MethodName}))!)");
                content.AppendLine($"\t\t\t\t.HasName(\"{fileModel.ScopeFunction.DbFunctionName}\");");
            }

            if (fileModel.Relations.Count > 0)
            {
                content.AppendLine();
                content.AppendLine("\t\t\t// NOTE: We only configure relations from child to parent.");
                content.AppendLine("\t\t\t// Relations from parent to child are redundant.");
            }

            foreach (RelationConfigurationModel relation in fileModel.Relations)
            {
                content.AppendLine();
                content.AppendLine($"\t\t\t// Relation '{relation.RelationName}' from {relation.SourceEntityName} to {relation.TargetEntityName} in {relation.TargetDomainName} domain");
                content.AppendLine($"\t\t\t// Cardinality: {relation.CardinalityDescription}");
                content.AppendLine(relation.EntityLine);
                content.AppendLine(relation.HasLine);
                content.AppendLine(relation.WithLine);
                content.AppendLine(relation.ForeignKeyLine);
            }

            if (fileModel.IgnoredNavigations.Count > 0)
            {
                content.AppendLine();
                content.AppendLine("\t\t\t// Ignore navigation properties that are not used by explicit relationship configuration.");
                content.AppendLine("\t\t\t// This prevents EF Core from inferring additional relationships.");
                foreach (IgnoredNavigationModel ignoredNavigation in fileModel.IgnoredNavigations)
                {
                    content.AppendLine($"\t\t\tmodelBuilder.Entity<{ignoredNavigation.EntityName}Entity>().Ignore(e => e.{ignoredNavigation.NavigationPropertyName});");
                }
            }

            content.AppendLine("\t\t}");
        }

        private void AppendClassFooter(StringBuilder content)
        {
            content.AppendLine("\t}");
            content.AppendLine("}");
        }

        private static string GetNavigationKey(string entityName, string navigationPropertyName)
        {
            return $"{entityName}.{navigationPropertyName}";
        }

        private sealed record ContextFileModel(
            string DomainName,
            string PrimaryEntityName,
            string NamespaceName,
            string OutputFilePath,
            IReadOnlyList<string> UsingStatements,
            IReadOnlyList<DbSetModel> DbSets,
            IReadOnlyList<string> ConfigurationRegistrations,
            ScopeFunctionModel? ScopeFunction,
            IReadOnlyList<RelationConfigurationModel> Relations,
            IReadOnlyList<IgnoredNavigationModel> IgnoredNavigations);

        private sealed record DbSetModel(string EntityName, string PropertyName);

        private sealed record ScopeFunctionModel(string MethodName, string DbFunctionName);

        private sealed record RelationConfigurationModel(
            string RelationName,
            string SourceEntityName,
            string TargetEntityName,
            string TargetDomainName,
            string CardinalityDescription,
            string EntityLine,
            string HasLine,
            string WithLine,
            string ForeignKeyLine,
            IReadOnlyList<string> ConfiguredNavigationKeys);

        private sealed record IgnoredNavigationModel(
            string EntityName,
            string NavigationPropertyName);
    }
}
