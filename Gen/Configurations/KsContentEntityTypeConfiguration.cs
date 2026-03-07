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
	public class KsContentEntityTypeConfiguration : IEntityTypeConfiguration<KsContentEntity>
	{
		public void Configure(EntityTypeBuilder<KsContentEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsContentEntity>("ks_content", "ks");

			typeBuilder.HasKey(e => new { e.ContentId })
				.HasName("ks_content_PK");

			typeBuilder.Property(e => e.ContentId)
				.HasMaxLength(255)
				.IsUnicode(false)
				.HasColumnName("content_id");

			typeBuilder.Property(e => e.HtmlContent)
				.HasMaxLength(2147483647)
				.IsUnicode(false)
				.HasColumnName("html_content");

		}
	}
}
