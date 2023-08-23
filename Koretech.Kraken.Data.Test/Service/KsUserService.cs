using Koretech.Kraken.BusinessObjects.KsUser;
using Koretech.Kraken.Repositories;

namespace Koretech.Kraken.Service
{
    public class KsUserService : IKsUserService
    {
        private readonly KsUserRepository _repository;

        public KsUserService(KsUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<KsUser>> GetAllAsync()
        {
            IEnumerable<KsUser> users = await _repository.GetAllAsync();
            return users;
        }

        public async Task<KsUser> GetByKsUserIdAsync(string ksUserId)
        {
            KsUser? user = await _repository.GetByKsUserIdAsync(ksUserId);
            if (user is null)
            {
                throw new KsUserNotFoundException(ksUserId);
            }
            return user;
        }

        public async Task<KsUser> InsertAsync(KsUser user) 
        {
            _repository.Insert(user);
            await _repository.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(KsUser user) 
        {
            _repository.Update(user);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(KsUser user) 
        {
            _repository.Delete(user);
            await _repository.SaveChangesAsync();
        }
    }
}
