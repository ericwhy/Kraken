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


namespace Koretech.Domains.KsMenus.Entities
{
	public class KsMenuItemEntity
	{
	
		public int? MenuItemNo { get; set; }

		public string? MenuTitle { get; set; }

		public string? MenuHref { get; set; }

		public int? MenuSeq { get; set; }

		public string? ResourceName { get; set; }

		public int? ParentMenuItemNo { get; set; }

		public char? DisplaySeparatorBeforeFlg { get; set; }

		public string? IconCssClass { get; set; }

		public string? IconName { get; set; }

		public int? MenuNo { get; set; }

		public string? PageName { get; set; }

		public char? PopupFlg { get; set; }

		public string? BodyHtml { get; set; }

	}
}
