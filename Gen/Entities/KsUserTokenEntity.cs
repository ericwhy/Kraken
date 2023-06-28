//
// Created by Kraken KAML BO Generator
//
namespace Koretech.Kraken.Data
{
	public class KsUserTokenEntity
	{
	
		public int? TokenNo {get; set;}
		public Guid? Selector {get; set;}
		public byte[]? ValidatorHash {get; set;}
		public string? KsUserId {get; set;}
		public DateTime? ExpirationDt {get; set;}
		public KsUserEntity KsUser {get; set;} = new();  // Navigation property to parent KsUserEntity
	}
}
