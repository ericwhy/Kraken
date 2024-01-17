using Koretech.Kraken.BusinessObjects.KsUser;
using Koretech.Kraken.Contexts;
using Koretech.Kraken.Entities.KsUser;
using Microsoft.EntityFrameworkCore;

namespace Koretech.Kraken.Repositories
{
    public class KsUserRepository : Repository<KsUserEntity>
    {
        public KsUserRepository(KsUserContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<KsUser>> GetAllAsync()
        {
            var entities = await FindAll()
                .OrderBy(ksUserEntity => ksUserEntity.KsUserId)
                .ToListAsync();
            IList<KsUser> businessObjects = KsUser.NewInstance(entities);
            return businessObjects;
        }

        public async Task<KsUser?> GetByKsUserIdAsync(string ksUserId)
        {
            KsUserEntity? entity = await FindByCondition(ksUserEntity => ksUserEntity.KsUserId.Equals(ksUserId))
                .FirstOrDefaultAsync();
            KsUser? businessObject = (entity != null) ? KsUser.NewInstance(entity) : null;
            return businessObject;  
        }

        public void Insert(KsUser ksUser)
        {
            Insert(ksUser.Entity);
        }

        public void Update(KsUser ksUser)
        {
            Update(ksUser.Entity);
        }

        public void Delete(KsUser ksUser)
        {
            Delete(ksUser.Entity);
        }
    }
}
