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
    public class KsUserRoleEntityTypeConfiguration : IEntityTypeConfiguration<KsUserRoleEntity>
    {
        public void Configure(EntityTypeBuilder<KsUserRoleEntity> typeBuilder)
        {
            typeBuilder.HasKey(e => new { e.KsUserId, e.ResourceType, e.ResourceName, e.RoleNo }).HasName("pk_ks_user_role");

            typeBuilder.ToTable("ks_user_role");

            typeBuilder.Property(e => e.KsUserId)
                .HasMaxLength(60)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ks_user_id");
            typeBuilder.Property(e => e.ResourceType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("resource_type");
            typeBuilder.Property(e => e.ResourceName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("resource_name");
            typeBuilder.Property(e => e.RoleNo).HasColumnName("role_no");

            typeBuilder.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.KsUserId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .HasConstraintName("fk_ks_user_role_ks_user");

        }
    }
}
