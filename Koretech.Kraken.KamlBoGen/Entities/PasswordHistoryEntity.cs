//
// Created by Kraken KAML BO Generator
//
namespace Koretech.Kraken.Data
{
	public class PasswordHistoryEntity
	{
	
		public string? KsUserId {get; set;}
		public string? Password {get; set;}
		public string? PasswordSalt {get; set;}
		public DateTime? CreateDt {get; set;}
		public KsUserEntity KsUser {get; set;} = new();  // Navigation property to parent KsUserEntity
	}
}
