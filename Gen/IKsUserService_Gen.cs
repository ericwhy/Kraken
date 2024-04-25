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
	public partial interface IKsUserService
	{
		public Task<IEnumerable<BusinessObjects.KsUser>> GetAllAsync();

		public Task<BusinessObjects.KsUser?> GetByPrimaryKeyAsync(string KsUserId);

		public void Insert(BusinessObjects.KsUser businessObject);

		public void Update(BusinessObjects.KsUser businessObject);

		public void Delete(BusinessObjects.KsUser businessObject);
	}
}
