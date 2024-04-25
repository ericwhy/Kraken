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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Koretech.Domains.KsRoles.EntityConfigurations
{
	public class KsRoleUserEntityTypeConfiguration : IEntityTypeConfiguration<KsRoleUserEntity>
	{
		public void Configure(EntityTypeBuilder<KsRoleUserEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsRoleUserEntity>("ks_role_user", "ks");

			typeBuilder.HasKey(e => new { e.RoleNo })
				.HasName("ks_role_user_PK");

			typeBuilder.Property(e => e.RoleNo)
				.HasColumnName("role_no");

			typeBuilder.Property(e => e.RoleDesc)
				.HasMaxLength(255)
				.IsUnicode(false)
				.HasColumnName("role_desc");

			typeBuilder.Property(e => e.WebAssignFlg)
				.HasMaxLength(1)
				.IsUnicode(false)
				.HasDefaultValueSql("('N')")
				.IsFixedLength()
				.HasColumnName("web_assign_flg");

			typeBuilder.Property(e => e.ImplicitAssignFlg)
				.HasMaxLength(1)
				.IsUnicode(false)
				.HasDefaultValueSql("('N')")
				.IsFixedLength()
				.HasColumnName("implicit_assign_flg");

			typeBuilder.Property(e => e.HomePage)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("home_page");

			typeBuilder.Property(e => e.HomePageQueryString)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("home_page_link_querystring");

		}
	}
}
