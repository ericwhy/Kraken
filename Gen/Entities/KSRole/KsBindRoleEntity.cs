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


namespace Koretech.Domains.KsRoles.Entities
{
	public class KsBindRoleEntity
	{
	
		public int? RoleNo { get; set; }

		public string? IfColumnName { get; set; }

		public string? IfAttrName { get; set; }

		public string? IfOperator { get; set; }

		public string? IfValue { get; set; }

		public IList<KsRoleUserEntity> KsRoleUser { get; set; } = new List<KsRoleUserEntity>();  // Navigation property to owner KsRoleUserEntity

	}
}
