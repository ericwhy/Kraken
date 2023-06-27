using System.Xml.Linq;

namespace Koretech.Kraken.Kaml {
    public class KamlBoGen {
        public KamlBoGen(FileInfo source, DirectoryInfo outPath) 
        {
            SourceKamlBo = source;
            OutputRoot = outPath;
         }
        public FileInfo SourceKamlBo {get;set;}
        public DirectoryInfo OutputRoot {get;set;}
        private List<KamlBoEntity> entities = new();

        private const string entitiesPath = "Entities";
        private const string configurationsPath = "Configurations";
        private const string contextsPath = "Contexts";

        private const string NameAt = "Name";

        private const string StringType = "string";
        private const string DateTimeTime = "datetime";
        private const string YesNoType = "yesno";
        private const string IntegerType = "integer";
        private const string UniqueIdentifierType = "uniqueidentifier";
        private const string BytesType = "bytes";

        private const string ObjectNameDef = "DefaultObjectName";

        public void Generate()
        {
            // First create the output directories if they don't exist
            OutputRoot.Create();
            OutputRoot.CreateSubdirectory(entitiesPath);
            OutputRoot.CreateSubdirectory(configurationsPath);
            OutputRoot.CreateSubdirectory(contextsPath);
 
           // Read the document
            XElement kamlRoot = XElement.Load(SourceKamlBo.FullName);

            // Get the BusinessObjects
            var boElements = from e in kamlRoot.Descendants("BusinessObject") select e;
            foreach(var bo in boElements) {
                Console.WriteLine($"Found object {bo.Attribute("Name")?.Value}");
                KamlBoEntity entity = ParseKamlBoEntity(bo);
                CreateEntityFile(bo);
            }

            KamlBoEntity ParseKamlBoEntity(XElement boEl) {
                string name = boEl.Attribute("Name")?.Value ?? "?_?";
                string tableName = boEl.Element("Data")?.Attribute("Table")?.Value ?? "?_?";
                var entity = new KamlBoEntity(name, tableName);
                // Get all the properties
                var propertiesEl = boEl.Element("Properties");
                if(propertiesEl != null) {
                    foreach(var propertyEl in propertiesEl.Elements()) 
                    {
                        if(string.Equals(propertyEl.Name.LocalName, "BoundProperty"))
                        {
                            KamlEntityProperty prop = new() {
                                Label = propertyEl.Attribute("Label")?.Value,
                                DataType = propertyEl.Attribute("Type")?.Value,
                                Length = propertyEl.Attribute("Length")?.Value.AsInteger() ?? 0,
                                IsKey = propertyEl.Attribute("IsKey")?.Value.AsBoolean() ?? false
                            }
                            prop.Label = propertyEl.Attribute("Label")?.Value;
                            prop.DataType = propertyEl.Attribute("Type")?.Value;
                        }
                    }
                }


                return entity;
            }
        }

        public void CreateEntityFile(XElement businessObjectEl) 
        {
            string objectName = businessObjectEl.Attribute(NameAt)?.Value ?? ObjectNameDef;
            string sourceFileName = Path.Combine(
                Path.Combine(OutputRoot.FullName, entitiesPath), $"{objectName}Entity.cs");
            if(File.Exists(sourceFileName)) 
            {
                File.Delete(sourceFileName);
            }
            var writer = File.CreateText(sourceFileName);
            writer.WriteLine("//");
            writer.WriteLine("// Created by Kraken KAML BO Generator");
            writer.WriteLine("//");
            writer.WriteLine("namespace Koretech.Kraken.Data");
            writer.WriteLine("{");
            writer.WriteLine($"\tpublic class {objectName}Entity");
            writer.WriteLine("\t{");
            writer.WriteLine("\t");
            // Properties
            var propertiesEl = businessObjectEl.Element("Properties");
            if(propertiesEl != null)
            {
                foreach(var propertyEl in propertiesEl.Elements()) 
                {
                    if(string.Equals(propertyEl.Name.LocalName, "BoundProperty"))
                    {
                        string? elType = propertyEl.Attribute("Type")?.Value;
                        if(string.Equals(elType, "character", StringComparison.CurrentCultureIgnoreCase))
                        {
                            WriteStringProperty(propertyEl, writer);
                        }
                        else if(string.Equals(elType, "datetime", StringComparison.CurrentCultureIgnoreCase))
                        {
                            WriteDateTimeProperty(propertyEl, writer);
                        }
                        else if(string.Equals(elType, YesNoType, StringComparison.CurrentCultureIgnoreCase))
                        {
                            WriteStringProperty(propertyEl, writer);
                        }
                        else if(string.Equals(elType, IntegerType, StringComparison.CurrentCultureIgnoreCase))
                        {
                            WriteIntegerProperty(propertyEl, writer);
                        }
                        else if(string.Equals(elType, UniqueIdentifierType, StringComparison.CurrentCultureIgnoreCase))
                        {
                            WriteUuidProperty(propertyEl, writer);
                        }
                        else if(string.Equals(elType, BytesType, StringComparison.CurrentCultureIgnoreCase))
                        {
                            WriteBytesProperty(propertyEl, writer);
                        }
                        else
                        {
                            Console.WriteLine($"{objectName} has property {propertyEl.Attribute(NameAt)} with unknown type {elType}");
                        }
                    }
                }
            }
            // Relations
            var relationsEl = businessObjectEl.Element("Relations");
            if(relationsEl != null) 
            {
                foreach(var relationEl in relationsEl.Elements())
                {
                    
                }
            }
            

            writer.WriteLine("\t}");
            writer.WriteLine("}");
            writer.Flush();
            writer.Close();
        }

