using Koretech.Kraken.Data.Configurations;
using Koretech.Kraken.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace Koretech.Kraken.Data.Contexts
{
    public class KsUserContext: DbContext
    {
        public KsUserContext() { }
        public KsUserContext(DbContextOptions<KsUserContext> options) : base(options) { }

        public virtual DbSet<KsUserEntity> Users { get; set; }
        public virtual DbSet<KsUserLoginFailureEntity> LoginFailures { get; set; }
        public virtual DbSet<KsUserRoleEntity> UserRoles { get; set; }
        public virtual DbSet<KsUserTokenEntity> UserTokens { get; set; }
        public virtual DbSet<PasswordHistoryEntity> PasswordHistory { get; set; }

        #region Scope functions
        
        public IQueryable<KsUserEntity> KsUserEntityScope(string userId, string objectId, string methodName, int? scopeOverride)
            => FromExpression(() => KsUserEntityScope(userId, objectId, methodName, scopeOverride));
        
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new KsUserEntityTypeConfiguration().Configure(modelBuilder.Entity<KsUserEntity>());
            new KsUserLoginFailureEntityTypeConfiguration().Configure(modelBuilder.Entity<KsUserLoginFailureEntity>());
            new KsUserRoleEntityTypeConfiguration().Configure(modelBuilder.Entity<KsUserRoleEntity>());
            new KsUserTokenEntityTypeConfiguration().Configure(modelBuilder.Entity<KsUserTokenEntity>());
            new PasswordHistoryEntityTypeConfiguration().Configure(modelBuilder.Entity<PasswordHistoryEntity>());

            modelBuilder.HasDbFunction(typeof(KsUserContext).GetMethod(nameof(KsUserEntityScope)))
            .HasName("ks.kssf_scope_ksuser");
        }
    }
}
