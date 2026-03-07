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

		public IList<KsPageContentEntity> KsPageContents { get; set; } = new List<KsPageContentEntity>();  // Navigation property to child KsPageContentEntity

		public IList<KsPageObjectEntity> KsPageObjects { get; set; } = new List<KsPageObjectEntity>();  // Navigation property to child KsPageObjectEntity

		public IList<KsPageParameterEntity> KsPageParameters { get; set; } = new List<KsPageParameterEntity>();  // Navigation property to child KsPageParameterEntity

		public IList<KsPageSecurityEntity> KsPageSecuritys { get; set; } = new List<KsPageSecurityEntity>();  // Navigation property to child KsPageSecurityEntity

	}
}
