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
using Koretech.Domains.KsRoles.BusinessObjects;
using Koretech.Domains.KsRoles.Entities;
using Microsoft.EntityFrameworkCore;

namespace Koretech.Domains.KsRoles.Repositories
{
	internal partial class KsRoleRepository : Repository<KsRoleUserEntity>
	{
		public KsRoleRepository(KsRoleContext dbContext) : base(dbContext) { }

		public async Task<IEnumerable<KsRoleUser>> GetAllAsync()
		{
			var entities = await FindAll()
				.ToListAsync();
			IList<KsRoleUser> businessObjects = KsRoleUser.NewInstance(entities);
			return businessObjects;
		}

		public async Task<KsRoleUser?> GetByPrimaryKeyAsync(int RoleNo)
		{
			KsRoleUserEntity? entity = await FindByCondition(e => e.RoleNo.Equals(RoleNo))
				.FirstOrDefaultAsync();
			KsRoleUser? businessObject = (entity != null) ? KsRoleUser.NewInstance(entity) : null;
			return businessObject;
		}

		public void Insert(KsRoleUser businessObject)
		{
			Insert(businessObject.Entity);
		}

		public void Update(KsRoleUser businessObject)
		{
			Update(businessObject.Entity);
		}

		public void Delete(KsRoleUser businessObject)
		{
			Delete(businessObject.Entity);
		}
	}
}
