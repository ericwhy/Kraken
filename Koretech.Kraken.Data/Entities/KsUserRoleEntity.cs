namespace Koretech.Kraken.Data.Entity
{
    public class KsUserRoleEntity {
        public string KsUserId {get;set;} = "";
        public string ResourceType {get;set;} = "";
        public string ResourceName {get;set;} = "";
        public int RoleNo {get;set;}
        public KsUserEntity User {get;set;} = new();
        // public KsRoleRuser Role {get;set;} = new();
    }
}