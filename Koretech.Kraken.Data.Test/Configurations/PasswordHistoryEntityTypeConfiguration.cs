//
// Created by Kraken KAML BO Generator
//

using Koretech.Kraken.Entities.KsUser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Koretech.Kraken.Data.Configurations
{
	public class PasswordHistoryEntityTypeConfiguration : IEntityTypeConfiguration<PasswordHistoryEntity>
	{
		public void Configure(EntityTypeBuilder<PasswordHistoryEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<PasswordHistoryEntity>("password_history", "ks");

			typeBuilder.HasKey(e => new { e.KsUserId, e.Password })
				.HasName("password_history_PK");

			typeBuilder.Property(e => e.KsUserId)
				.HasMaxLength(60)
				.IsUnicode(false)
				.HasColumnName("ks_user_id");

			typeBuilder.Property(e => e.Password)
				.HasMaxLength(256)
				.IsUnicode(false)
				.HasColumnName("password");

			typeBuilder.Property(e => e.PasswordSalt)
				.HasMaxLength(256)
				.IsUnicode(false)
				.HasColumnName("password_salt");

			typeBuilder.Property(e => e.CreateDt)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("create_dt");

		}
	}
}
