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

using Koretech.Domains.KsMenus.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Koretech.Domains.KsMenus.EntityConfigurations
{
	public class KsMenuEntityTypeConfiguration : IEntityTypeConfiguration<KsMenuEntity>
	{
		public void Configure(EntityTypeBuilder<KsMenuEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsMenuEntity>("ks_menu", "ks");

			typeBuilder.HasKey(e => new { e.MenuNo })
				.HasName("ks_menu_PK");

			typeBuilder.Property(e => e.MenuNo)
				.HasColumnName("menu_no");

			typeBuilder.Property(e => e.MenuId)
				.HasMaxLength(60)
				.IsUnicode(false)
				.HasColumnName("menu_id");

			typeBuilder.Property(e => e.MenuName)
				.HasMaxLength(750)
				.IsUnicode(false)
				.HasColumnName("menu_name");

			typeBuilder.Property(e => e.FooterHtml)
				.HasMaxLength(2147483647)
				.IsUnicode(false)
				.HasColumnName("footer_html");

			typeBuilder.Property(e => e.Style)
				.HasMaxLength(1)
				.IsUnicode(false)
				.HasColumnName("style");

			typeBuilder.Property(e => e.TitleVisibleFlg)
				.HasMaxLength(1)
				.IsUnicode(false)
				.HasDefaultValueSql("('N')")
				.IsFixedLength()
				.HasColumnName("title_visible_flg");

		}
	}
}
