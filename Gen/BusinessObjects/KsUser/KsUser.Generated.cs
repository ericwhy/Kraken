//
// Created by Kraken KAML BO Generator
//

using Koretech.Framework.BusinessObjects;
using Koretech.KommerceServer.BusinessObjects.Utility;
using System.ComponentModel;
using System.Collections;

namespace Koretech.Kraken.BusinessObjects.KsUser
{
	public class KsUserBase : BODomainEntity<KsUserBase>, IDomainObject
	{
		#region Properties
			public string KsUserId { get; set; }

			public string DisplayName { get; set; }

			public string EmailAddress { get; set; }

			public string PasswordHints { get; set; }

			public string Password { get; set; }

			public string PasswordSalt { get; set; }

			public DateTime PasswordDt { get; set; }

			public char AllowAccessFlg { get; set; }

			public char IntegratedAuth { get; set; }

			public char AuthPrompt { get; set; }

			public char PwresetFlg { get; set; }

			public int FailedLoginCnt { get; set; }

			public DateTime FailedLoginDt { get; set; }

		#endregion Properties

		#region Relationships
			public List<KsUserLoginFailure> LoginFailures = new();

			public List<PasswordHistory> PasswordHistory = new();

			public List<KsUserRole> UserRoles = new();

			public List<KsUserToken> UserToken = new();

		#endregion Relationships
		}
	}
}
