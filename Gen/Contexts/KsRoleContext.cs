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

using Koretech.Domains.KsRoles.Entities;
using Koretech.Domains.KsRoles.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Koretech.Domains.KsRoles.Repositories
{
	public class KsRoleContext : DbContext
	{
		public KsRoleContext() { }
		
		public KsRoleContext(DbContextOptions<KsRoleContext> options) : base(options) { }

		public virtual DbSet<KsRoleUserEntity> KsRoleUsers { get; set; }
		public virtual DbSet<KsBindRoleEntity> KsBindRoles { get; set; }

		#region Scope Functions

		public IQueryable<KsRoleUserEntity> KsRoleEntityScope(string userId, string objectId, string methodName, int? scopeOverride)
			=> FromExpression(() => KsRoleEntityScope(userId, objectId, methodName, scopeOverride));

		#endregion Scope Functions

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			new KsRoleUserEntityTypeConfiguration().Configure(modelBuilder.Entity<KsRoleUserEntity>());
			new KsBindRoleEntityTypeConfiguration().Configure(modelBuilder.Entity<KsBindRoleEntity>());

			modelBuilder.HasDefaultSchema("ks");
			modelBuilder.HasDbFunction(typeof(KsRoleContext).GetMethod(nameof(KsRoleEntityScope))!)
				.HasName("kssf_scope_all");
		}
	}
}
