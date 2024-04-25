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
	internal partial class KsPageService : IKsPageService
	{
		private KsPageRepository _repository;

		public KsPageService(KsPageRepository repository)
		{
			_repository = repository;
		}

		public async Task<IEnumerable<KsPage>> GetAllAsync()
		{
			return await _repository.GetAllAsync();
		}

		public async Task<KsPage?> GetByPrimaryKeyAsync(string PageName)
		{
			return await _repository.GetByPrimaryKeyAsync(PageName);
		}

		public void Insert(KsPage businessObject)
		{
			_repository.Insert(businessObject);
		}

		public void Update(KsPage businessObject)
		{
			_repository.Update(businessObject);
		}

		public void Delete(KsPage businessObject)
		{
			_repository.Delete(businessObject);
		}
	}
}
