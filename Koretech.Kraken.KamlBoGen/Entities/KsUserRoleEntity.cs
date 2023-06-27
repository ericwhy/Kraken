//
// Created by Kraken KAML BO Generator
//
namespace Koretech.Kraken.Data
{
	public class KsUserRoleEntity
	{
	
		public string? KsUserId {get; set;}
		public string? ResourceType {get; set;}
		public string? ResourceName {get; set;}
		public int? RoleNo {get; set;}
		public KsUserEntity User {get; set;} = new();  // Navigation property to parent KsUserEntity
	}
}
