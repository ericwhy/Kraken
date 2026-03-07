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
	public class KsPageModuleEntityTypeConfiguration : IEntityTypeConfiguration<KsPageModuleEntity>
	{
		public void Configure(EntityTypeBuilder<KsPageModuleEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsPageModuleEntity>("ks_page_module", "ks");

			typeBuilder.HasKey(e => new { e.PageModuleNo })
				.HasName("ks_page_module_PK");

			typeBuilder.Property(e => e.PageModuleNo)
				.HasColumnName("page_module_no");

			typeBuilder.Property(e => e.PageName)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("page_name");

			typeBuilder.Property(e => e.ModuleNo)
				.HasColumnName("module_no");

			typeBuilder.Property(e => e.Title)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("title");

			typeBuilder.Property(e => e.ZoneName)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("zone_name");

			typeBuilder.Property(e => e.Sequence)
				.HasColumnName("sequence");

			typeBuilder.Property(e => e.CssClass)
				.HasMaxLength(750)
				.IsUnicode(false)
				.HasColumnName("css_class");

		}
	}
}
