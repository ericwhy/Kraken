﻿using Koretech.Kraken.Data.Configurations;
using Koretech.Kraken.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace Koretech.Kraken.Contexts
{
    public class KsUserContext : DbContext
    {
        public KsUserContext() { }
        public KsUserContext(DbContextOptions<KsUserContext> options) : base(options) { }

        public virtual DbSet<KsUserEntity> Users { get; set; }
        public virtual DbSet<KsUserLoginFailureEntity> LoginFailures { get; set; }
        public virtual DbSet<KsUserRoleEntity> UserRoles { get; set; }
        public virtual DbSet<KsUserTokenEntity> UserTokens { get; set; }
        public virtual DbSet<PasswordHistoryEntity> PasswordHistory { get; set; }

        #region Scope Functions
        
        public IQueryable<KsUserEntity> KsUserEntityScope(string userId, string objectId, string methodName, int? scopeOverride)
            => FromExpression(() => KsUserEntityScope(userId, objectId, methodName, scopeOverride));
        
        #endregion Scope Functions

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new KsUserEntityTypeConfiguration().Configure(modelBuilder.Entity<KsUserEntity>());
            new KsUserLoginFailureEntityTypeConfiguration().Configure(modelBuilder.Entity<KsUserLoginFailureEntity>());
            new KsUserRoleEntityTypeConfiguration().Configure(modelBuilder.Entity<KsUserRoleEntity>());
            new KsUserTokenEntityTypeConfiguration().Configure(modelBuilder.Entity<KsUserTokenEntity>());
            new PasswordHistoryEntityTypeConfiguration().Configure(modelBuilder.Entity<PasswordHistoryEntity>());

            modelBuilder.HasDefaultSchema("ks");
            modelBuilder.HasDbFunction(typeof(KsUserContext).GetMethod(nameof(KsUserEntityScope)))
                .HasName("kssf_scope_ksuser");
        }
    }
}
