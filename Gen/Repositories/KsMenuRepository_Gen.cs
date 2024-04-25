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
using Koretech.Domains.KsMenus.BusinessObjects;
using Koretech.Domains.KsMenus.Entities;
using Microsoft.EntityFrameworkCore;

namespace Koretech.Domains.KsMenus.Repositories
{
	internal partial class KsMenuRepository : Repository<KsMenuEntity>
	{
		public KsMenuRepository(KsMenuContext dbContext) : base(dbContext) { }

		public async Task<IEnumerable<KsMenu>> GetAllAsync()
		{
			var entities = await FindAll()
				.ToListAsync();
			IList<KsMenu> businessObjects = KsMenu.NewInstance(entities);
			return businessObjects;
		}

		public async Task<KsMenu?> GetByPrimaryKeyAsync(int MenuNo)
		{
			KsMenuEntity? entity = await FindByCondition(e => e.MenuNo.Equals(MenuNo))
				.FirstOrDefaultAsync();
			KsMenu? businessObject = (entity != null) ? KsMenu.NewInstance(entity) : null;
			return businessObject;
		}

		public void Insert(KsMenu businessObject)
		{
			Insert(businessObject.Entity);
		}

		public void Update(KsMenu businessObject)
		{
			Update(businessObject.Entity);
		}

		public void Delete(KsMenu businessObject)
		{
			Delete(businessObject.Entity);
		}
	}
}
