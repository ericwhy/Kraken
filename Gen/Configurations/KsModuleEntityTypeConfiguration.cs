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
	public class KsModuleEntityTypeConfiguration : IEntityTypeConfiguration<KsModuleEntity>
	{
		public void Configure(EntityTypeBuilder<KsModuleEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsModuleEntity>("ks_module", "ks");

			typeBuilder.HasKey(e => new { e.ModuleNo })
				.HasName("ks_module_PK");

			typeBuilder.Property(e => e.ModuleNo)
				.HasColumnName("module_no");

			typeBuilder.Property(e => e.Title)
				.HasMaxLength(128)
				.IsUnicode(false)
				.HasColumnName("title");

		}
	}
}
