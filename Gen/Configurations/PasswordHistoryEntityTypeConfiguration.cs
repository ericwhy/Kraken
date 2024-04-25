/********************************************************/
/*                                                      */
/* Created by Kraken KAML BO Generator                  */
/*                                                      */
/* DO NOT MODIFY                                        */
/*                                                      */
/* Extensions or overrides should be placed in a        */
/* subclass or partial class, whichever is appropriate. */
/*                                                      */
/********************************************************/

using Koretech.Domains.KsUsers.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Koretech.Domains.KsUsers.EntityConfigurations
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
