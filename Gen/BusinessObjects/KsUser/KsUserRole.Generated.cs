//
// Created by Kraken KAML BO Generator
//

using Koretech.Framework.BusinessObjects;
using Koretech.KommerceServer.BusinessObjects.Utility;
using System.ComponentModel;
using System.Collections;

namespace Koretech.Kraken.BusinessObjects.KsUser
{
	public class KsUserRoleBase : BODomainEntity<KsUserRoleBase>, IDomainObject
	{
		#region Properties
			public string KsUserId { get; set; }

			public string ResourceType { get; set; }

			public string ResourceName { get; set; }

			public int RoleNo { get; set; }

		#endregion Properties

		#region Relationships
			public List<KsUser> User = new();

			public List<KsRoleUser> Role = new();

		#endregion Relationships
		}
	}
}
