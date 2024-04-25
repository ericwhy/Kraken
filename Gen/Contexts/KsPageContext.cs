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

using Koretech.Domains.KsPages.Entities;
using Koretech.Domains.KsPages.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Koretech.Domains.KsPages.Repositories
{
	public class KsPageContext : DbContext
	{
		public KsPageContext() { }
		
		public KsPageContext(DbContextOptions<KsPageContext> options) : base(options) { }

		public virtual DbSet<KsPageEntity> KsPages { get; set; }
		public virtual DbSet<KsPageObjectEntity> KsPageObjects { get; set; }
		public virtual DbSet<KsPageSecurityEntity> KsPageSecuritys { get; set; }
		public virtual DbSet<KsPageParameterEntity> KsPageParameters { get; set; }
		public virtual DbSet<KsPageContentEntity> KsPageContents { get; set; }

		#region Scope Functions

		public IQueryable<KsPageEntity> KsPageEntityScope(string userId, string objectId, string methodName, int? scopeOverride)
			=> FromExpression(() => KsPageEntityScope(userId, objectId, methodName, scopeOverride));

		#endregion Scope Functions

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			new KsPageEntityTypeConfiguration().Configure(modelBuilder.Entity<KsPageEntity>());
			new KsPageObjectEntityTypeConfiguration().Configure(modelBuilder.Entity<KsPageObjectEntity>());
			new KsPageSecurityEntityTypeConfiguration().Configure(modelBuilder.Entity<KsPageSecurityEntity>());
			new KsPageParameterEntityTypeConfiguration().Configure(modelBuilder.Entity<KsPageParameterEntity>());
			new KsPageContentEntityTypeConfiguration().Configure(modelBuilder.Entity<KsPageContentEntity>());

			modelBuilder.HasDefaultSchema("ks");
			modelBuilder.HasDbFunction(typeof(KsPageContext).GetMethod(nameof(KsPageEntityScope))!)
				.HasName("kssf_scope_all");
		}
	}
}
