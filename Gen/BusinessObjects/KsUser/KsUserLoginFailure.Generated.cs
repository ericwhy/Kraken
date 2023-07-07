//
// Created by Kraken KAML BO Generator
//

using Koretech.Framework.BusinessObjects;
using Koretech.KommerceServer.BusinessObjects.Utility;
using System.ComponentModel;
using System.Collections;

namespace Koretech.Kraken.BusinessObjects.KsUser
{
	public class KsUserLoginFailureBase : BODomainEntity<KsUserLoginFailureBase>, IDomainObject
	{
		#region Properties
			public string KsUserId { get; set; }

			public DateTime FailDt { get; set; }

		#endregion Properties

		#region Relationships
			public List<KsUser> User = new();

		#endregion Relationships
		}
	}
}
