//
// Created by Kraken KAML BO Generator
//
// DO NOT MODIFY
//
using Koretech.Kraken.Entities.KsRole;

namespace Koretech.Kraken.Entities.KsUser
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
