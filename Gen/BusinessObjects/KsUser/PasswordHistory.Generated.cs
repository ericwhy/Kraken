//
// Created by Kraken KAML BO Generator
//

using Koretech.Framework.BusinessObjects;
using Koretech.KommerceServer.BusinessObjects.Utility;
using System.ComponentModel;
using System.Collections;

namespace Koretech.Kraken.BusinessObjects.KsUser
{
	public class PasswordHistoryBase : BODomainEntity<PasswordHistoryBase>, IDomainObject
	{
		#region Properties
			public string KsUserId { get; set; }

			public string Password { get; set; }

			public string PasswordSalt { get; set; }

			public DateTime CreateDt { get; set; }

		#endregion Properties

		#region Relationships
			public List<KsUser> KsUser = new();

		#endregion Relationships
		}
	}
}
