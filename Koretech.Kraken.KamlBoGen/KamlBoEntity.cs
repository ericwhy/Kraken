namespace Koretech.Kraken.Kaml {
    public class KamlBoEntity {
        public string Name {get;set;}
        public string TableName {get;set;}
        public List<KamlEntityProperty> Properties {get;}
        public List<KamlEntityRelation> Relations {get;}

        public KamlBoEntity(string eName, string tableName)
        {
            Properties = new();
            Relations = new();
            Name = eName;
            TableName = tableName;
        }
    }

    public class KamlEntityProperty {
        public string? Name {get;set;}
        public string? Label {get;set;}
        public string? DataType {get;set;}
        public int Length {get;set;}
        public bool IsKey {get;set;}
        public bool IsRequired {get;set;}
        public bool IsIdentity {get;set;}
        public string? Table {get;set;}
        public string? Column {get;set;}
    }

    public class KamlEntityRelation {
        public string Name {get; set;}
        public string TargetEntity {get;set;}
        public string? TargetDomain {get;set;}
        public Dictionary<string, string> KeyMap {get; private set;}
        public bool IsToMany {get;set;}

        public KamlEntityRelation(string myName, string myTargetEntity) {
            Name = myName;
            TargetEntity = myTargetEntity;
            KeyMap = new Dictionary<string, string>();
            IsToMany = false;
        }
    }
}