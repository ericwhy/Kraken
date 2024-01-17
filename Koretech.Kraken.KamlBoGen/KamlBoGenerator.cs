using Koretech.Kraken.KamlBoGen;
using Koretech.Kraken.KamlBoGen.FileGenerators;
using System.Net;
using System.Xml.Linq;

namespace Koretech.Kraken.Kaml
{
    public class KamlBoGen {
        public KamlBoGen(FileInfo source, DirectoryInfo outPath) {
            SourceKamlBo = source;
            OutputRoot = outPath;
        }

        public FileInfo SourceKamlBo { get; private set; }
        public DirectoryInfo OutputRoot { get; private set; }

        private const string NameA = "Name";
        private const string LabelA = "Label";
        private const string TypeA = "Type";
        private const string LengthA = "Length";
        private const string IsRequiredA = "IsRequired";
        private const string IsKeyA = "IsKey";
        private const string IsIdentityA = "IsIdentity";
        private const string IsFixedLengthA = "IsFixedLength";
        private const string TableA = "Table";
        private const string ColumnA = "Column";
        private const string TargetObjectA = "TargetObject";
        private const string SourcePropertyA = "SourceProperty";
        private const string TargetDomainA = "TargetDomain";
        private const string TargetPropertyA = "TargetProperty";

        private const string ObjectNameDef = "DefaultObjectName";

        public void Generate() {
            // Create generators for the various output file types
            EntityFileGenerator entityFileGenerator = new(OutputRoot);
            BusinessObjectFileGenerator boFileGenerator = new(OutputRoot);
            ContextFileGenerator contextFileGenerator = new(OutputRoot);
            ConfigurationFileGenerator configurationFileGenerator = new(OutputRoot);
            RepositoryFileGenerator repositoryFileGenerator = new(OutputRoot);
            ServiceFileGenerator serviceFileGenerator = new(OutputRoot);

            // Create the output directories if they don't exist
            OutputRoot.Create();
            entityFileGenerator.CreateOutputDirectory();
            configurationFileGenerator.CreateOutputDirectory();            
            contextFileGenerator.CreateOutputDirectory();
            boFileGenerator.CreateOutputDirectory();
            repositoryFileGenerator.CreateOutputDirectory();
            serviceFileGenerator.CreateOutputDirectory();

            // Read the .kamlbo document
            XElement kamlRoot = XElement.Load(SourceKamlBo.FullName);

            // Load the KAML BusinessObjects
            var boElements = from e in kamlRoot.Descendants("BusinessObject") select e;
            List<KamlBoEntity> entities = new();
            foreach (var bo in boElements)
            {
                Console.WriteLine($"Found object {bo.Attribute("Name")?.Value}");
                KamlBoEntity entity = ParseKamlBoEntity(bo);
                entities.Add(entity);
            }
            Console.WriteLine();

            // Read the scope function for the context generator
            XElement scopeElement = (from e in kamlRoot.Descendants("Scopes") select e).First();
            if (scopeElement != null )
            {
                contextFileGenerator.ScopeFunction = scopeElement.Attribute("Function")?.Value;
                Console.WriteLine($"Found scope function {contextFileGenerator.ScopeFunction}");
            }

            // Determine the domain's root entity
            KamlBoEntity domainRoot = entities
                .Where(e => e.IsDomainPrimary)
                .Single();
            contextFileGenerator.DomainRoot = domainRoot;
            entityFileGenerator.DomainRoot = domainRoot;
            boFileGenerator.DomainRoot = domainRoot;
            configurationFileGenerator.DomainRoot = domainRoot;
            repositoryFileGenerator.DomainRoot = domainRoot;
            serviceFileGenerator.DomainRoot = domainRoot;

            // Create the context file
            contextFileGenerator.CreateContextFile(entities);
            Console.WriteLine();

            // Create the repository file
            repositoryFileGenerator.CreateRepositoryFile(domainRoot);
            Console.WriteLine();

            // Create the service files
            serviceFileGenerator.CreateServiceFile(domainRoot);
// TODO:            serviceFileGenerator.CreateServiceInterfaceFile(domainRoot);
            Console.WriteLine();

            // Create source files for the entities, business object POCOs and entity configurations
            entityFileGenerator.CreateDomainSubdirectory();
            boFileGenerator.CreateDomainSubdirectory();
            foreach (KamlBoEntity entity in entities)
            {
                entityFileGenerator.CreateEntityFile(entity);
                boFileGenerator.CreateBusinessObjectFile(entity);
                configurationFileGenerator.CreateConfigurationFile(entity);
                Console.WriteLine();
            }
        }

