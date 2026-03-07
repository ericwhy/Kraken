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
	public class KsPageModuleSettingEntityTypeConfiguration : IEntityTypeConfiguration<KsPageModuleSettingEntity>
	{
		public void Configure(EntityTypeBuilder<KsPageModuleSettingEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsPageModuleSettingEntity>("ks_page_module_setting", "ks");

			typeBuilder.HasKey(e => new { e.PageModuleNo, e.Name })
				.HasName("ks_page_module_setting_PK");

			typeBuilder.Property(e => e.PageModuleNo)
				.HasColumnName("page_module_no");

			typeBuilder.Property(e => e.Name)
				.HasMaxLength(50)
				.IsUnicode(false)
				.HasColumnName("name");

			typeBuilder.Property(e => e.Value)
				.HasMaxLength(2147483647)
				.IsUnicode(false)
				.HasColumnName("value");

		}
	}
}
