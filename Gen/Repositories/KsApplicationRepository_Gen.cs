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
using Koretech.Domains.KsApplications.BusinessObjects;
using Koretech.Domains.KsApplications.Entities;
using Microsoft.EntityFrameworkCore;

namespace Koretech.Domains.KsApplications.Repositories
{
	internal partial class KsApplicationRepository : Repository<KsApplicationEntity>
	{
		public KsApplicationRepository(KsApplicationContext dbContext) : base(dbContext) { }

		public async Task<IEnumerable<KsApplication>> GetAllAsync()
		{
			var entities = await FindAll()
				.ToListAsync();
			IList<KsApplication> businessObjects = KsApplication.NewInstance(entities);
			return businessObjects;
		}

		public async Task<KsApplication?> GetByPrimaryKeyAsync(int AppNo)
		{
			KsApplicationEntity? entity = await FindByCondition(e => e.AppNo.Equals(AppNo))
				.FirstOrDefaultAsync();
			KsApplication? businessObject = (entity != null) ? KsApplication.NewInstance(entity) : null;
			return businessObject;
		}

		public void Insert(KsApplication businessObject)
		{
			Insert(businessObject.Entity);
		}

		public void Update(KsApplication businessObject)
		{
			Update(businessObject.Entity);
		}

		public void Delete(KsApplication businessObject)
		{
			Delete(businessObject.Entity);
		}
	}
}
