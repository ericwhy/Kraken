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


using Koretech.Infrastructure.Services.KsUser.Entities;
using Koretech.Infrastructure.Services.KsUser.Repositories;
using KsUserBO = Koretech.Infrastructure.Services.KsUser.BusinessObjects.KsUser;

namespace Koretech.Infrastructure.Services.KsUser
{
	internal interface IKsUserService
	{
		public Task<IEnumerable<KsUserBO>> GetAllAsync();

		public Task<KsUserBO?> GetByPrimaryKeyAsync(string KsUserId);

		public void Insert(KsUserBO businessObject);

		public void Update(KsUserBO businessObject);

		public void Delete(KsUserBO businessObject);
	}
}
