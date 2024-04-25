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
	public class KsUserLoginFailureEntity
	{
	
		public string KsUserId { get; set; } = string.Empty;

		public DateTime FailDt { get; set; } = DateTime.Now;

		public IList<KsUserEntity> User { get; set; } = new List<KsUserEntity>();  // Navigation property to owner KsUserEntity

	}
}
