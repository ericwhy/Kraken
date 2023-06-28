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
    public class KsUserLoginFailureEntityTypeConfiguration : IEntityTypeConfiguration<KsUserLoginFailureEntity>
    {
        public void Configure(EntityTypeBuilder<KsUserLoginFailureEntity> typeBuilder)
        {
            typeBuilder.HasKey(e => new { e.KsUserId, e.FailDt }).HasName("ks_user_login_failure_PK");

            typeBuilder.ToTable("ks_user_login_failure");

            typeBuilder.Property(e => e.KsUserId)
                .HasMaxLength(60)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ks_user_id");
            typeBuilder.Property(e => e.FailDt)
                .HasColumnType("datetime")
                .HasColumnName("fail_dt");

            typeBuilder.HasOne(d => d.User).WithMany(p => p.LoginFailures)
                .HasForeignKey(d => d.KsUserId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("ks_user_ks_user_login_failure_FK1");

        }
    }
}
