//
// Created by Kraken KAML BO Generator
//
// DO NOT MODIFY
//
namespace Koretech.Kraken.Entities.KsUser
{
	public class PasswordHistoryEntity
	{
	
		public string KsUserId {get; set;} = string.Empty;

		public string? Password {get; set;}

		public string? PasswordSalt {get; set;}

		public DateTime CreateDt {get; set;} = DateTime.Now;

		public IList<KsUserEntity> KsUser {get; set;} = new List<KsUserEntity>();  // Navigation property to owner KsUserEntity

	}
}
