using Koretech.Kraken.Kaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Koretech.Kraken.KamlBoGen.FileGenerators
{
    internal class ConfigurationFileGenerator
    {
        private const string configurationsPath = "Configurations";

        private readonly DirectoryInfo outputRootDirectory;

        /// <summary>
        /// The entity that is at the root of the DDD domain.
        /// </summary>
        public KamlBoEntity DomainRoot { get; set; } = null!;

        public ConfigurationFileGenerator(DirectoryInfo outputRootDirectory)
        {
            this.outputRootDirectory = outputRootDirectory ?? throw new ArgumentNullException(nameof(outputRootDirectory));
        }

        /// <summary>
        /// Creates the subdirectory for storing configuration files if it doesn't already exist.
        /// </summary>
        public void CreateOutputDirectory()
        {
            outputRootDirectory.CreateSubdirectory(configurationsPath);
        }

        /// <summary>
        /// Creates the subdirectory for storing configuration files in a specific domain if it doesn't already exist.
        /// </summary>
        //public void CreateDomainSubdirectory() {
        //    outputRootDirectory.GetDirectories(configurationsPath).Single().CreateSubdirectory(DomainRoot.Name);
        //}

        /// <summary>
        /// Generates an EF entity type configuration class from a KAML BO specification.
        /// </summary>
        /// <param name="entity">KAML BO specification</param>
        public void CreateConfigurationFile(KamlBoEntity entity)
        {
            _ = DomainRoot ?? throw new InvalidOperationException($"DomainRoot must be set before calling {nameof(CreateConfigurationFile)}");

                string entityName = entity.Name;
                string tableName = entity.TableName;
                string sourceFileName = Path.Combine(
                    Path.Combine(outputRootDirectory.FullName, configurationsPath), $"{entityName}EntityTypeConfiguration.cs");
                if (File.Exists(sourceFileName))
                {
                    File.Delete(sourceFileName);
                }

                var writer = File.CreateText(sourceFileName);
                writer.WriteLine("//");
                writer.WriteLine("// Created by Kraken KAML BO Generator");
                writer.WriteLine("//");
                writer.WriteLine();
                writer.WriteLine("using Koretech.Kraken.Data;");
                writer.WriteLine("using Microsoft.EntityFrameworkCore;");
                writer.WriteLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
                writer.WriteLine("using System;");
                writer.WriteLine();
                writer.WriteLine("namespace Koretech.Kraken.Data.Configuration");
                writer.WriteLine("{");
                writer.WriteLine($"\tpublic class {entityName}EntityTypeConfiguration : IEntityTypeConfiguration<{entityName}Entity>");
                writer.WriteLine("\t{");
                writer.WriteLine($"\t\tpublic void Configure(EntityTypeBuilder<{entityName}Entity> typeBuilder)");
                writer.WriteLine("\t\t{");
                writer.WriteLine("\t\t\ttypeBuilder");
                writer.WriteLine($"\t\t\t\t.ToTable<{entityName}Entity>(\"{tableName}\", \"ks\");\r\n");
                writer.WriteLine($"\t\t\ttypeBuilder.HasKey(e => e.{entityName}Id)");
                writer.WriteLine($"\t\t\t\t.HasName(\"{tableName}_PK\");");
                writer.WriteLine();

                // Properties
                foreach (KamlEntityProperty property in entity.Properties)
                {
                    if (string.Equals(property.DataType, SqlType.Character.TypeName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        WriteStringColumn(property, writer);
                    }
                    else if (string.Equals(property.DataType, SqlType.DateTime.TypeName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        WriteDateTimeColumn(property, writer);
                    }
                    else if (string.Equals(property.DataType, SqlType.YesNo.TypeName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        WriteYesNoColumn(property, writer);
                    }
                    else if (string.Equals(property.DataType, SqlType.Integer.TypeName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        WriteIntegerColumn(property, writer);
                    }
                    else if (string.Equals(property.DataType, SqlType.UniqueIdentifier.TypeName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        WriteUuidColumn(property, writer);
                    }
                    else if (string.Equals(property.DataType, SqlType.Bytes.TypeName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        WriteBytesColumn(property, writer);
                    }
                    else
                    {
                        Console.WriteLine($"{entityName} has property {property.Name} with unknown type {property.DataType}");
                    }
                }

                writer.WriteLine("\t\t}");
                writer.WriteLine("\t}");
                writer.WriteLine("}");

                writer.Flush();
                writer.Close();
                Console.WriteLine($"File {sourceFileName} generated.");
            }

            private void WriteStringColumn(KamlEntityProperty property, StreamWriter writer)
            {
                writer.WriteLine($"\t\t\ttypeBuilder.Property(e => e.{property.Name})");
                writer.WriteLine($"\t\t\t\t.HasMaxLength({property.Length})");
                writer.WriteLine("\t\t\t\t.IsUnicode(false)");
                writer.WriteLine($"\t\t\t\t.HasColumnName({property.Column});");
                writer.WriteLine();
            }

            private void WriteDateTimeColumn(KamlEntityProperty property, StreamWriter writer)
            {
                writer.WriteLine($"\t\t\ttypeBuilder.Property(e => e.{property.Name})");
                writer.WriteLine($"\t\t\t\t.HasDefaultValueSql(\"(getdate())\")");
                writer.WriteLine($"\t\t\t\t.HasColumnType(\"datetime\")");
                writer.WriteLine($"\t\t\t\t.HasColumnName({property.Column});");
                writer.WriteLine();
            }

            private void WriteIntegerColumn(KamlEntityProperty property, StreamWriter writer)
            {
                writer.WriteLine($"\t\t\ttypeBuilder.Property(e => e.{property.Name})");
                writer.WriteLine($"\t\t\t\t.HasColumnName({property.Column});");
                writer.WriteLine();
            }

            private void WriteUuidColumn(KamlEntityProperty property, StreamWriter writer)
            {
                writer.WriteLine($"\t\t\ttypeBuilder.Property(e => e.{property.Name})");
                writer.WriteLine($"\t\t\t\t.HasColumnType(\"uniqueidentifier\")");
                writer.WriteLine($"\t\t\t\t.HasColumnName({property.Column});");
                writer.WriteLine();
            }

            private void WriteBytesColumn(KamlEntityProperty property, StreamWriter writer)
            {
                writer.WriteLine($"\t\t\ttypeBuilder.Property(e => e.{property.Name})");
                writer.WriteLine($"\t\t\t\t.HasColumnType(\"varbinary\")");
                writer.WriteLine($"\t\t\t\t.HasMaxLength({property.Length})");
                writer.WriteLine($"\t\t\t\t.HasColumnName({property.Column});");
                writer.WriteLine();
            }

            private void WriteYesNoColumn(KamlEntityProperty property, StreamWriter writer)
            {
                writer.WriteLine($"\t\t\ttypeBuilder.Property(e => e.{property.Name})");
                writer.WriteLine($"\t\t\t\t.HasMaxLength(1)");
                writer.WriteLine("\t\t\t\t.IsUnicode(false)");
                writer.WriteLine($"\t\t\t\t.HasDefaultValueSql(\"('N')\")");
                writer.WriteLine($"\t\t\t\t.IsFixedLength()");
                writer.WriteLine($"\t\t\t\t.HasColumnName({property.Column});");
                writer.WriteLine();
            }
        }
    }
