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
	public class KsMenuItemEntityTypeConfiguration : IEntityTypeConfiguration<KsMenuItemEntity>
	{
		public void Configure(EntityTypeBuilder<KsMenuItemEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsMenuItemEntity>("ks_menu_item", "ks");

			typeBuilder.HasKey(e => new { e.MenuItemNo })
				.HasName("ks_menu_item_PK");

			typeBuilder.Property(e => e.MenuItemNo)
				.HasColumnName("menu_item_no");

			typeBuilder.Property(e => e.MenuTitle)
				.HasMaxLength(750)
				.IsUnicode(false)
				.HasColumnName("menu_title");

			typeBuilder.Property(e => e.MenuHref)
				.HasMaxLength(750)
				.IsUnicode(false)
				.HasColumnName("menu_href");

			typeBuilder.Property(e => e.MenuSeq)
				.HasColumnName("menu_seq");

			typeBuilder.Property(e => e.ResourceName)
				.HasMaxLength(750)
				.IsUnicode(false)
				.HasColumnName("resource_name");

			typeBuilder.Property(e => e.ParentMenuItemNo)
				.HasColumnName("parent_menu_item_no");

			typeBuilder.Property(e => e.DisplaySeparatorBeforeFlg)
				.HasMaxLength(1)
				.IsUnicode(false)
				.HasDefaultValueSql("('N')")
				.IsFixedLength()
				.HasColumnName("display_separator_before_flg");

			typeBuilder.Property(e => e.IconCssClass)
				.HasMaxLength(750)
				.IsUnicode(false)
				.HasColumnName("icon_css_class");

			typeBuilder.Property(e => e.IconName)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("icon_name");

			typeBuilder.Property(e => e.MenuNo)
				.HasColumnName("menu_no");

			typeBuilder.Property(e => e.PageName)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("page_name");

			typeBuilder.Property(e => e.PopupFlg)
				.HasMaxLength(1)
				.IsUnicode(false)
				.HasDefaultValueSql("('N')")
				.IsFixedLength()
				.HasColumnName("popup_flg");

			typeBuilder.Property(e => e.BodyHtml)
				.HasMaxLength(2147483647)
				.IsUnicode(false)
				.HasColumnName("body_html");

		}
	}
}
