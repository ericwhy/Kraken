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
