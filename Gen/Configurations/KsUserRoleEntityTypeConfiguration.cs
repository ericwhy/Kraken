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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Koretech.Domains.KsUsers.EntityConfigurations
{
	public class KsUserRoleEntityTypeConfiguration : IEntityTypeConfiguration<KsUserRoleEntity>
	{
		public void Configure(EntityTypeBuilder<KsUserRoleEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsUserRoleEntity>("ks_user_role", "ks");

			typeBuilder.HasKey(e => new { e.KsUserId, e.ResourceType, e.ResourceName, e.RoleNo })
				.HasName("ks_user_role_PK");

			typeBuilder.Property(e => e.KsUserId)
				.HasMaxLength(60)
				.IsUnicode(false)
				.HasColumnName("ks_user_id");

			typeBuilder.Property(e => e.ResourceType)
				.HasMaxLength(20)
				.IsUnicode(false)
				.HasColumnName("resource_type");

			typeBuilder.Property(e => e.ResourceName)
				.HasMaxLength(20)
				.IsUnicode(false)
				.HasColumnName("resource_name");

			typeBuilder.Property(e => e.RoleNo)
				.HasColumnName("role_no");

		}
	}
}
