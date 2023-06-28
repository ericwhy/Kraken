using Koretech.Kraken.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koretech.Kraken.Data.Configurations
{
    public class KsUserTokenEntityTypeConfiguration : IEntityTypeConfiguration<KsUserTokenEntity>
    {
        public void Configure(EntityTypeBuilder<KsUserTokenEntity> typeBuilder)
        {
            typeBuilder.HasKey(e => e.TokenNo).HasName("pk_ks_user_token");

            typeBuilder.ToTable("ks_user_token");

            typeBuilder.Property(e => e.TokenNo).HasColumnName("token_no");
            typeBuilder.Property(e => e.ExpirationDt)
                .HasColumnType("datetime")
                .HasColumnName("expiration_dt");
            typeBuilder.Property(e => e.KsUserId)
                .HasMaxLength(60)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ks_user_id");
            typeBuilder.Property(e => e.Selector).HasColumnName("selector");
            typeBuilder.Property(e => e.ValidatorHash)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("validator_hash");

            typeBuilder.HasOne(d => d.User).WithMany(p => p.UserTokens)
                .HasForeignKey(d => d.KsUserId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("ks_user_ks_user_token_FK1");
        }
    }
}
