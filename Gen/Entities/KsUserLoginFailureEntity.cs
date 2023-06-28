//
// Created by Kraken KAML BO Generator
//
namespace Koretech.Kraken.Data
{
	public class KsUserLoginFailureEntity
	{
	
		public string KsUserId {get; set;} = string.Empty;
		public DateTime FailDt {get; set;} = DateTime.Now;
		public KsUserEntity User {get; set;} = new();  // Navigation property to parent KsUserEntity
	}
}
