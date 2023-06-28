namespace Koretech.Kraken.Data.Entity
{
    public class KsUserEntity {
        public string KsUserId {get;set;} = "";
        public string? DisplayName {get;set;} = "";
        public string? EmailAddress {get;set;} = "";
        public string? PasswordHints {get;set;} = "";
        public string? Password {get;set;} = "";
        public string? PasswordSalt {get;set;} = "";
        public DateTime? PasswordDt {get;set;}
        public string AllowAccessFlg {get;set;} = "";
        public string IntegratedAuth {get;set;} = "";
        public string AuthPrompt {get;set;} = "";
        public string PwresetFlg {get;set;} = "";  
        public byte? FailedLoginCnt {get;set;}
        public DateTime? FailedLoginDt {get;set;}
        public IList<KsUserLoginFailureEntity> LoginFailures {get;} = new List<KsUserLoginFailureEntity>();
        public IList<KsUserRoleEntity> UserRoles {get;} = new List<KsUserRoleEntity>();
        public IList<PasswordHistoryEntity> PasswordHistory {get;} = new List<PasswordHistoryEntity>();
        public IList<KsUserTokenEntity> UserTokens {get;set;} = new List<KsUserTokenEntity>();
    }
}
