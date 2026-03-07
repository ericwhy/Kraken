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
		public Task<IEnumerable<KsRole>> GetAllAsync();

		public Task<KsRoleUser?> GetByPrimaryKeyAsync(int RoleNo);

		public void Insert(KsRoleUser businessObject);

		public void Update(KsRoleUser businessObject);

		public void Delete(KsRoleUser businessObject);
	}
}
