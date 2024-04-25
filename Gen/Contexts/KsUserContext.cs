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

using Koretech.Domains.KsUsers.Entities;
using Koretech.Domains.KsUsers.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Koretech.Domains.KsUsers.Repositories
{
	public class KsUserContext : DbContext
	{
		public KsUserContext() { }
		
		public KsUserContext(DbContextOptions<KsUserContext> options) : base(options) { }

		public virtual DbSet<KsUserEntity> KsUsers { get; set; }
		public virtual DbSet<KsUserLoginFailureEntity> KsUserLoginFailures { get; set; }
		public virtual DbSet<KsUserRoleEntity> KsUserRoles { get; set; }
		public virtual DbSet<PasswordHistoryEntity> PasswordHistorys { get; set; }
		public virtual DbSet<KsUserTokenEntity> KsUserTokens { get; set; }

		#region Scope Functions

		public IQueryable<KsUserEntity> KsUserEntityScope(string userId, string objectId, string methodName, int? scopeOverride)
			=> FromExpression(() => KsUserEntityScope(userId, objectId, methodName, scopeOverride));

		#endregion Scope Functions

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			new KsUserEntityTypeConfiguration().Configure(modelBuilder.Entity<KsUserEntity>());
			new KsUserLoginFailureEntityTypeConfiguration().Configure(modelBuilder.Entity<KsUserLoginFailureEntity>());
			new KsUserRoleEntityTypeConfiguration().Configure(modelBuilder.Entity<KsUserRoleEntity>());
			new PasswordHistoryEntityTypeConfiguration().Configure(modelBuilder.Entity<PasswordHistoryEntity>());
			new KsUserTokenEntityTypeConfiguration().Configure(modelBuilder.Entity<KsUserTokenEntity>());

			modelBuilder.HasDefaultSchema("ks");
			modelBuilder.HasDbFunction(typeof(KsUserContext).GetMethod(nameof(KsUserEntityScope))!)
				.HasName("kssf_scope_ksuser");
		}
	}
}
