using System.Text;
using SharedModel = Koretech.Tools.KamlBoModel.Model;

namespace Koretech.Kraken.KamlBoGen.FileGenerators
{
    internal abstract class FileGenerator
    {
        protected string generatePath = string.Empty;
        protected readonly DirectoryInfo outputRootDirectory;

        public FileGenerator(DirectoryInfo outputRootDirectory)
        {
            this.outputRootDirectory = outputRootDirectory ?? throw new ArgumentNullException(nameof(outputRootDirectory));
        }

        /// <summary>
        /// Creates the subdirectory for storing BO files if it doesn't already exist.
        /// </summary>
        public virtual void CreateOutputDirectory()
        {
            if (!string.IsNullOrEmpty(generatePath))
            {
                outputRootDirectory.CreateSubdirectory(generatePath);
            }
        }

        /// <summary>
        /// Here is where we generate the output file.
        /// </summary>
        public void Generate(SharedModel.KamlBoDomain domain)
        {
            DoGenerate(domain);
        }

        /// <summary>
        /// A place for concrete implementation for generating the output file.
        /// </summary>
        protected abstract void DoGenerate(SharedModel.KamlBoDomain domain);

        /// <summary>
        /// Generates the header lines that appear at the top of every generated code file.
        /// </summary>
        protected string GetFileHeader()
        {
            StringBuilder sb = new();

            sb.AppendLine("/********************************************************/");
            sb.AppendLine("/*                                                      */");
            sb.AppendLine("/* Created by Kraken KAML BO Generator                  */");
            sb.AppendLine("/*                                                      */");
            sb.AppendLine("/* DO NOT MODIFY                                        */");
            sb.AppendLine("/*                                                      */");
            sb.AppendLine("/* Extensions or overrides should be placed in a        */");
            sb.AppendLine("/* subclass or partial class, whichever is appropriate. */");
            sb.AppendLine("/*                                                      */");
            sb.AppendLine("/********************************************************/");

            return sb.ToString();
        }

        /// <summary>
        /// Gets a string containing a method signature for passing the primary key(s) of the given entity.
        /// </summary>
        protected string GetPrimaryKeyAsParameters(SharedModel.KamlBoEntity entity, bool includeTypes)
        {
            string result = string.Empty;
            foreach (SharedModel.KamlBoEntityProperty property in entity.Properties)
            {
                if (property.IsKey)
                {
                    string clrType = SqlType.GetClrTypeName(property.DataType);
                    if (!string.IsNullOrEmpty(result))
                    {
                        result += ", ";
                    }
                    if (includeTypes)
                    {
                        result += $"{clrType} ";
                    }

                    result += property.Name;
                }
            }

            return result;
        }

    }
}
