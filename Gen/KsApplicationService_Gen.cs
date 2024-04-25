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

using Koretech.Domains.KsApplications.BusinessObjects;
using Koretech.Domains.KsApplications.Repositories;

namespace Koretech.Domains.KsApplications
{
	internal partial class KsApplicationService : IKsApplicationService
	{
		private KsApplicationRepository _repository;

		public KsApplicationService(KsApplicationRepository repository)
		{
			_repository = repository;
		}

		public async Task<IEnumerable<KsApplication>> GetAllAsync()
		{
			return await _repository.GetAllAsync();
		}

		public async Task<KsApplication?> GetByPrimaryKeyAsync(int AppNo)
		{
			return await _repository.GetByPrimaryKeyAsync(AppNo);
		}

		public void Insert(KsApplication businessObject)
		{
			_repository.Insert(businessObject);
		}

		public void Update(KsApplication businessObject)
		{
			_repository.Update(businessObject);
		}

		public void Delete(KsApplication businessObject)
		{
			_repository.Delete(businessObject);
		}
	}
}
