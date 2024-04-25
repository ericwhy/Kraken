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

using Koretech.Domains.KsUsers.BusinessObjects;
using Koretech.Domains.KsUsers.Repositories;

namespace Koretech.Domains.KsUsers
{
	internal partial class KsUserService : IKsUserService
	{
		private KsUserRepository _repository;

		public KsUserService(KsUserRepository repository)
		{
			_repository = repository;
		}

		public async Task<IEnumerable<KsUser>> GetAllAsync()
		{
			return await _repository.GetAllAsync();
		}

		public async Task<KsUser?> GetByPrimaryKeyAsync(string KsUserId)
		{
			return await _repository.GetByPrimaryKeyAsync(KsUserId);
		}

		public void Insert(KsUser businessObject)
		{
			_repository.Insert(businessObject);
		}

		public void Update(KsUser businessObject)
		{
			_repository.Update(businessObject);
		}

		public void Delete(KsUser businessObject)
		{
			_repository.Delete(businessObject);
		}
	}
}
