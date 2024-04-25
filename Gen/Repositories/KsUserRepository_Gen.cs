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

using Koretech.Domains.Repositories;
using Koretech.Domains.KsUsers.BusinessObjects;
using Koretech.Domains.KsUsers.Entities;
using Microsoft.EntityFrameworkCore;

namespace Koretech.Domains.KsUsers.Repositories
{
	internal partial class KsUserRepository : Repository<KsUserEntity>
	{
		public KsUserRepository(KsUserContext dbContext) : base(dbContext) { }

		public async Task<IEnumerable<KsUser>> GetAllAsync()
		{
			var entities = await FindAll()
				.ToListAsync();
			IList<KsUser> businessObjects = KsUser.NewInstance(entities);
			return businessObjects;
		}

		public async Task<KsUser?> GetByPrimaryKeyAsync(string KsUserId)
		{
			KsUserEntity? entity = await FindByCondition(e => e.KsUserId.Equals(KsUserId))
				.FirstOrDefaultAsync();
			KsUser? businessObject = (entity != null) ? KsUser.NewInstance(entity) : null;
			return businessObject;
		}

		public void Insert(KsUser businessObject)
		{
			Insert(businessObject.Entity);
		}

		public void Update(KsUser businessObject)
		{
			Update(businessObject.Entity);
		}

		public void Delete(KsUser businessObject)
		{
			Delete(businessObject.Entity);
		}
	}
}
