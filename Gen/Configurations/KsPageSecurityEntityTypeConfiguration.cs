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

using Koretech.Domains.KsPages.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Koretech.Domains.KsPages.EntityConfigurations
{
	public class KsPageSecurityEntityTypeConfiguration : IEntityTypeConfiguration<KsPageSecurityEntity>
	{
		public void Configure(EntityTypeBuilder<KsPageSecurityEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsPageSecurityEntity>("ks_page_security", "ks");

			typeBuilder.HasKey(e => new { e.PageName, e.RoleNo })
				.HasName("ks_page_security_PK");

			typeBuilder.Property(e => e.PageName)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("page_name");

			typeBuilder.Property(e => e.RoleNo)
				.HasColumnName("role_no");

			typeBuilder.Property(e => e.DenyAccessOverrideFlg)
				.HasMaxLength(1)
				.IsUnicode(false)
				.HasDefaultValueSql("('N')")
				.IsFixedLength()
				.HasColumnName("deny_access_override_flg");

		}
	}
}
