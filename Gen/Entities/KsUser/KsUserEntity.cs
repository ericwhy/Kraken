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
	public class KsUserEntity
	{
	
		public string KsUserId {get; set;} = string.Empty;

		public string? DisplayName {get; set;}

		public string EmailAddress {get; set;} = string.Empty;

		public string? PasswordHints {get; set;}

		public string? Password {get; set;}

		public string? PasswordSalt {get; set;}

		public DateTime PasswordDt {get; set;} = DateTime.Now;

		public char AllowAccessFlg {get; set;}

		public char IntegratedAuth {get; set;}

		public char AuthPrompt {get; set;}

		public char PwresetFlg {get; set;}

		public byte? FailedLoginCnt {get; set;}

		public DateTime? FailedLoginDt {get; set;}

		public IList<KsUserLoginFailureEntity> LoginFailures {get; set;} = new List<KsUserLoginFailureEntity>();  // Navigation property to child KsUserLoginFailureEntity

		public IList<PasswordHistoryEntity> PasswordHistory {get; set;} = new List<PasswordHistoryEntity>();  // Navigation property to child PasswordHistoryEntity

		public IList<KsUserRoleEntity> UserRoles {get; set;} = new List<KsUserRoleEntity>();  // Navigation property to child KsUserRoleEntity

		public KsUserTokenEntity UserToken {get; set;}  // Navigation property to child KsUserTokenEntity

	}
}
