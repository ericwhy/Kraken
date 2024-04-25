/********************************************************/
/*                                                      */
/* Created by Kraken KAML BO Generator                  */
/*                                                      */
/* DO NOT MODIFY                                        */
/*                                                      */
/* Extensions or overrides should be placed in a        */
/* subclass or partial class, whichever is appropriate. */
/*                                                      */
/********************************************************/


namespace Koretech.Domains.KsUsers.Entities
{
	public class KsUserTokenEntity
	{
	
		public int TokenNo { get; set; }

		public Guid Selector { get; set; }

		public byte[] ValidatorHash { get; set; } = new byte[32];

		public string KsUserId { get; set; } = string.Empty;

		public DateTime ExpirationDt { get; set; } = DateTime.Now;

		public KsUserEntity? KsUser { get; set; } = null;  // Navigation property to owner KsUserEntity

	}
}
