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
		public Task<IEnumerable<KsUser>> GetAllAsync();

		public Task<KsUser?> GetByPrimaryKeyAsync(string KsUserId);

		public void Insert(KsUser businessObject);

		public void Update(KsUser businessObject);

		public void Delete(KsUser businessObject);
	}
}
