//
// Created by Kraken KAML BO Generator
//

using Koretech.Kraken.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Koretech.Kraken.Data.Configuration
{
	public class KsUserRoleEntityTypeConfiguration : IEntityTypeConfiguration<KsUserRoleEntity>
	{
		public void Configure(EntityTypeBuilder<KsUserRoleEntity> typeBuilder)
		{
			typeBuilder
				.ToTable<KsUserRoleEntity>("ks_user_role", "ks");

			typeBuilder.HasKey(e => e.KsUserRoleId)
				.HasName("ks_user_role_PK");

			typeBuilder.Property(e => e.KsUserId)
				.HasMaxLength(60)
				.IsUnicode(false)
				.HasColumnName(ks_user_id);

			typeBuilder.Property(e => e.ResourceType)
				.HasMaxLength(20)
				.IsUnicode(false)
				.HasColumnName(resource_type);

			typeBuilder.Property(e => e.ResourceName)
				.HasMaxLength(20)
				.IsUnicode(false)
				.HasColumnName(resource_name);

			typeBuilder.Property(e => e.RoleNo)
				.HasColumnName(role_no);

		}
	}
}
