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
	public class KsPageContentEntityTypeConfiguration : IEntityTypeConfiguration<KsPageContentEntity>
	{
		public void Configure(EntityTypeBuilder<KsPageContentEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsPageContentEntity>("ks_page_content", "ks");

			typeBuilder.HasKey(e => new { e.PageName, e.ControlId })
				.HasName("ks_page_content_PK");

			typeBuilder.Property(e => e.PageName)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("page_name");

			typeBuilder.Property(e => e.ZoneName)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("zone_name");

			typeBuilder.Property(e => e.ControlId)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("control_id");

		}
	}
}
