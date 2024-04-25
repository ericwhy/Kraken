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
	public class KsPageEntityTypeConfiguration : IEntityTypeConfiguration<KsPageEntity>
	{
		public void Configure(EntityTypeBuilder<KsPageEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsPageEntity>("ks_page", "ks");

			typeBuilder.HasKey(e => new { e.PageName })
				.HasName("ks_page_PK");

			typeBuilder.Property(e => e.PageName)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("page_name");

			typeBuilder.Property(e => e.Title)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("title");

			typeBuilder.Property(e => e.LayoutName)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("layout_name");

			typeBuilder.Property(e => e.TypeCd)
				.HasMaxLength(1)
				.IsUnicode(false)
				.HasColumnName("type_cd");

			typeBuilder.Property(e => e.MetaDescription)
				.HasMaxLength(2147483647)
				.IsUnicode(false)
				.HasColumnName("meta_description");

			typeBuilder.Property(e => e.MetaKeywords)
				.HasMaxLength(2147483647)
				.IsUnicode(false)
				.HasColumnName("meta_keywords");

			typeBuilder.Property(e => e.AdditionalMetaElements)
				.HasMaxLength(2147483647)
				.IsUnicode(false)
				.HasColumnName("add_meta_elements");

			typeBuilder.Property(e => e.Controller)
				.HasMaxLength(50)
				.IsUnicode(false)
				.HasColumnName("controller");

			typeBuilder.Property(e => e.Action)
				.HasMaxLength(50)
				.IsUnicode(false)
				.HasColumnName("action");

			typeBuilder.Property(e => e.MetadataHash)
				.HasColumnType("varbinary")
				.HasMaxLength(128)
				.HasColumnName("metadata_hash");

			typeBuilder.Property(e => e.TitleVisibleFlg)
				.HasMaxLength(1)
				.IsUnicode(false)
				.HasDefaultValueSql("('N')")
				.IsFixedLength()
				.HasColumnName("title_visible_flg");

			typeBuilder.Property(e => e.FullWidthFlg)
				.HasMaxLength(1)
				.IsUnicode(false)
				.HasDefaultValueSql("('N')")
				.IsFixedLength()
				.HasColumnName("full_width_flg");

			typeBuilder.Property(e => e.LogicalPageName)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("logical_page_name");

			typeBuilder.Property(e => e.RedirectPageName)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("redirect_page_name");

		}
	}
}
