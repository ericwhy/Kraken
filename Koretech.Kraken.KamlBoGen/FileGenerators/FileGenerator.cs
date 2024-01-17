using Koretech.Kraken.Kaml;
using System.Text;

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
        /// The entity that is at the root of the DDD domain.
        /// </summary>
        public KamlBoEntity DomainRoot { get; set; } = null!;

        /// <summary>
        /// Creates the subdirectory for storing BO files if it doesn't already exist.
        /// </summary>
        /// <param name="path"></param>
        public virtual void CreateOutputDirectory()
        {
            if (!string.IsNullOrEmpty(generatePath))
            {
                outputRootDirectory.CreateSubdirectory(generatePath);
            }
        }

        /// <summary>
        /// Generates the header lines that appear at the top of every generated code file.
        /// </summary>
        /// <returns>the header as a string</returns>
        protected string GetFileHeader()
        {
            StringBuilder sb = new StringBuilder();

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
        /// <param name="entity">Representation of the entity from KAMLBO</param>
        /// <param name="includeTypes">If true, each parameter name will be preceded by its data type.</param>
        /// <returns>a comma-delimited string of parameters</returns>
        protected string GetPrimaryKeyAsParameters(KamlBoEntity entity, bool includeTypes)
        {
            string result = string.Empty;
            foreach (KamlEntityProperty property in entity.Properties)
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
