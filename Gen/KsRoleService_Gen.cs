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

using Koretech.Domains.KsRoles.BusinessObjects;
using Koretech.Domains.KsRoles.Repositories;

namespace Koretech.Domains.KsRoles
{
	internal partial class KsRoleService : IKsRoleService
	{
		private KsRoleRepository _repository;

		public KsRoleService(KsRoleRepository repository)
		{
			_repository = repository;
		}

		public async Task<IEnumerable<KsRoleUser>> GetAllAsync()
		{
			return await _repository.GetAllAsync();
		}

		public async Task<KsRoleUser?> GetByPrimaryKeyAsync(int RoleNo)
		{
			return await _repository.GetByPrimaryKeyAsync(RoleNo);
		}

		public void Insert(KsRoleUser businessObject)
		{
			_repository.Insert(businessObject);
		}

		public void Update(KsRoleUser businessObject)
		{
			_repository.Update(businessObject);
		}

		public void Delete(KsRoleUser businessObject)
		{
			_repository.Delete(businessObject);
		}
	}
}
