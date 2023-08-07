//
// Created by Kraken KAML BO Generator
//

using System.Collections;

namespace Koretech.Kraken.BusinessObjects.KsUser
{
	public class KsUserRole
	{
		#region Properties

		public string KsUserId { get; set; }

		public string ResourceType { get; set; }

		public string ResourceName { get; set; }

		public int RoleNo { get; set; }

		#endregion Properties

		#region Relationships

		public List<KsUser> User = new();

		public KsRoleUser Role;

		#endregion Relationships
		}
	}
}
