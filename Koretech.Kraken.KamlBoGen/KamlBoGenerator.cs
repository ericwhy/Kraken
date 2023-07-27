using System.Net;
using System.Xml.Linq;

namespace Koretech.Kraken.Kaml {
    public class KamlBoGen {
        public KamlBoGen(FileInfo source, DirectoryInfo outPath) {
            SourceKamlBo = source;
            OutputRoot = outPath;
        }

        public FileInfo SourceKamlBo { get; private set; }
        public DirectoryInfo OutputRoot { get; private set; }

        private const string entitiesPath = "Entities";
        private const string configurationsPath = "Configurations";
        private const string contextsPath = "Contexts";
        private const string businessObjectsPath = "BusinessObjects";

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

        // Mapping from SQL types to CLR types
        private readonly Dictionary<string, string> ClrTypes = new()
        {
            { BooleanType, "bool" },
            { BytesType, "byte[]" },
            { CharacterType, "string" },
            { DateType, "DateTime" },
            { DateTimeType, "DateTime" },
            { DecimalType, "decimal" },
            { DoubleType, "double" },
            { IntegerType, "int" },
            { MoneyType, "decimal" },
            { UniqueIdentifierType, "Guid" },
            { NCharacterType, "string" },
            { YesNoType, "char" }
        };

        private const string ObjectNameDef = "DefaultObjectName";

        public void Generate() {
            // First create the output directories if they don't exist
            OutputRoot.Create();
            OutputRoot.CreateSubdirectory(entitiesPath);
            OutputRoot.CreateSubdirectory(configurationsPath);
            OutputRoot.CreateSubdirectory(contextsPath);
            OutputRoot.CreateSubdirectory(businessObjectsPath);

            // Read the document
            XElement kamlRoot = XElement.Load(SourceKamlBo.FullName);

            // Get the BusinessObjects
            var boElements = from e in kamlRoot.Descendants("BusinessObject") select e;
            List<KamlBoEntity> entities = new();
            foreach (var bo in boElements) 
            {
                Console.WriteLine();
                Console.WriteLine($"Found object {bo.Attribute("Name")?.Value}");
                KamlBoEntity entity = ParseKamlBoEntity(bo);
                entities.Add(entity);
                CreateEntityFile(entity);
                CreateEntityConfigurationFile(entity);
            }
            Console.WriteLine();
            
            // Determine the domain's primary entity and generate the context
            KamlBoEntity primaryEntity = entities
                .Where(e => e.IsDomainPrimary)
                .Single();
            CreateContextFile(entities, primaryEntity);
            Console.WriteLine();
            
            // Create the business object POCOs
            OutputRoot.GetDirectories(businessObjectsPath).Single().CreateSubdirectory(primaryEntity.Name);
            foreach (KamlBoEntity entity in entities)
            {
                CreateBusinessObjectFile(entity, primaryEntity);
            }
        }

