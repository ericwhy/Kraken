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
	public class KsPageModuleEntity
	{
	
		public int? PageModuleNo { get; set; }

		public string? PageName { get; set; }

		public int? ModuleNo { get; set; }

		public string? Title { get; set; }

		public string? ZoneName { get; set; }

		public int? Sequence { get; set; }

		public string? CssClass { get; set; }

		public KsPageEntity? Page { get; set; } = null;  // Navigation property to owner KsPageEntity

		public KsModuleEntity? Module { get; set; } = null;  // Navigation property to child KsModuleEntity

	}
}
