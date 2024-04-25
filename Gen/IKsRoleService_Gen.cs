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
	public partial interface IKsRoleService
	{
		public Task<IEnumerable<BusinessObjects.KsRole>> GetAllAsync();

		public Task<BusinessObjects.KsRoleUser?> GetByPrimaryKeyAsync(int RoleNo);

		public void Insert(BusinessObjects.KsRoleUser businessObject);

		public void Update(BusinessObjects.KsRoleUser businessObject);

		public void Delete(BusinessObjects.KsRoleUser businessObject);
	}
}
