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

using Koretech.Infrastructure.Services.KsUser.BusinessObjects;
using Koretech.Infrastructure.Services.KsUser.Repositories;

namespace Koretech.Infrastructure.Services.KsUser
{
	public interface IKsUserService
	{
		public KsUserService(KsUserRepository repository);

		public async Task<IEnumerable<KsUser>> GetAllAsync();

		public async Task<KsUser?> GetByPrimaryKeyAsync(string KsUserId);

		public void Insert(KsUser businessObject);

		public void Update(KsUser businessObject);

		public void Delete(KsUser businessObject);
	}
}
