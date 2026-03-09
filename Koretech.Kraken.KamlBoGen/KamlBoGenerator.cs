using Koretech.Kraken.KamlBoGen.FileGenerators;
using Koretech.Kraken.KamlBoModel.Model;
using Koretech.Kraken.KamlBoModel.Utility;

namespace Koretech.Kraken.KamlBoGen
{
    public class KamlBoGen
    {
        public KamlBoGen(FileInfo source, DirectoryInfo outPath)
        {
            SourceKamlBo = source;
            OutputRoot = outPath;
        }

        public FileInfo SourceKamlBo { get; private set; }
        public DirectoryInfo OutputRoot { get; private set; }

        public void Generate()
        {
            EntityFileGenerator entityFileGenerator = new(OutputRoot);
            List<FileGenerator> generators =
            [
                entityFileGenerator,
                new ContextFileGenerator(OutputRoot),
                new ConfigurationFileGenerator(OutputRoot),
                new RepositoryFileGenerator(OutputRoot),
                new ServiceFileGenerator(OutputRoot)
            ];

            OutputRoot.Create();
            foreach (FileGenerator generator in generators)
            {
                generator.CreateOutputDirectory();
            }

            IReadOnlyList<KamlBoDependencyResolver.KamlBoReference> rootBusinessObjects =
                KamlBoDependencyResolver.GetBusinessObjectsDefinedInKamlBo(SourceKamlBo);
            string domainName = rootBusinessObjects
                .Select(reference => reference.Domain)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Single();

            List<FileInfo> kamlBoFiles = KamlBoDependencyResolver.GetKamlBoFilesForBusinessObjects(
                SourceKamlBo.Directory!,
                rootBusinessObjects,
                out List<KamlBoDependencyResolver.KamlBoReference> missingBusinessObjects);

            if (missingBusinessObjects.Count > 0)
            {
                string missingNames = string.Join(", ", missingBusinessObjects);
                throw new InvalidOperationException(
                    $"Unable to load required KAMLBO dependencies for '{SourceKamlBo.Name}'. Missing business objects: {missingNames}.");
            }

            var sharedModel = global::Koretech.Kraken.KamlBoModel.Model.KamlBoModel.ParseFromKamlBoFiles(SourceKamlBo.Directory!, kamlBoFiles);
            sharedModel.ExpandCompositeTypes();
            KamlBoDomain sharedDomain = sharedModel.GetDomain(domainName)
                ?? throw new InvalidOperationException(
                    $"Could not find domain '{domainName}' after loading KAMLBO metadata for '{SourceKamlBo.Name}'.");

            entityFileGenerator.CreateDomainSubdirectory(sharedDomain);

            foreach (FileGenerator generator in generators)
            {
                generator.Generate(sharedDomain);
                Console.WriteLine();
            }
        }
    }
}
