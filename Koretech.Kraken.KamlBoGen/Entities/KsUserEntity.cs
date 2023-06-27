//
// Created by Kraken KAML BO Generator
//
namespace Koretech.Kraken.Data
{
	public class KsUserEntity
	{
	
		public string? KsUserId {get; set;}
		public string? DisplayName {get; set;}
		public string? EmailAddress {get; set;}
		public string? PasswordHints {get; set;}
		public string? Password {get; set;}
		public string? PasswordSalt {get; set;}
		public DateTime? PasswordDt {get; set;}
		public string? AllowAccessFlg {get; set;}
		public string? IntegratedAuth {get; set;}
		public string? AuthPrompt {get; set;}
		public string? PwresetFlg {get; set;}
		public int? FailedLoginCnt {get; set;}
		public DateTime? FailedLoginDt {get; set;}
		public IList<KsUserLoginFailureEntity> LoginFailures {get; set;} = new List<KsUserLoginFailureEntity>();  // Navigation property to child KsUserLoginFailureEntity
		public IList<PasswordHistoryEntity> PasswordHistory {get; set;} = new List<PasswordHistoryEntity>();  // Navigation property to child PasswordHistoryEntity
		public IList<KsUserRoleEntity> UserRoles {get; set;} = new List<KsUserRoleEntity>();  // Navigation property to child KsUserRoleEntity
		public KsUserTokenEntity UserToken {get; set;} = new();  // Navigation property to parent KsUserTokenEntity
	}
}
