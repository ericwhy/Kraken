using SharedModel = Koretech.Kraken.KamlBoModel.Model;

namespace Koretech.Kraken.KamlBoGen.FileGenerators
{
    internal class BusinessObjectFileGenerator : FileGenerator
    {
        private const string BusinessObjectsPath = "BusinessObjects";

        public BusinessObjectFileGenerator(DirectoryInfo outputRootDirectory) : base(outputRootDirectory)
        {
            generatePath = BusinessObjectsPath;
        }

        protected override void DoGenerate(SharedModel.KamlBoDomain domain)
        {
            throw new InvalidOperationException("Business object generation is currently deactivated.");
        }
    }
}
