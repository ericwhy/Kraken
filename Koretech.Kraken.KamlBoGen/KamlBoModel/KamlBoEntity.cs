using System.Reflection.Emit;
using System.Security.Principal;
using System.Xml.Linq;
using System;
using Koretech.Kraken.Kaml;

namespace Koretech.Kraken.KamlBoGen.KamlBoModel
{

    /// <summary>
    /// Represents a BO configuration taken from a kamlbo file.
    /// </summary>
    public class KamlBoEntity
    {
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

        public string Name { get; set; }
        public string TableName { get; set; }
        public List<KamlEntityProperty> Properties { get; }
        public List<KamlEntityRelation> Relations { get; }
        public KamlBoDomain Domain { get; set; }

        public bool HasCrossDomainRelationship => Relations.Where(r => r.IsCrossDomain).Any();

        /// <summary>
        /// Indicates whether the entity is the root of the dependencies in the entity's domain,
        /// i.e. is it an owner of other types without being owned by any other types.
        /// </summary>
        public bool IsDomainPrimary => Domain.PrimaryEntity == this;
  
        public KamlBoEntity(string eName, string tableName, KamlBoDomain domain)
        {
            Properties = new();
            Relations = new();
            Name = eName;
            TableName = tableName;
            Domain = domain;
        }

        public bool Validate()
        {
            IEnumerable<KamlEntityRelation> ownerRelations = Relations.Where(r => r.IsToOwner);
            if (IsDomainPrimary && ownerRelations.Count() > 0)
            {
                return false;
            }
            return true;
        }

        static public KamlBoEntity ParseFromXElement(XElement boEl, KamlBoDomain domain)
        {
            string name = boEl.Attribute("Name")?.Value ?? throw new Exception("Unable to get the value of attribute 'Name' from BusinessObject element");
            XElement? dataEl = boEl.Elements().FirstOrDefault(e => e.Name.LocalName.Equals("Data"));
            string tableName = dataEl?.Attribute("Table")?.Value ?? throw new Exception($"Unable to get the value of attribute 'Table' from Data element in BO '{name}'"); ;
            var entity = new KamlBoEntity(name, tableName, domain);
            domain.AddBoEntity(entity); // Link the entity to its domain.

            // Get all the properties
            var propertiesEl = boEl.Elements().Where(e => e.Name.LocalName.Equals("Properties"));
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
                            string? sourceProperty = keyMapEl.Attribute(SourcePropertyA)?.Value;
                            if (sourceProperty != null)
                            {
                                relation.KeyMap.Add(sourceProperty, keyMapEl.Attribute(TargetPropertyA)?.Value);
                            }
                        }
                    }
                    entity.Relations.Add(relation);
                }
            }
            return entity;
        }
    }

    /// <summary>
    /// Represents a BO property configuration taken from a kamlbo file.
    /// </summary>
    public class KamlEntityProperty
    {
        public string? Name { get; set; }
        public string? Label { get; set; }
        public string? DataType { get; set; }
        public int Length { get; set; }
        public bool IsKey { get; set; }
        public bool IsRequired { get; set; }
        public bool IsIdentity { get; set; }
        public string? Table { get; set; }
        public string? Column { get; set; }
    }

}