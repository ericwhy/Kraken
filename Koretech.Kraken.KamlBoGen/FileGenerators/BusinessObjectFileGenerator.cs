using Koretech.Kraken.Kaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Koretech.Kraken.KamlBoGen.FileGenerators
{
    internal class BusinessObjectFileGenerator
    {

        private const string businessObjectsPath = "BusinessObjects";

        private readonly DirectoryInfo outputRootDirectory;

        /// <summary>
        /// The entity that is at the root of the DDD domain.
        /// </summary>
        public KamlBoEntity DomainRoot { get; set; } = null!;

        public BusinessObjectFileGenerator(DirectoryInfo outputRootDirectory)
        {
            this.outputRootDirectory = outputRootDirectory ?? throw new ArgumentNullException(nameof(outputRootDirectory));
        }

        /// <summary>
        /// Creates the subdirectory for storing BO files if it doesn't already exist.
        /// </summary>
        public void CreateOutputDirectory()
        {
            outputRootDirectory.CreateSubdirectory(businessObjectsPath);
        }

        /// <summary>
        /// Creates the subdirectory for storing BO files in a specific domain if it doesn't already exist.
        /// </summary>
        public void CreateDomainSubdirectory()
        {
            outputRootDirectory.GetDirectories(businessObjectsPath).Single().CreateSubdirectory(DomainRoot.Name);
        }

        /// <summary>
        /// Generates a BO class from a KAML BO specification.
        /// </summary>
        /// <param name="entity">KAML BO specification</param>
        public void CreateBusinessObjectFile(KamlBoEntity entity)
        {
            _ = DomainRoot ?? throw new InvalidOperationException($"DomainRoot must be set before calling {nameof(CreateBusinessObjectFile)}");

            string entityName = entity.Name;
            string domainName = DomainRoot.Name;
            string entityFullPath = Path.Combine(outputRootDirectory.FullName, businessObjectsPath);
            string domainFullPath = Path.Combine(entityFullPath, domainName);
            string sourceFileName = Path.Combine(domainFullPath, $"{entityName}.cs");
            if (File.Exists(sourceFileName))
            {
                File.Delete(sourceFileName);
            }

            var writer = File.CreateText(sourceFileName);
            writer.WriteLine("//");
            writer.WriteLine("// Created by Kraken KAML BO Generator");
            writer.WriteLine("//");
            writer.WriteLine();
            writer.WriteLine("using System.Collections;");
            writer.WriteLine();
            writer.WriteLine($"namespace Koretech.Kraken.BusinessObjects.{domainName}");
            writer.WriteLine("{");
            writer.WriteLine($"\tpublic class {entityName}");
            writer.WriteLine("\t{");

            // Properties
            writer.WriteLine("\t\t#region Properties");
            writer.WriteLine();
            foreach (KamlEntityProperty property in entity.Properties)
            {
                string clrType = SqlType.GetClrTypeName(property.DataType);
                writer.WriteLine($"\t\tpublic {clrType} {property.Name} {{ get; set; }}");
                writer.WriteLine();
            }
            writer.WriteLine("\t\t#endregion Properties");
            writer.WriteLine();

            // Relationships
            writer.WriteLine("\t\t#region Relationships");
            writer.WriteLine();
            foreach (KamlEntityRelation relationship in entity.Relations)
            {
                if (relationship.IsToMany || relationship.IsToOwnerMany)
                {
                    writer.WriteLine($"\t\tpublic List<{relationship.TargetEntity}> {relationship.Name} = new();");
                }
                else if (relationship.IsToOne || relationship.IsToOwnerOne)
                {
                    writer.WriteLine($"\t\tpublic {relationship.TargetEntity} {relationship.Name};");
                }
                writer.WriteLine();
            }
            writer.WriteLine("\t\t#endregion Relationships");

            writer.WriteLine("\t\t}");
            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Flush();
            writer.Close();
            Console.WriteLine($"File {sourceFileName} generated.");
        }
    }
}
