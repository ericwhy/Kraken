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
using Koretech.Domains.KsPages.BusinessObjects;
using Koretech.Domains.KsPages.Entities;
using Microsoft.EntityFrameworkCore;

namespace Koretech.Domains.KsPages.Repositories
{
	internal partial class KsPageRepository : Repository<KsPageEntity>
	{
		public KsPageRepository(KsPageContext dbContext) : base(dbContext) { }

		public async Task<IEnumerable<KsPage>> GetAllAsync()
		{
			var entities = await FindAll()
				.ToListAsync();
			IList<KsPage> businessObjects = KsPage.NewInstance(entities);
			return businessObjects;
		}

		public async Task<KsPage?> GetByPrimaryKeyAsync(string PageName)
		{
			KsPageEntity? entity = await FindByCondition(e => e.PageName.Equals(PageName))
				.FirstOrDefaultAsync();
			KsPage? businessObject = (entity != null) ? KsPage.NewInstance(entity) : null;
			return businessObject;
		}

		public void Insert(KsPage businessObject)
		{
			Insert(businessObject.Entity);
		}

		public void Update(KsPage businessObject)
		{
			Update(businessObject.Entity);
		}

		public void Delete(KsPage businessObject)
		{
			Delete(businessObject.Entity);
		}
	}
}
