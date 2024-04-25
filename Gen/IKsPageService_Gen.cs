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

using Koretech.Domains.KsPages.BusinessObjects;
using Koretech.Domains.KsPages.Repositories;

namespace Koretech.Domains.KsPages
{
	public partial interface IKsPageService
	{
		public Task<IEnumerable<KsPage>> GetAllAsync();

		public Task<KsPage?> GetByPrimaryKeyAsync(string PageName);

		public void Insert(KsPage businessObject);

		public void Update(KsPage businessObject);

		public void Delete(KsPage businessObject);
	}
}
