using Koretech.Kraken.BusinessObjects.KsUser;

namespace Koretech.Kraken.Service
{
    public interface IKsUserService
    {
        public Task<IEnumerable<KsUser>> GetAllAsync();

        public Task<KsUser> GetByKsUserIdAsync(string ksUserId);

        public Task<KsUser> InsertAsync(KsUser user);

        public Task UpdateAsync(KsUser user);

        public Task DeleteAsync(KsUser user);
    }
}
