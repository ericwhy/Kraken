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

using Koretech.Domains.KsApplications.Entities;
using Koretech.Domains.KsApplications.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Koretech.Domains.KsApplications.Repositories
{
	public class KsApplicationContext : DbContext
	{
		public KsApplicationContext() { }
		
		public KsApplicationContext(DbContextOptions<KsApplicationContext> options) : base(options) { }

		public virtual DbSet<KsApplicationEntity> KsApplications { get; set; }
		public virtual DbSet<KsControlEntity> KsControls { get; set; }

		#region Scope Functions

		public IQueryable<KsApplicationEntity> KsApplicationEntityScope(string userId, string objectId, string methodName, int? scopeOverride)
			=> FromExpression(() => KsApplicationEntityScope(userId, objectId, methodName, scopeOverride));

		#endregion Scope Functions

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			new KsApplicationEntityTypeConfiguration().Configure(modelBuilder.Entity<KsApplicationEntity>());
			new KsControlEntityTypeConfiguration().Configure(modelBuilder.Entity<KsControlEntity>());

			modelBuilder.HasDefaultSchema("ks");
			modelBuilder.HasDbFunction(typeof(KsApplicationContext).GetMethod(nameof(KsApplicationEntityScope))!)
				.HasName("kssf_scope_all");
		}
	}
}
