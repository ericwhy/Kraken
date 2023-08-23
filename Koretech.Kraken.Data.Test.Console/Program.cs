using Koretech.Kraken.BusinessObjects.KsUser;
using Koretech.Kraken.Contexts;
using Koretech.Kraken.Repositories;
using Koretech.Kraken.Service;
using Microsoft.EntityFrameworkCore;

namespace Koretech.Kraken
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var contextOptions = new DbContextOptionsBuilder<KsUserContext>()
                .UseSqlServer("Data Source=(local);Initial Catalog=kommerceserver_master;User ID=ks;Password=ndc!ut;TrustServerCertificate=True;Max Pool Size=1000;MultipleActiveResultSets=True;Application Name=KommerceServer.Net.App+;Enlist=False")
                .Options;

            // Test service
            var context = new KsUserContext(contextOptions);
            KsUserRepository repository = new KsUserRepository(context);
            IKsUserService service = new KsUserService(repository);

            // READ ALL
            Console.WriteLine("READ ALL:");
            var allUsers = await service.GetAllAsync();
            foreach (var user in allUsers) 
            {
                Console.WriteLine($"  User id {user.KsUserId}, display name {user.DisplayName}");
            }
            Console.WriteLine();

            // READ BY ID
            var userById = await service.GetByKsUserIdAsync("ericy@koretech.com");
            Console.WriteLine($"READ BY ID: User id {userById.KsUserId}, failed login date {userById.FailedLoginDt}");
            Console.WriteLine();

            // UPDATE
            userById.FailedLoginDt = DateTime.Now;
            await service.UpdateAsync(userById);
            var userAfter = await service.GetByKsUserIdAsync("ericy@koretech.com");
            Console.WriteLine($"AFTER UPDATE: User id {userAfter.KsUserId}, failed login date {userAfter.FailedLoginDt}");
            Console.WriteLine();

            // DELETE & CREATE
            const string newUserId = "kevink@koretech.com";
            Console.WriteLine($"CREATE AND DELETE:");
            // First see if the record exists
            try
            {
                userById = await service.GetByKsUserIdAsync(newUserId);
            }
            catch (KsUserNotFoundException)
            {
                Console.WriteLine($"User {newUserId} does not exist (expected).");

                // CREATE it since it doesn't exist
                KsUser newUser = new();
                newUser.KsUserId = newUserId;
                newUser.DisplayName = "Kevin Keigwin";
                newUser.EmailAddress = newUserId;
                newUser.AllowAccessFlg = 'Y';
                newUser.PwresetFlg = 'Y';
                newUser = await service.InsertAsync(newUser);
                Console.WriteLine($"User {newUser.KsUserId} has been created.");

                // DELETE it
                await service.DeleteAsync(newUser);
                try
                {
                    userById = await service.GetByKsUserIdAsync(newUser.KsUserId);
                }
                catch (KsUserNotFoundException)
                {
                    Console.WriteLine($"User has been successfully deleted.");

                }

                Console.WriteLine();
            }
        }
    }
}