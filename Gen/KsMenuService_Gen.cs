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
	internal partial class KsMenuService : IKsMenuService
	{
		private KsMenuRepository _repository;

		public KsMenuService(KsMenuRepository repository)
		{
			_repository = repository;
		}

		public async Task<IEnumerable<KsMenu>> GetAllAsync()
		{
			return await _repository.GetAllAsync();
		}

		public async Task<KsMenu?> GetByPrimaryKeyAsync(int MenuNo)
		{
			return await _repository.GetByPrimaryKeyAsync(MenuNo);
		}

		public void Insert(KsMenu businessObject)
		{
			_repository.Insert(businessObject);
		}

		public void Update(KsMenu businessObject)
		{
			_repository.Update(businessObject);
		}

		public void Delete(KsMenu businessObject)
		{
			_repository.Delete(businessObject);
		}
	}
}
