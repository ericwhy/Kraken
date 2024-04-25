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

using Koretech.Domains.KsMenus.BusinessObjects;
using Koretech.Domains.KsMenus.Repositories;

namespace Koretech.Domains.KsMenus
{
	public partial interface IKsMenuService
	{
		public Task<IEnumerable<KsMenu>> GetAllAsync();

		public Task<KsMenu?> GetByPrimaryKeyAsync(int MenuNo);

		public void Insert(KsMenu businessObject);

		public void Update(KsMenu businessObject);

		public void Delete(KsMenu businessObject);
	}
}