        private bool getNullable(XElement prop) 
        {
            bool isRequired = false;
            var requiredAttribute = prop.Attribute("Required")?.Value;
            if(!string.IsNullOrEmpty(requiredAttribute)) 
            {
                bool.TryParse(requiredAttribute, out isRequired);
            }
            return !isRequired;
        }

        private string getElementSize(XElement prop)
        {
            string sizeValue = "0";
            var sizeAttribute = prop.Attribute("Length");
            if(sizeAttribute != null) 
            {
                sizeValue = (string)sizeAttribute;
            }
            return sizeValue;
        }

        private void WriteStringProperty(XElement propertyEl, StreamWriter writer)
        {
            bool isNullable = getNullable(propertyEl);
            string nullableChar = isNullable ? "?" : string.Empty;
            writer.Write($"\t\tpublic string{nullableChar} {propertyEl.Attribute(NameAt)?.Value}");
            writer.Write(" {get; set;}");
            if(!isNullable)
            {
                writer.Write(" = string.Empty;");
            }
            writer.WriteLine();
        }

        private void WriteDateTimeProperty(XElement propertyEl, StreamWriter writer)
        {
            bool isNullable = getNullable(propertyEl);
            string nullableChar = isNullable ? "?" : string.Empty;
            writer.Write($"\t\tpublic DateTime{nullableChar} {propertyEl.Attribute(NameAt)?.Value}");
            writer.Write(" {get; set;}");
            if(!isNullable)
            {
                writer.Write(" = DateTime.Now;");
            }
            writer.WriteLine();
        }

        private void WriteIntegerProperty(XElement propertyEl, StreamWriter writer)
        {
            bool isNullable = getNullable(propertyEl);
            string nullableChar = isNullable ? "?" : string.Empty;
            writer.Write($"\t\tpublic int{nullableChar} {propertyEl.Attribute(NameAt)?.Value}");
            writer.Write(" {get; set;}");
            writer.WriteLine();
        }

        private void WriteUuidProperty(XElement propertyEl, StreamWriter writer)
        {
            bool isNullable = getNullable(propertyEl);
            string nullableChar = isNullable ? "?" : string.Empty;
            writer.Write($"\t\tpublic Guid{nullableChar} {propertyEl.Attribute(NameAt)?.Value}");
            writer.Write(" {get; set;}");
            writer.WriteLine();
        }

        private void WriteBytesProperty(XElement propertyEl, StreamWriter writer)
        {
            bool isNullable = getNullable(propertyEl);
            string nullableChar = isNullable ? "?" : string.Empty;
            writer.Write($"\t\tpublic byte[]{nullableChar} {propertyEl.Attribute(NameAt)?.Value}");
            writer.Write(" {get; set;}");
            if(!isNullable)
            {
                writer.Write($" = new byte[{getElementSize(propertyEl)}];");
            }   
            writer.WriteLine();
        }

    }
}