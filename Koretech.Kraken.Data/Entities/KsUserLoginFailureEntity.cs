namespace Koretech.Kraken.Data.Entity
{
    public class KsUserLoginFailureEntity
    {
        public string KsUserId { get; set; } = string.Empty;
        public DateTime FailDt { get; set;}
        public KsUserEntity User { get; set; } = new();
    }
}
