// Added by hand.

using Koretech.Kraken.Entities.KsRole;
using Koretech.Kraken.Entities.KsUser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Koretech.Kraken.Data.Configurations
{
	public class KsRoleUserEntityTypeConfiguration : IEntityTypeConfiguration<KsRoleUserEntity>
	{
		public void Configure(EntityTypeBuilder<KsRoleUserEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsRoleUserEntity>("ks_role_user", "ks");

			typeBuilder.HasKey(e => new { e.KsUserId, e.FailDt })
				.HasName("ks_role_user_PK");

			typeBuilder.Property(e => e.KsUserId)
				.HasMaxLength(60)
				.IsUnicode(false)
				.HasColumnName("ks_user_id");

            typeBuilder.Property(e => e.FailDt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fail_dt");

        }
    }
}
