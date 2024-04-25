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

using Koretech.Domains.KsPages.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Koretech.Domains.KsPages.EntityConfigurations
{
	public class KsPageParameterEntityTypeConfiguration : IEntityTypeConfiguration<KsPageParameterEntity>
	{
		public void Configure(EntityTypeBuilder<KsPageParameterEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsPageParameterEntity>("ks_page_parameter", "ks");

			typeBuilder.HasKey(e => new { e.PageName, e.ParameterName })
				.HasName("ks_page_parameter_PK");

			typeBuilder.Property(e => e.PageName)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("page_name");

			typeBuilder.Property(e => e.ParameterName)
				.HasMaxLength(75)
				.IsUnicode(false)
				.HasColumnName("parameter_name");

			typeBuilder.Property(e => e.ParameterType)
				.HasMaxLength(256)
				.IsUnicode(false)
				.HasColumnName("parameter_type");

		}
	}
}
