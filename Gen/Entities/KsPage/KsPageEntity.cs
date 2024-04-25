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


namespace Koretech.Domains.KsPages.Entities
{
	public class KsPageEntity
	{
	
		public string? PageName { get; set; }

		public string? Title { get; set; }

		public string? LayoutName { get; set; }

		public string? TypeCd { get; set; }

		public string? MetaDescription { get; set; }

		public string? MetaKeywords { get; set; }

		public string? AdditionalMetaElements { get; set; }

		public string? Controller { get; set; }

		public string? Action { get; set; }

		public byte[]? MetadataHash { get; set; }

		public char? TitleVisibleFlg { get; set; }

		public char? FullWidthFlg { get; set; }

		public string? LogicalPageName { get; set; }

		public string? RedirectPageName { get; set; }

	}
}
