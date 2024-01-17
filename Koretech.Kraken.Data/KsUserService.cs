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


using Koretech.Infrastructure.Services.KsUser.Repositories;
using KsUserBO = Koretech.Infrastructure.Services.KsUser.BusinessObjects.KsUser;

namespace Koretech.Infrastructure.Services.KsUser
{
	internal class KsUserService : IKsUserService
	{
		private KsUserRepository _repository;

		public KsUserService(KsUserRepository repository) 
		{ 
			_repository = repository;
		}

		public async Task<IEnumerable<KsUserBO>> GetAllAsync()
		{
			return await _repository.GetAllAsync();
		}

		public async Task<KsUserBO?> GetByPrimaryKeyAsync(string KsUserId)
		{
			return await _repository.GetByPrimaryKeyAsync(KsUserId);
		}

		public void Insert(KsUserBO businessObject)
		{
			_repository.Insert(businessObject);
		}

		public void Update(KsUserBO businessObject)
		{
			_repository.Update(businessObject);
		}

		public void Delete(KsUserBO businessObject)
		{
			_repository.Delete(businessObject);
		}
	}
}