        private KamlBoEntity ParseKamlBoEntity(XElement boEl)
        {
            string name = boEl.Attribute("Name")?.Value ?? "?_?";
            string tableName = boEl.Element("Data")?.Attribute("Table")?.Value ?? "?_?";
            var entity = new KamlBoEntity(name, tableName);
            
            // Get all the properties
            var propertiesEl = boEl.Element("Properties");
            if (propertiesEl != null)
            {
                foreach (var propertyEl in propertiesEl.Elements())
                {
                    if (string.Equals(propertyEl.Name.LocalName, "BoundProperty"))
                    {
                        KamlEntityProperty prop = new()
                        {
                            Name = propertyEl.Attribute(NameA)?.Value,
                            Label = propertyEl.Attribute(LabelA)?.Value,
                            DataType = propertyEl.Attribute(TypeA)?.Value,
                            Length = propertyEl.Attribute(LengthA)?.Value.AsInteger() ?? 0,
                            IsKey = propertyEl.Attribute(IsKeyA)?.Value.AsBoolean() ?? false,
                            IsRequired = propertyEl.Attribute(IsRequiredA)?.Value.AsBoolean() ?? false,
                            IsIdentity = propertyEl.Attribute(IsIdentityA)?.Value.AsBoolean() ?? false,
                            Table = propertyEl.Attribute(TableA)?.Value ?? tableName,
                            Column = propertyEl.Attribute(ColumnA)?.Value
                        };
                        entity.Properties.Add(prop);
                    }
                }
            }
            
            // Get all the relationships
            var relationsEl = boEl.Element("Relations");
            if (relationsEl != null)
            {
                foreach (var relationEl in relationsEl.Elements())
                {
                    string relName = relationEl.Attribute(NameA)?.Value ?? "?Name?";
                    string? targetDomain = relationEl.Attribute(TargetDomainA)?.Value;
                    string target = relationEl.Attribute(TargetObjectA)?.Value ?? "?TargetObject?";
                    bool isToMany = string.Equals(relationEl.Attribute(TypeA)?.Value, "ToMany");
                    bool isToOne = string.Equals(relationEl.Attribute(TypeA)?.Value, "ToOne");
                    bool isToOwnerMany = string.Equals(relationEl.Attribute(TypeA)?.Value, "ToOwnerMany");
                    bool isToOwnerOne = string.Equals(relationEl.Attribute(TypeA)?.Value, "ToOwnerOne");
                    KamlEntityRelation relation = new KamlEntityRelation(relName, target)
                    {
                        IsToMany = isToMany,
                        IsToOne = isToOne,
                        IsToOwnerMany = isToOwnerMany,
                        IsToOwnerOne = isToOwnerOne,
                        TargetDomain = targetDomain
                    };
                    var keyMapEls = relationEl.Element("KeyMap");
                    if (keyMapEls != null)
                    {
                        foreach (var keyMapEl in keyMapEls.Elements())
                        {
                            relation.KeyMap.Add(keyMapEl.Attribute(SourcePropertyA)?.Value, keyMapEl.Attribute(TargetPropertyA)?.Value);
                        }
                    }
                    entity.Relations.Add(relation);
                }
            }
            return entity;
        }
    }
}