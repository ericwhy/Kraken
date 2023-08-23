//
// Created by Kraken KAML BO Generator
//
// DO NOT MODIFY
//
namespace Koretech.Kraken.Entities.KsUser
{
	public class KsUserLoginFailureEntity
	{
	
		public string KsUserId {get; set;} = string.Empty;

		public DateTime FailDt {get; set;} = DateTime.Now;

		public IList<KsUserEntity> User {get; set;} = new List<KsUserEntity>();  // Navigation property to owner KsUserEntity

	}
}
