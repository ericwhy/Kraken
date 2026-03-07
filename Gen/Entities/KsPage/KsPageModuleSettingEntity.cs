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
	public class KsPageModuleSettingEntity
	{
	
		public int? PageModuleNo { get; set; }

		public string? Name { get; set; }

		public string? Value { get; set; }

		public KsPageModuleEntity? PageModule { get; set; } = null;  // Navigation property to owner KsPageModuleEntity

	}
}
