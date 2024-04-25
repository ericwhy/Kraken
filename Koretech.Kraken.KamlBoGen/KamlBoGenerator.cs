using Koretech.Kraken.KamlBoGen.FileGenerators;
using Koretech.Kraken.KamlBoGen.KamlBoModel;
using System.Xml.Linq;

namespace Koretech.Kraken.KamlBoGen
{
    public class KamlBoGen {
        public KamlBoGen(FileInfo source, DirectoryInfo outPath) {
            SourceKamlBo = source;
            OutputRoot = outPath;
        }

        public FileInfo SourceKamlBo { get; private set; }
        public DirectoryInfo OutputRoot { get; private set; }

        private const string ObjectNameDef = "DefaultObjectName";

        public void Generate() {
            // Create generators for the various output file types
            EntityFileGenerator entityFileGenerator = new(OutputRoot);
            BusinessObjectFileGenerator boFileGenerator = new(OutputRoot);
            ContextFileGenerator contextFileGenerator = new(OutputRoot);
            ConfigurationFileGenerator configurationFileGenerator = new(OutputRoot);
            RepositoryFileGenerator repositoryFileGenerator = new(OutputRoot);
            ServiceFileGenerator serviceFileGenerator = new(OutputRoot);

            List<FileGenerator> generators = new List<FileGenerator>()
            {
                entityFileGenerator,
                boFileGenerator,
                contextFileGenerator,
                configurationFileGenerator,
                repositoryFileGenerator,
                serviceFileGenerator
            };

            // Create the output directories if they don't exist
            OutputRoot.Create();
            foreach (FileGenerator generator in generators)
            {
                generator.CreateOutputDirectory();
            }

            // Read the .kamlbo document
            XElement kamlRoot = XElement.Load(SourceKamlBo.FullName);

            // Load the domain information
            var domainElement = (from e in kamlRoot.DescendantsAndSelf("DomainObject") select e).First();
            KamlBoDomain domain = KamlBoDomain.ParseFromXElement(domainElement);

            // Load the business object definitions
            var boElements = from e in kamlRoot.Descendants("BusinessObject") select e;
            List<KamlBoEntity> entities = new();
            foreach (var boElement in boElements)
            {
                Console.WriteLine($"Found object {boElement.Attribute("Name")?.Value}");
                KamlBoEntity entity = KamlBoEntity.ParseFromXElement(boElement, domain);
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

            // Validate the loaded model
            if (!domain.Validate())
            {
                throw new Exception("Model created from KAMLBO specification is not valid.  Sorry, that's all I can tell you right now.");
            }

            // Create subdirectories for each domain where needed
            entityFileGenerator.CreateDomainSubdirectory(domain);
            boFileGenerator.CreateDomainSubdirectory(domain);

            // Create source files for the entities, business object POCOs and entity configurations
            foreach (FileGenerator generator in generators)
            {
                generator.Generate(domain);
                Console.WriteLine();
            }
        }
    }
}