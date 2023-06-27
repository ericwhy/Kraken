//
// Created by Kraken KAML BO Generator
//
namespace Koretech.Kraken.Data
{
	public class PasswordHistoryEntity
	{
	
		public string KsUserId {get; set;} = string.Empty;
		public string? Password {get; set;}
		public string? PasswordSalt {get; set;}
		public DateTime CreateDt {get; set;} = DateTime.Now;
	}
}
