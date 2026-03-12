using SharedModel = Koretech.Tools.KamlBoModel.Model;

namespace Koretech.Tools.KrakenGenerator.FileGenerators
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
