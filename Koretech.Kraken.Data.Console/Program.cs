using Koretech.Kraken.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Koretech.Kraken
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var contextOptions = new DbContextOptionsBuilder<KsUserContext>()
                .UseSqlServer("Data Source=(local);Initial Catalog=kommerceserver_master;User ID=ks;Password=ndc!ut;TrustServerCertificate=True;Max Pool Size=1000;MultipleActiveResultSets=True;Application Name=KommerceServer.Net.App+;Enlist=False")
                .Options;

            var context = new KsUserContext(contextOptions);
            var users = (from u in context.Users
                        join s in context.KsUserEntityScope("keithl@koretech.com", "Koretech.KommerceServer.BusinessObjects.KSUser.KsUser", "Retrieve", null)
                          on u.KsUserId equals s.KsUserId
                        select u);
                        //.FirstOrDefault(); //context.Users.Where(x=>x.DisplayName.EndsWith("Yarbrough")).FirstOrDefault();
//            user.FailedLoginDt = DateTime.Now;
            context.SaveChanges();

            foreach (var user in users)
            {
                Console.WriteLine($"Hello {user?.DisplayName} at {user?.EmailAddress}");
            }
        }
    }
}