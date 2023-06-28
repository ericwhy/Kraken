namespace Koretech.Kraken.Data.Entity
{
    public class KsUserTokenEntity {
        public int TokenNo {get;set;}
        public string KsUserId {get;set;} = "";
        public Guid Selector {get;set;}
        public byte[] ValidatorHash {get;set;} = new byte[0];
        public DateTime ExpirationDt {get;set;}
        public KsUserEntity User {get;set;} = new();
    }
}