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

using Koretech.Domains.KsMenus.Entities;
using Koretech.Domains.KsMenus.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Koretech.Domains.KsMenus.Repositories
{
	public class KsMenuContext : DbContext
	{
		public KsMenuContext() { }
		
		public KsMenuContext(DbContextOptions<KsMenuContext> options) : base(options) { }

		public virtual DbSet<KsMenuEntity> KsMenus { get; set; }
		public virtual DbSet<KsMenuItemEntity> KsMenuItems { get; set; }

		#region Scope Functions

		public IQueryable<KsMenuEntity> KsMenuEntityScope(string userId, string objectId, string methodName, int? scopeOverride)
			=> FromExpression(() => KsMenuEntityScope(userId, objectId, methodName, scopeOverride));

		#endregion Scope Functions

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			new KsMenuEntityTypeConfiguration().Configure(modelBuilder.Entity<KsMenuEntity>());
			new KsMenuItemEntityTypeConfiguration().Configure(modelBuilder.Entity<KsMenuItemEntity>());

			modelBuilder.HasDefaultSchema("ks");
			modelBuilder.HasDbFunction(typeof(KsMenuContext).GetMethod(nameof(KsMenuEntityScope))!)
				.HasName("kssf_scope_all");
		}
	}
}
