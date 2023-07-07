//
// Created by Kraken KAML BO Generator
//

using Koretech.Framework.BusinessObjects;
using Koretech.KommerceServer.BusinessObjects.Utility;
using System.ComponentModel;
using System.Collections;

namespace Koretech.Kraken.BusinessObjects.KsUser
{
	public class KsUserTokenBase : BODomainEntity<KsUserTokenBase>, IDomainObject
	{
		#region Properties
			public int TokenNo { get; set; }

			public Guid Selector { get; set; }

			public byte[] ValidatorHash { get; set; }

			public string KsUserId { get; set; }

			public DateTime ExpirationDt { get; set; }

		#endregion Properties

		#region Relationships
			public List<KsUser> KsUser = new();

		#endregion Relationships
		}
	}
}
