namespace Koretech.Kraken.Data.Entity
{
    public class PasswordHistoryEntity
    {
        public string KsUserId { get; set; } = null!;

        public string Password { get; set; } = null!;

        public DateTime CreateDt { get; set; }

        public string? PasswordSalt { get; set; }

        public virtual KsUserEntity User { get; set; } = null!;
    }
}
