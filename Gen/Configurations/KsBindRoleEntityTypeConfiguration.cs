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

using Koretech.Domains.KsRoles.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Koretech.Domains.KsRoles.EntityConfigurations
{
	public class KsBindRoleEntityTypeConfiguration : IEntityTypeConfiguration<KsBindRoleEntity>
	{
		public void Configure(EntityTypeBuilder<KsBindRoleEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsBindRoleEntity>("ks_bind_role", "ks");

			typeBuilder.HasKey(e => new { e.RoleNo })
				.HasName("ks_bind_role_PK");

			typeBuilder.Property(e => e.RoleNo)
				.HasColumnName("role_no");

			typeBuilder.Property(e => e.IfColumnName)
				.HasMaxLength(30)
				.IsUnicode(false)
				.HasColumnName("if_column_name");

			typeBuilder.Property(e => e.IfAttrName)
				.HasMaxLength(30)
				.IsUnicode(false)
				.HasColumnName("if_attr_name");

			typeBuilder.Property(e => e.IfOperator)
				.HasMaxLength(2)
				.IsUnicode(false)
				.HasColumnName("if_operator");

			typeBuilder.Property(e => e.IfValue)
				.HasMaxLength(255)
				.IsUnicode(false)
				.HasColumnName("if_value");

		}
	}
}
