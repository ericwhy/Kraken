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

using Koretech.Domains.KsUsers.Entities;
using Koretech.Domains.KsObjects.Entities;
using Koretech.Domains.KsPages.Entities;


namespace Koretech.Domains.KsRoles.Entities
{
	public class KsRoleUserEntity
	{
	
		public int? RoleNo { get; set; }

		public string? RoleDesc { get; set; }

		public char? WebAssignFlg { get; set; }

		public char? ImplicitAssignFlg { get; set; }

		public string? HomePage { get; set; }

		public string? HomePageQueryString { get; set; }

		public IList<KsBindRoleEntity> KsBindRoles { get; set; } = new List<KsBindRoleEntity>();  // Navigation property to child KsBindRoleEntity

		public IList<KsUserRoleEntity> KsUsers { get; set; } = new List<KsUserRoleEntity>();  // Navigation property to child KsUserRoleEntity
		// This is an inter-domain relationship. Not fully implemented yet.

		public IList<KsObjectMethodSecurityEntity> KsObjectMethodSecurities { get; set; } = new List<KsObjectMethodSecurityEntity>();  // Navigation property to child KsObjectMethodSecurityEntity
		// This is an inter-domain relationship. Not fully implemented yet.

		public IList<KsObjectPropertySecurityEntity> KsObjectPropertySecurities { get; set; } = new List<KsObjectPropertySecurityEntity>();  // Navigation property to child KsObjectPropertySecurityEntity
		// This is an inter-domain relationship. Not fully implemented yet.

		public IList<KsPageSecurityEntity> KsPageSecurities { get; set; } = new List<KsPageSecurityEntity>();  // Navigation property to child KsPageSecurityEntity
		// This is an inter-domain relationship. Not fully implemented yet.

	}
}
