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

using Koretech.Domains.KsRoles.Entities;


namespace Koretech.Domains.KsPages.Entities
{
	public class KsPageModuleSecurityEntity
	{
	
		public int? PageModuleNo { get; set; }

		public int? RoleNo { get; set; }

		public char? DenyAccessOverrideFlg { get; set; }

		public KsPageModuleEntity? PageModule { get; set; } = null;  // Navigation property to owner KsPageModuleEntity

		public KsRoleUserEntity? Role { get; set; } = null;  // Navigation property to child KsRoleUserEntity
		// This is an inter-domain relationship.

	}
}
