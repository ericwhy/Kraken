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

using Koretech.Domains.KsApplications.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Koretech.Domains.KsApplications.EntityConfigurations
{
	public class KsApplicationEntityTypeConfiguration : IEntityTypeConfiguration<KsApplicationEntity>
	{
		public void Configure(EntityTypeBuilder<KsApplicationEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsApplicationEntity>("ks_application", "ks");

			typeBuilder.HasKey(e => new { e.AppNo })
				.HasName("ks_application_PK");

			typeBuilder.Property(e => e.AppNo)
				.HasColumnName("app_no");

			typeBuilder.Property(e => e.AppName)
				.HasMaxLength(750)
				.IsUnicode(false)
				.HasColumnName("app_name");

			typeBuilder.Property(e => e.AppKey)
				.HasColumnType("uniqueidentifier")
				.HasColumnName("app_key");

		}
	}
}
