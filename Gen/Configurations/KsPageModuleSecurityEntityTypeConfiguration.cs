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
	public class KsPageModuleSecurityEntityTypeConfiguration : IEntityTypeConfiguration<KsPageModuleSecurityEntity>
	{
		public void Configure(EntityTypeBuilder<KsPageModuleSecurityEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsPageModuleSecurityEntity>("ks_page_module_security", "ks");

			typeBuilder.HasKey(e => new { e.PageModuleNo })
				.HasName("ks_page_module_security_PK");

			typeBuilder.Property(e => e.PageModuleNo)
				.HasColumnName("page_module_no");

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
