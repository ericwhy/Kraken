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

namespace Koretech.Infrastructure.Services.KsUser.Entities
{
	public class KsUserRoleEntity
	{
	
		public string KsUserId {get; set;} = string.Empty;

		public string ResourceType {get; set;} = string.Empty;

		public string ResourceName {get; set;} = string.Empty;

		public int RoleNo {get; set;}

		public IList<KsUserEntity> User {get; set;} = new List<KsUserEntity>();  // Navigation property to owner KsUserEntity

		public KsRoleUserEntity Role {get; set;}  // Navigation property to child KsRoleUserEntity
		// This is an inter-domain relationship. Not fully implemented yet.

	}
}
