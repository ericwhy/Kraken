//
// Created by Kraken KAML BO Generator
//

using Koretech.Kraken.Data.Configurations;
using Koretech.Kraken.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace Koretech.Kraken.Data.Contexts
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

		public IQueryable<KsUserEntity> KsUserEntityScope(string userId, string objectId, string methodName, int? scopeOverride)
            => FromExpression(() => KsUserEntityScope(userId, objectId, methodName, scopeOverride));

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			new KsUserEntityTypeConfiguration().Configure(modelBuilder.Entity<KsUserEntity>());
			new KsUserLoginFailureEntityTypeConfiguration().Configure(modelBuilder.Entity<KsUserLoginFailureEntity>());
			new KsUserRoleEntityTypeConfiguration().Configure(modelBuilder.Entity<KsUserRoleEntity>());
			new PasswordHistoryEntityTypeConfiguration().Configure(modelBuilder.Entity<PasswordHistoryEntity>());
			new KsUserTokenEntityTypeConfiguration().Configure(modelBuilder.Entity<KsUserTokenEntity>());

            modelBuilder.HasDbFunction(typeof(KsUserContext).GetMethod(nameof(KsUserEntityScope)))
            .HasName("ks.kssf_scope_ksuser");
		}
	}
}
