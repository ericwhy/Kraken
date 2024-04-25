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
	public class KsControlEntityTypeConfiguration : IEntityTypeConfiguration<KsControlEntity>
	{
		public void Configure(EntityTypeBuilder<KsControlEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsControlEntity>("ks_control", "ks");

			typeBuilder.HasKey(e => new { e.CategoryCd, e.KeySeq })
				.HasName("ks_control_PK");

			typeBuilder.Property(e => e.CategoryCd)
				.HasColumnName("category_cd");

			typeBuilder.Property(e => e.KeySeq)
				.HasColumnName("key_seq");

			typeBuilder.Property(e => e.KeyIntValue)
				.HasColumnName("key_int_value");

			typeBuilder.Property(e => e.KeyStrValue)
				.HasMaxLength(4000)
				.IsUnicode(false)
				.HasColumnName("key_str_value");

			typeBuilder.Property(e => e.KeyDateValue)
				.HasDefaultValueSql("(getdate())")
				.HasColumnType("datetime")
				.HasColumnName("key_date_value");

			typeBuilder.Property(e => e.KeyType)
				.HasMaxLength(1)
				.IsUnicode(false)
				.HasColumnName("key_type");

			typeBuilder.Property(e => e.KeyDesc)
				.HasMaxLength(30)
				.IsUnicode(false)
				.HasColumnName("key_desc");

		}
	}
}
