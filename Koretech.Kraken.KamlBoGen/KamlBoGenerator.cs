using Koretech.Kraken.KamlBoGen.FileGenerators;
using Koretech.Kraken.KamlBoModel.Model;

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

            var sharedModel = global::Koretech.Kraken.KamlBoModel.Model.KamlBoModel.ParseFromKamlBoFiles(SourceKamlBo.Directory!, new[] { SourceKamlBo });
            KamlBoDomain sharedDomain = sharedModel.Domains.Single();

            entityFileGenerator.CreateDomainSubdirectory(sharedDomain);

            foreach (FileGenerator generator in generators)
            {
                generator.Generate(sharedDomain);
                Console.WriteLine();
            }
        }
    }
}
