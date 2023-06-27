//
// Created by Kraken KAML BO Generator
//
namespace Koretech.Kraken.Data
{
	public class KsUserTokenEntity
	{
	
		public int TokenNo {get; set;}
		public Guid Selector {get; set;}
		public byte[] ValidatorHash {get; set;} = new byte[32];
		public string KsUserId {get; set;} = string.Empty;
		public DateTime ExpirationDt {get; set;} = DateTime.Now;
	}
}
