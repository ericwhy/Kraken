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

		// Scope Function
		public IQueryable<KsUserEntity> KsUserEntityScope(string userId, string objectId, string methodName, int? scopeOverride)
			=> FromExpression(() => KsUserEntityScope(userId, objectId, methodName, scopeOverride));

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Configure entity types
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(KsUserEntity).Assembly);

			modelBuilder.HasDefaultSchema("ks");
			modelBuilder.HasDbFunction(typeof(KsUserContext).GetMethod(nameof(KsUserEntityScope))!)
				.HasName("kssf_scope_ksuser");

			// Relation 'LoginFailures' from KsUser to KsUserLoginFailure in this domain
			// Cardinality: to many
			modelBuilder.Entity<KsUserEntity>()
				.HasMany(entity => entity.LoginFailures)
				.WithOne()
				.HasForeignKey(target => target.KsUserId);

			// Relation 'PasswordHistory' from KsUser to PasswordHistory in this domain
			// Cardinality: to many
			modelBuilder.Entity<KsUserEntity>()
				.HasMany(entity => entity.PasswordHistory)
				.WithOne()
				.HasForeignKey(target => target.KsUserId);

			// Relation 'UserRoles' from KsUser to KsUserRole in this domain
			// Cardinality: to many
			modelBuilder.Entity<KsUserEntity>()
				.HasMany(entity => entity.UserRoles)
				.WithOne()
				.HasForeignKey(target => target.KsUserId);

			// Relation 'UserToken' from KsUser to KsUserToken in this domain
			// Cardinality: to one
			modelBuilder.Entity<KsUserEntity>()
				.HasOne(entity => entity.UserToken)
				.WithMany()
				.HasForeignKey(target => target.KsUserId);

			// Relation 'User' from KsUserLoginFailure to KsUser in this domain
			// Cardinality: to many (owner)
			modelBuilder.Entity<KsUserLoginFailureEntity>()
				.HasMany(entity => entity.User)
				.WithOne()
				.HasForeignKey(target => target.KsUserId);

			// Relation 'User' from KsUserRole to KsUser in this domain
			// Cardinality: to one (owner)
			modelBuilder.Entity<KsUserRoleEntity>()
				.HasOne(entity => entity.User)
				.WithMany()
				.HasForeignKey(target => target.KsUserId);

			// Relation 'Role' from KsUserRole to KsRoleUser in KsRole domain
			// Cardinality: to one
			modelBuilder.Entity<KsUserRoleEntity>()
				.HasOne(entity => entity.Role)
				.WithMany()
				.HasForeignKey(target => target.RoleNo);

			// Relation 'KsUser' from PasswordHistory to KsUser in this domain
			// Cardinality: to many (owner)
			modelBuilder.Entity<PasswordHistoryEntity>()
				.HasMany(entity => entity.KsUser)
				.WithOne()
				.HasForeignKey(target => target.KsUserId);

			// Relation 'KsUser' from KsUserToken to KsUser in this domain
			// Cardinality: to one (owner)
			modelBuilder.Entity<KsUserTokenEntity>()
				.HasOne(entity => entity.KsUser)
				.WithMany()
				.HasForeignKey(target => target.KsUserId);
		}
	}
}
