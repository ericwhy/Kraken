using System.Xml.Linq;

namespace Koretech.Kraken.Kaml {
    public class KamlBoGen {
        public KamlBoGen(FileInfo source, DirectoryInfo outPath) {
            SourceKamlBo = source;
            OutputRoot = outPath;
         }
        public FileInfo SourceKamlBo {get;set;}
        public DirectoryInfo OutputRoot {get;set;}
        private List<KamlBoEntity> entities = new();

        private const string entitiesPath = "Entities";
        private const string configurationsPath = "Configurations";
        private const string contextsPath = "Contexts";

        private const string NameA = "Name";
        private const string LabelA = "Label";
        private const string TypeA = "Type";
        private const string LengthA = "Length";
        private const string IsRequiredA = "IsRequired";
        private const string IsKeyA = "IsKey";
        private const string IsIdentityA = "IsIdentity";
        private const string IsFixedLengthA = "IsFixedLength";
        private const string TableA = "Table";
        private const string ColumnA = "Column";
        private const string TargetObjectA = "TargetObject";
        private const string SourcePropertyA = "SourceProperty";
        private const string TargetDomainA = "TargetDomain";
        private const string TargetPropertyA = "TargetProperty";


        private const string BooleanType = "boolean";
        private const string BytesType = "bytes";
        private const string CharacterType = "character";
        private const string DateType = "date";
        private const string DateTimeType = "datetime";
        private const string DecimalType = "decimal";
        private const string DoubleType = "double";
        private const string IntegerType = "integer";
        private const string MoneyType = "money";
        private const string UniqueIdentifierType = "uniqueidentifier";
        private const string NCharacterType = "widecharacter";
        private const string YesNoType = "yesno";

        private const string ObjectNameDef = "DefaultObjectName";

        public void Generate() {
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
                CreateEntityFile(entity);
                //CreateConfigurationFile(entity);
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
                                Name = propertyEl.Attribute(NameA)?.Value,
                                Label = propertyEl.Attribute(LabelA)?.Value,
                                DataType = propertyEl.Attribute(TypeA)?.Value,
                                Length = propertyEl.Attribute(LengthA)?.Value.AsInteger() ?? 0,
                                IsKey = propertyEl.Attribute(IsKeyA)?.Value.AsBoolean() ?? false,
                                IsRequired = propertyEl.Attribute(IsRequiredA)?.Value.AsBoolean() ?? false,
                                IsIdentity = propertyEl.Attribute(IsIdentityA)?.Value.AsBoolean() ?? false,
                                Table = propertyEl.Attribute(TableA)?.Value ?? tableName,
                                Column = propertyEl.Attribute(ColumnA)?.Value
                            };
                            entity.Properties.Add(prop);
                        }
                    }
                }
                var relationsEl = boEl.Element("Relations");
                if(relationsEl != null) {
                    foreach(var relationEl in relationsEl.Elements()) {
                        string relName = relationEl.Attribute(NameA)?.Value ?? "?Name?";
                        string? targetDomain = relationEl.Attribute(TargetDomainA)?.Value;
                        string target = relationEl.Attribute(TargetObjectA)?.Value ?? "?TargetObject?";
                        bool isToMany = string.Equals(relationEl.Attribute(TypeA)?.Value, "ToMany");
                        KamlEntityRelation relation = new KamlEntityRelation(relName, target);
                        relation.IsToMany = isToMany;
                        relation.TargetDomain = targetDomain;
                        var keyMapEls = relationEl.Element("KeyMap");
                        if(keyMapEls != null) {
                            foreach(var keyMapEl in keyMapEls.Elements()) {
                                relation.KeyMap.Add(keyMapEl.Attribute(SourcePropertyA)?.Value, keyMapEl.Attribute(TargetPropertyA)?.Value);
                            }
                        }
                        entity.Relations.Add(relation);
                    }
                }
                return entity;
            }
        }

        public void CreateEntityFile(KamlBoEntity entity) {
            string objectName = entity.Name;
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
            foreach(var property in entity.Properties) {
                if(string.Equals(property.DataType, CharacterType, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteStringProperty(property, writer);
                }
                else if(string.Equals(property.DataType, DateTimeType, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteDateTimeProperty(property, writer);
                }
                else if(string.Equals(property.DataType, YesNoType, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteStringProperty(property, writer);
                }
                else if(string.Equals(property.DataType, IntegerType, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteIntegerProperty(property, writer);
                }
                    else if(string.Equals(property.DataType, UniqueIdentifierType, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteUuidProperty(property, writer);
                }
                else if(string.Equals(property.DataType, BytesType, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteBytesProperty(property, writer);
                }
                else
                {
                    Console.WriteLine($"{objectName} has property {property.Name} with unknown type {property.DataType}");
                }
            }

            // relations
            foreach(var rel in entity.Relations) {
                // Not supporting inter-domain relationships at this time
                if(string.IsNullOrEmpty(rel.TargetDomain)) {
                    if(rel.IsToMany) {
                        WriteToManyRelationProperty(rel, writer);
                    } else {
                        WriteToOneRelationProperty(rel, writer);
                    }
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

        private void WriteStringProperty(KamlEntityProperty property, StreamWriter writer)
        {
            string nullableChar = property.IsRequired ? string.Empty : "?";
            writer.Write($"\t\tpublic string{nullableChar} {property.Name}");
            writer.Write(" {get; set;}");
            if(property.IsRequired)
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
            if(property.IsRequired)
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
            if(property.IsRequired)
            {
                writer.Write($" = new byte[{property.Length}];");
            }   
            writer.WriteLine();
        }

        private void WriteToManyRelationProperty(KamlEntityRelation relation , StreamWriter writer) {
            string targetEntityType = $"{relation.TargetEntity}Entity";
            writer.Write($"\t\tpublic IList<{targetEntityType}> {relation.Name} {{get; set;}}");
            writer.Write($" = new List<{targetEntityType}>();  // Navigation property to child {targetEntityType}");
            writer.WriteLine();
        }

        private void WriteToOneRelationProperty(KamlEntityRelation relation , StreamWriter writer) {
            string targetEntityType = $"{relation.TargetEntity}Entity";
            writer.Write($"\t\tpublic {targetEntityType} {relation.Name} {{get; set;}}");
            writer.Write($" = new();  // Navigation property to parent {targetEntityType}");
            writer.WriteLine();
        }

        private void CreateEntityConfigurationFile(KamlBoEntity entity) {
            string objectName = entity.Name;
            string sourceFileName = Path.Combine(
                Path.Combine(OutputRoot.FullName, configurationsPath), $"{objectName}EntityTypeConfiguration.cs");
            if(File.Exists(sourceFileName)) 
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
            writer.WriteLine($"\tpublic class {objectName}Entity");
            writer.WriteLine("\t{");
            writer.WriteLine("\t");
        }


    }
}