        private KamlBoEntity ParseKamlBoEntity(XElement boEl)
        {
            string name = boEl.Attribute("Name")?.Value ?? "?_?";
            string tableName = boEl.Element("Data")?.Attribute("Table")?.Value ?? "?_?";
            var entity = new KamlBoEntity(name, tableName);
            
            // Get all the properties
            var propertiesEl = boEl.Element("Properties");
            if (propertiesEl != null)
            {
                foreach (var propertyEl in propertiesEl.Elements())
                {
                    if (string.Equals(propertyEl.Name.LocalName, "BoundProperty"))
                    {
                        KamlEntityProperty prop = new()
                        {
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
            
            // Get all the relationships
            var relationsEl = boEl.Element("Relations");
            if (relationsEl != null)
            {
                foreach (var relationEl in relationsEl.Elements())
                {
                    string relName = relationEl.Attribute(NameA)?.Value ?? "?Name?";
                    string? targetDomain = relationEl.Attribute(TargetDomainA)?.Value;
                    string target = relationEl.Attribute(TargetObjectA)?.Value ?? "?TargetObject?";
                    bool isToMany = string.Equals(relationEl.Attribute(TypeA)?.Value, "ToMany");
                    bool isToOne = string.Equals(relationEl.Attribute(TypeA)?.Value, "ToOne");
                    bool isToOwnerMany = string.Equals(relationEl.Attribute(TypeA)?.Value, "ToOwnerMany");
                    bool isToOwnerOne = string.Equals(relationEl.Attribute(TypeA)?.Value, "ToOwnerOne");
                    KamlEntityRelation relation = new KamlEntityRelation(relName, target)
                    {
                        IsToMany = isToMany,
                        IsToOne = isToOne,
                        IsToOwnerMany = isToOwnerMany,
                        IsToOwnerOne = isToOwnerOne,
                        TargetDomain = targetDomain
                    };
                    var keyMapEls = relationEl.Element("KeyMap");
                    if (keyMapEls != null)
                    {
                        foreach (var keyMapEl in keyMapEls.Elements())
                        {
                            relation.KeyMap.Add(keyMapEl.Attribute(SourcePropertyA)?.Value, keyMapEl.Attribute(TargetPropertyA)?.Value);
                        }
                    }
                    entity.Relations.Add(relation);
                }
            }
            return entity;
        }

        #region Entity File

        private void CreateEntityFile(KamlBoEntity entity) {
            string objectName = entity.Name;
            string sourceFileName = Path.Combine(
                Path.Combine(OutputRoot.FullName, entitiesPath), $"{objectName}Entity.cs");
            if (File.Exists(sourceFileName))
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
            foreach (var property in entity.Properties) {
                if (string.Equals(property.DataType, CharacterType, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteStringProperty(property, writer);
                }
                else if (string.Equals(property.DataType, DateTimeType, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteDateTimeProperty(property, writer);
                }
                else if (string.Equals(property.DataType, YesNoType, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteStringProperty(property, writer);
                }
                else if (string.Equals(property.DataType, IntegerType, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteIntegerProperty(property, writer);
                }
                else if (string.Equals(property.DataType, UniqueIdentifierType, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteUuidProperty(property, writer);
                }
                else if (string.Equals(property.DataType, BytesType, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteBytesProperty(property, writer);
                }
                else
                {
                    Console.WriteLine($"{objectName} has property {property.Name} with unknown type {property.DataType}");
                }
            }

            // relations
            foreach (var rel in entity.Relations) {
                // Not supporting inter-domain relationships at this time
                if (string.IsNullOrEmpty(rel.TargetDomain)) {
                    if (rel.IsToMany) {
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
            Console.WriteLine($"File {sourceFileName} generated.");
        }

        private bool getNullable(XElement prop)
        {
            bool isRequired = false;
            var requiredAttribute = prop.Attribute("Required")?.Value;
            if (!string.IsNullOrEmpty(requiredAttribute))
            {
                bool.TryParse(requiredAttribute, out isRequired);
            }
            return !isRequired;
        }

        private string getElementSize(XElement prop)
        {
            string sizeValue = "0";
            var sizeAttribute = prop.Attribute("Length");
            if (sizeAttribute != null)
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

        private void WriteToManyRelationProperty(KamlEntityRelation relation, StreamWriter writer) {
            string targetEntityType = $"{relation.TargetEntity}Entity";
            writer.Write($"\t\tpublic IList<{targetEntityType}> {relation.Name} {{get; set;}}");
            writer.Write($" = new List<{targetEntityType}>();  // Navigation property to child {targetEntityType}");
            writer.WriteLine();
        }

        private void WriteToOneRelationProperty(KamlEntityRelation relation, StreamWriter writer) {
            string targetEntityType = $"{relation.TargetEntity}Entity";
            writer.Write($"\t\tpublic {targetEntityType} {relation.Name} {{get; set;}}");
            writer.Write($" = new();  // Navigation property to parent {targetEntityType}");
            writer.WriteLine();
        }

        #endregion Entity File

        #region Configuration File

        /// <summary>
        /// Generates an EF EntityTypeConfiguration class from a BO specification.
        /// </summary>
        /// <param name="entity">BO specification</param>
        private void CreateEntityConfigurationFile(KamlBoEntity entity) {
            string entityName = entity.Name;
            string tableName = entity.TableName;
            string sourceFileName = Path.Combine(
                Path.Combine(OutputRoot.FullName, configurationsPath), $"{entityName}EntityTypeConfiguration.cs");
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
                if (string.Equals(property.DataType, CharacterType, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteStringColumn(property, writer);
                }
                else if (string.Equals(property.DataType, DateTimeType, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteDateTimeColumn(property, writer);
                }
                else if (string.Equals(property.DataType, YesNoType, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteYesNoColumn(property, writer);
                }
                else if (string.Equals(property.DataType, IntegerType, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteIntegerColumn(property, writer);
                }
                else if (string.Equals(property.DataType, UniqueIdentifierType, StringComparison.CurrentCultureIgnoreCase))
                {
                    WriteUuidColumn(property, writer);
                }
                else if (string.Equals(property.DataType, BytesType, StringComparison.CurrentCultureIgnoreCase))
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

        private void WriteStringColumn (KamlEntityProperty property, StreamWriter writer)
        {
            writer.WriteLine($"\t\t\ttypeBuilder.Property(e => e.{property.Name})");
            writer.WriteLine($"\t\t\t\t.HasMaxLength({property.Length})");
            writer.WriteLine("\t\t\t\t.IsUnicode(false)");
            writer.WriteLine($"\t\t\t\t.HasColumnName({property.Column});");
            writer.WriteLine();
        }

        private void WriteDateTimeColumn (KamlEntityProperty property, StreamWriter writer)
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

        #endregion Configuration File

        #region Context File

        /// <summary>
        /// Generates an EF Context class from a set of kamlbo BO specifications.
        /// </summary>
        /// <param name="entities">All entity specifications from the kamlbo file.</param>
        /// <param name="primaryEntity">The primary entity in the business domain.</param>
        private void CreateContextFile(List<KamlBoEntity> entities, KamlBoEntity primaryEntity)
        {
            string primaryEntityName = primaryEntity.Name;
            IEnumerable<string> ownedEntityNames = entities
                .Where(e => !e.IsDomainPrimary)
                .Select(e => e.Name);

            string sourceFileName = Path.Combine(
                Path.Combine(OutputRoot.FullName, contextsPath), $"{primaryEntityName}Context.cs");
            if (File.Exists(sourceFileName))
            {
                File.Delete(sourceFileName);
            }

            var writer = File.CreateText(sourceFileName);
            writer.WriteLine("//");
            writer.WriteLine("// Created by Kraken KAML BO Generator");
            writer.WriteLine("//");
            writer.WriteLine();
            writer.WriteLine("using Koretech.Kraken.Data.Configurations;");
            writer.WriteLine("using Koretech.Kraken.Data.Entity;");
            writer.WriteLine("using Microsoft.EntityFrameworkCore;");
            writer.WriteLine();
            writer.WriteLine("namespace Koretech.Kraken.Data.Contexts");
            writer.WriteLine("{");
            writer.WriteLine($"\tpublic class {primaryEntityName}Context : DbContext");
            writer.WriteLine("\t{");
            writer.WriteLine($"\t\tpublic {primaryEntityName}Context() {{ }}");
            writer.WriteLine("\t\t");
            writer.WriteLine($"\t\tpublic {primaryEntityName}Context(DbContextOptions<{primaryEntityName}Context> options) : base(options) {{ }}");
            writer.WriteLine();
            
            writer.WriteLine($"\t\tpublic virtual DbSet<{primaryEntityName}Entity> {primaryEntityName}s {{ get; set; }}"); //TODO: Fix plural on property naming
            foreach(string objectName in ownedEntityNames)
            {
                writer.WriteLine($"\t\tpublic virtual DbSet<{objectName}Entity> {objectName}s {{ get; set; }}");
            }
            writer.WriteLine();

            writer.WriteLine("\t\tprotected override void OnModelCreating(ModelBuilder modelBuilder)");
            writer.WriteLine("\t\t{");
            writer.WriteLine($"\t\t\tnew {primaryEntityName}EntityTypeConfiguration().Configure(modelBuilder.Entity<{primaryEntityName}Entity>());");
            foreach (string objectName in ownedEntityNames)
            {
                writer.WriteLine($"\t\t\tnew {objectName}EntityTypeConfiguration().Configure(modelBuilder.Entity<{objectName}Entity>());");
            }
            writer.WriteLine("\t\t}");

            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Flush();
            writer.Close();
            Console.WriteLine($"File {sourceFileName} generated.");
        }

        #endregion Context File

        #region Business Object File

        /// <summary>
        /// Generates a business object class a BO specification.
        /// </summary>
        /// <param name="entity">BO specification</param>
        /// <param name="primaryEntity">The primary entity in the business domain.</param>
        private void CreateBusinessObjectFile(KamlBoEntity entity, KamlBoEntity primaryEntity)
        {
            string entityName = entity.Name;
            string primaryEntityName = primaryEntity.Name;
            string boFullPath = Path.Combine(OutputRoot.FullName, businessObjectsPath);
            string domainFullPath = Path.Combine(boFullPath, primaryEntityName);
            string sourceFileName = Path.Combine(domainFullPath, $"{entityName}.Generated.cs");
            if (File.Exists(sourceFileName))
            {
                File.Delete(sourceFileName);
            }

            var writer = File.CreateText(sourceFileName);
            writer.WriteLine("//");
            writer.WriteLine("// Created by Kraken KAML BO Generator");
            writer.WriteLine("//");
            writer.WriteLine();
            writer.WriteLine("using Koretech.Framework.BusinessObjects;");
            writer.WriteLine("using Koretech.KommerceServer.BusinessObjects.Utility;");
            writer.WriteLine("using System.ComponentModel;");
            writer.WriteLine("using System.Collections;");
            writer.WriteLine();
            writer.WriteLine($"namespace Koretech.Kraken.BusinessObjects.{primaryEntityName}");
            writer.WriteLine("{");
            writer.WriteLine($"\tpublic class {entityName}Base : BODomainEntity<{entityName}Base>, IDomainObject");
            writer.WriteLine("\t{");

            // Properties
            writer.WriteLine("\t\t#region Properties");
            writer.WriteLine();
            foreach (KamlEntityProperty property in entity.Properties)
            {
                string clrType = ClrTypes[property.DataType];
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

        #endregion Business Object File
    }
}