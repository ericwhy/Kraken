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
	public class KsPageObjectEntityTypeConfiguration : IEntityTypeConfiguration<KsPageObjectEntity>
	{
		public void Configure(EntityTypeBuilder<KsPageObjectEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsPageObjectEntity>("ks_page_object", "ks");

			typeBuilder.HasKey(e => new { e.PageName, e.ObjectId })
				.HasName("ks_page_object_PK");

			typeBuilder.Property(e => e.PageName)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("page_name");

			typeBuilder.Property(e => e.ObjectId)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("object_id");

			typeBuilder.Property(e => e.PrimaryFlg)
				.HasMaxLength(1)
				.IsUnicode(false)
				.HasDefaultValueSql("('N')")
				.IsFixedLength()
				.HasColumnName("primary_flg");

		}
	}
}
