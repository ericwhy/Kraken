//
// Created by Kraken KAML BO Generator
//

using Koretech.Kraken.Entities.KsUser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Koretech.Kraken.Data.Configurations
{
	public class KsUserLoginFailureEntityTypeConfiguration : IEntityTypeConfiguration<KsUserLoginFailureEntity>
	{
		public void Configure(EntityTypeBuilder<KsUserLoginFailureEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsUserLoginFailureEntity>("ks_user_login_failure", "ks");

			typeBuilder.HasKey(e => new { e.KsUserId, e.FailDt })
				.HasName("ks_user_login_failure_PK");

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
