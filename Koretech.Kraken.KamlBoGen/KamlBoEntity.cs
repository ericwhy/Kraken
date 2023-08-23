namespace Koretech.Kraken.Kaml
{

    /// <summary>
    /// Represents a BO configuration taken from a kamlbo file.
    /// </summary>
    public class KamlBoEntity
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public List<KamlEntityProperty> Properties { get; }
        public List<KamlEntityRelation> Relations { get; }

        /// <summary>
        /// Indicates whether the entity is the root of the dependencies in the entity's domain,
        /// i.e. is it an owner of other types without being owned by any other types.
        /// </summary>
        public bool IsDomainPrimary
        {
            get
            {
                IEnumerable<KamlEntityRelation> ownerRelations = Relations.Where(r => r.IsToOwner);
                return (ownerRelations.Count() == 0);
            }
        }

        public KamlBoEntity(string eName, string tableName)
        {
            Properties = new();
            Relations = new();
            Name = eName;
            TableName = tableName;
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

    /// <summary>
    /// Represents a BO relationship configuration taken from a kamlbo file.
    /// </summary>
    public class KamlEntityRelation
    {
        public string Name { get; set; }
        public string TargetEntity { get; set; }
        public string? TargetDomain { get; set; }
        public Dictionary<string, string> KeyMap { get; private set; }
        public bool IsToMany { get; set; }
        public bool IsToOne { get; set; }
        public bool IsToOwnerMany { get; set; }
        public bool IsToOwnerOne { get; set; }

        /// <summary>
        /// Indicates whether this is a relationship from an owned type to its owning type.
        /// </summary>
        public bool IsToOwner
        {
            get
            {
                return IsToOwnerMany || IsToOwnerOne;
            }
        }

        public KamlEntityRelation(string myName, string myTargetEntity)
        {
            Name = myName;
            TargetEntity = myTargetEntity;
            KeyMap = new Dictionary<string, string>();
            IsToMany = false;
            IsToOne = false;
            IsToOwnerMany = false;
            IsToOwnerOne = false;
        }
    }
}