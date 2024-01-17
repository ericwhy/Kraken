using Koretech.Kraken.Kaml;

namespace Koretech.Kraken.KamlBoGen.FileGenerators
{
    internal class EntityFileGenerator : FileGenerator
    {
        private const string entitiesPath = "Entities";

        public EntityFileGenerator(DirectoryInfo outputRootDirectory) : base(outputRootDirectory)
        {
            generatePath = entitiesPath;            
        }

        /// <summary>
        /// Creates the subdirectory for storing entityBO files in a specific domain if it doesn't already exist.
        /// </summary>
        public void CreateDomainSubdirectory()
        {
            outputRootDirectory.GetDirectories(entitiesPath).Single().CreateSubdirectory(DomainRoot.Name);
        }

        /// <summary>
        /// Generates an entity class from a KAML BO specification.
        /// </summary>
        /// <param name="entity">KAML BO specification</param>
        public void CreateEntityFile(KamlBoEntity entity)
        {
            _ = DomainRoot ?? throw new InvalidOperationException($"DomainRoot must be set before calling {nameof(CreateEntityFile)}");

            string entityName = entity.Name;
            string domainName = DomainRoot.Name;
            string entityFullPath = Path.Combine(outputRootDirectory.FullName, entitiesPath);
            string domainFullPath = Path.Combine(entityFullPath, domainName);
            string sourceFileName = Path.Combine(domainFullPath, $"{entityName}Entity.cs");
            {
                File.Delete(sourceFileName);
            }
            var writer = File.CreateText(sourceFileName);
            writer.Write(GetFileHeader());
            writer.WriteLine();
            writer.WriteLine($"namespace Koretech.Infrastructure.Services.{domainName}.Entities");
            writer.WriteLine("{");
            writer.WriteLine($"\tpublic class {entityName}Entity");
            writer.WriteLine("\t{");
            writer.WriteLine("\t");

            // Properties
            foreach (var property in entity.Properties)
            {
                if (string.Equals(property.DataType, SqlType.Character.TypeName, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteStringProperty(property, writer);
                }
                else if (string.Equals(property.DataType, SqlType.DateTime.TypeName, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteDateTimeProperty(property, writer);
                }
                else if (string.Equals(property.DataType, SqlType.YesNo.TypeName, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteYesNoProperty(property, writer);
                }
                else if (string.Equals(property.DataType, SqlType.Integer.TypeName, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteIntegerProperty(property, writer);
                }
                else if (string.Equals(property.DataType, SqlType.UniqueIdentifier.TypeName, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteUuidProperty(property, writer);
                }
                else if (string.Equals(property.DataType, SqlType.Bytes.TypeName, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteBytesProperty(property, writer);
                }
                else if (string.Equals(property.DataType, SqlType.Byte.TypeName, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteByteProperty(property, writer);
                }
                else
                {
                    Console.WriteLine($"{entityName} has property {property.Name} with unknown type {property.DataType}");
                }
                writer.WriteLine();
            }

            // Relations
            foreach (var rel in entity.Relations)
            {
                if (rel.IsToMany || rel.IsToOwnerMany)
                {
                    WriteToManyRelationProperty(rel, writer);
                }
                else
                {
                    WriteToOneRelationProperty(rel, writer);
                }

                // Not supporting inter-domain relationships at this time
                if (!string.IsNullOrEmpty(rel.TargetDomain))
                {
                    writer.WriteLine("\t\t// This is an inter-domain relationship. Not fully implemented yet.");
                }
                writer.WriteLine();
            }

            writer.WriteLine("\t}");
            writer.WriteLine("}");
            writer.Flush();
            writer.Close();
            Console.WriteLine($"File {sourceFileName} generated.");
        }

        private void WriteStringProperty(KamlEntityProperty property, StreamWriter writer)
        {
            string nullableChar = property.IsRequired ? string.Empty : "?";
            writer.Write($"\t\tpublic string{nullableChar} {property.Name}");
            writer.Write(" {get; set;}");
            if (property.IsRequired)
            {
                writer.Write(" = string.Empty;");
            }
            writer.WriteLine();
        }

        private void WriteDateTimeProperty(KamlEntityProperty property, StreamWriter writer)
        {
            string nullableChar = property.IsRequired ? string.Empty : "?";
            writer.Write($"\t\tpublic DateTime{nullableChar} {property.Name}");
            writer.Write(" {get; set;}");
            if (property.IsRequired)
            {
                writer.Write(" = DateTime.Now;");
            }
            writer.WriteLine();
        }

        private void WriteIntegerProperty(KamlEntityProperty property, StreamWriter writer)
        {
            string nullableChar = property.IsRequired ? string.Empty : "?";
            writer.Write($"\t\tpublic int{nullableChar} {property.Name}");
            writer.Write(" {get; set;}");
            writer.WriteLine();
        }

        private void WriteUuidProperty(KamlEntityProperty property, StreamWriter writer)
        {
            string nullableChar = property.IsRequired ? string.Empty : "?";
            writer.Write($"\t\tpublic Guid{nullableChar} {property.Name}");
            writer.Write(" {get; set;}");
            writer.WriteLine();
        }

        private void WriteBytesProperty(KamlEntityProperty property, StreamWriter writer)
        {
            string nullableChar = property.IsRequired ? string.Empty : "?";
            writer.Write($"\t\tpublic byte[]{nullableChar} {property.Name}");
            writer.Write(" {get; set;}");
            if (property.IsRequired)
            {
                writer.Write($" = new byte[{property.Length}];");
            }
            writer.WriteLine();
        }

        private void WriteByteProperty(KamlEntityProperty property, StreamWriter writer)
        {
            string nullableChar = property.IsRequired ? string.Empty : "?";
            writer.Write($"\t\tpublic byte{nullableChar} {property.Name}");
            writer.Write(" {get; set;}");
            writer.WriteLine();
        }

        private void WriteYesNoProperty(KamlEntityProperty property, StreamWriter writer)
        {
            string nullableChar = property.IsRequired ? string.Empty : "?";
            writer.Write($"\t\tpublic char{nullableChar} {property.Name}");
            writer.Write(" {get; set;}");
            writer.WriteLine();
        }

        private void WriteToManyRelationProperty(KamlEntityRelation relation, StreamWriter writer)
        {
            string targetEntityType = $"{relation.TargetEntity}Entity";
            string relationType = (relation.IsToOwnerOne || relation.IsToOwnerMany) ? "owner" : "child";
            writer.Write($"\t\tpublic IList<{targetEntityType}> {relation.Name} {{get; set;}}");
            writer.Write($" = new List<{targetEntityType}>();  // Navigation property to {relationType} {targetEntityType}");
            writer.WriteLine();
        }

        private void WriteToOneRelationProperty(KamlEntityRelation relation, StreamWriter writer)
        {
            string targetEntityType = $"{relation.TargetEntity}Entity";
            string relationType = (relation.IsToOwnerOne || relation.IsToOwnerMany) ? "owner" : "child";
            writer.Write($"\t\tpublic {targetEntityType} {relation.Name} {{get; set;}}");
            writer.Write($"  // Navigation property to {relationType} {targetEntityType}");  //TODO: How to initialize?  Can't use new() due to recursion.
            writer.WriteLine();
        }
    }
}
