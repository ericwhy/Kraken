//
// Created by Kraken KAML BO Generator
//

using Koretech.Kraken.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Koretech.Kraken.Data.Configuration
{
	public class KsUserTokenEntityTypeConfiguration : IEntityTypeConfiguration<KsUserTokenEntity>
	{
		public void Configure(EntityTypeBuilder<KsUserTokenEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsUserTokenEntity>("ks_user_token", "ks");

			typeBuilder.HasKey(e => e.KsUserTokenId)
				.HasName("ks_user_token_PK");

			typeBuilder.Property(e => e.TokenNo)
				.HasColumnName(token_no);

			typeBuilder.Property(e => e.Selector)
				.HasColumnType("uniqueidentifier")
				.HasColumnName(selector);

			typeBuilder.Property(e => e.ValidatorHash)
				.HasColumnType("varbinary")
				.HasMaxLength(32)
				.HasColumnName(validator_hash);

			typeBuilder.Property(e => e.KsUserId)
				.HasMaxLength(60)
				.IsUnicode(false)
				.HasColumnName(ks_user_id);

			typeBuilder.Property(e => e.ExpirationDt)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName(expiration_dt);

		}
	}
}
