//
// Created by Kraken KAML BO Generator
//

using Koretech.Kraken.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Koretech.Kraken.Data.Configuration
{
	public class KsUserLoginFailureEntityTypeConfiguration : IEntityTypeConfiguration<KsUserLoginFailureEntity>
	{
		public void Configure(EntityTypeBuilder<KsUserLoginFailureEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsUserLoginFailureEntity>("ks_user_login_failure", "ks");

			typeBuilder.HasKey(e => e.KsUserLoginFailureId)
				.HasName("ks_user_login_failure_PK");

			typeBuilder.Property(e => e.KsUserId)
				.HasMaxLength(60)
				.IsUnicode(false)
				.HasColumnName(ks_user_id);

			typeBuilder.Property(e => e.FailDt)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName(fail_dt);

		}
	}
}
