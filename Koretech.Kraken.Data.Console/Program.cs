using System;
using System.ComponentModel.DataAnnotations;
using Koretech.Kraken.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Koretech.Kraken.Data.Entity;

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
            var user = context.Users.Where(x=>x.DisplayName.EndsWith("Yarbrough")).FirstOrDefault();
            user.FailedLoginDt = DateTime.Now;
            context.SaveChanges();

            Console.WriteLine($"Hello {user?.DisplayName} at {user?.EmailAddress}");
        }
    }
}