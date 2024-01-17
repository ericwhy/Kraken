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

namespace Koretech.Infrastructure.Services.KsUser.Entities
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
