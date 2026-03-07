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
using Koretech.Domains.KsPages.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Koretech.Domains.KsPages.Repositories
{
	public class KsPageContext : DbContext
	{
		public KsPageContext() { }
		
		public KsPageContext(DbContextOptions<KsPageContext> options) : base(options) { }

		public virtual DbSet<KsPageEntity> KsPages { get; set; }
		public virtual DbSet<KsPageObjectEntity> KsPageObjects { get; set; }
		public virtual DbSet<KsPageSecurityEntity> KsPageSecuritys { get; set; }
		public virtual DbSet<KsPageParameterEntity> KsPageParameters { get; set; }
		public virtual DbSet<KsPageContentEntity> KsPageContents { get; set; }
		public virtual DbSet<KsContentEntity> KsContents { get; set; }
		public virtual DbSet<KsModuleEntity> KsModules { get; set; }
		public virtual DbSet<KsPageModuleEntity> KsPageModules { get; set; }
		public virtual DbSet<KsPageModuleSecurityEntity> KsPageModuleSecuritys { get; set; }
		public virtual DbSet<KsPageModuleSettingEntity> KsPageModuleSettings { get; set; }

		// Scope Function
		public IQueryable<KsPageEntity> KsPageEntityScope(string userId, string objectId, string methodName, int? scopeOverride)
			=> FromExpression(() => KsPageEntityScope(userId, objectId, methodName, scopeOverride));

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Configure entity types
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(KsPageEntity).Assembly);

			modelBuilder.HasDefaultSchema("ks");
			modelBuilder.HasDbFunction(typeof(KsPageContext).GetMethod(nameof(KsPageEntityScope))!)
				.HasName("kssf_scope_all");

			// Relation 'KsPageContents' from KsPage to KsPageContent in this domain
			// Cardinality: to many
			modelBuilder.Entity<KsPageEntity>()
				.HasMany(entity => entity.KsPageContents)
				.WithOne()
				.HasForeignKey(target => target.PageName);

			// Relation 'KsPageObjects' from KsPage to KsPageObject in this domain
			// Cardinality: to many
			modelBuilder.Entity<KsPageEntity>()
				.HasMany(entity => entity.KsPageObjects)
				.WithOne()
				.HasForeignKey(target => target.PageName);

			// Relation 'KsPageParameters' from KsPage to KsPageParameter in this domain
			// Cardinality: to many
			modelBuilder.Entity<KsPageEntity>()
				.HasMany(entity => entity.KsPageParameters)
				.WithOne()
				.HasForeignKey(target => target.PageName);

			// Relation 'KsPageSecuritys' from KsPage to KsPageSecurity in this domain
			// Cardinality: to many
			modelBuilder.Entity<KsPageEntity>()
				.HasMany(entity => entity.KsPageSecuritys)
				.WithOne()
				.HasForeignKey(target => target.PageName);

			// Relation 'KsPages' from KsPageObject to KsPage in this domain
			// Cardinality: to one (owner)
			modelBuilder.Entity<KsPageObjectEntity>()
				.HasOne(entity => entity.KsPages)
				.WithMany()
				.HasForeignKey(target => target.PageName);

			// Relation 'KsPage' from KsPageSecurity to KsPage in this domain
			// Cardinality: to one (owner)
			modelBuilder.Entity<KsPageSecurityEntity>()
				.HasOne(entity => entity.KsPage)
				.WithMany()
				.HasForeignKey(target => target.PageName);

			// Relation 'KsPage' from KsPageParameter to KsPage in this domain
			// Cardinality: to one (owner)
			modelBuilder.Entity<KsPageParameterEntity>()
				.HasOne(entity => entity.KsPage)
				.WithMany()
				.HasForeignKey(target => target.PageName);

			// Relation 'KsPage' from KsPageContent to KsPage in this domain
			// Cardinality: to one (owner)
			modelBuilder.Entity<KsPageContentEntity>()
				.HasOne(entity => entity.KsPage)
				.WithMany()
				.HasForeignKey(target => target.PageName);

			// Relation 'Page' from KsPageModule to KsPage in this domain
			// Cardinality: to one (owner)
			modelBuilder.Entity<KsPageModuleEntity>()
				.HasOne(entity => entity.Page)
				.WithMany()
				.HasForeignKey(target => target.PageName);

			// Relation 'Module' from KsPageModule to KsModule in this domain
			// Cardinality: to one
			modelBuilder.Entity<KsPageModuleEntity>()
				.HasOne(entity => entity.Module)
				.WithMany()
				.HasForeignKey(target => target.ModuleNo);

			// Relation 'PageModule' from KsPageModuleSecurity to KsPageModule in this domain
			// Cardinality: to one (owner)
			modelBuilder.Entity<KsPageModuleSecurityEntity>()
				.HasOne(entity => entity.PageModule)
				.WithMany()
				.HasForeignKey(target => target.PageModuleNo);

			// Relation 'Role' from KsPageModuleSecurity to KsRoleUser in KsRole domain
			// Cardinality: to one
			modelBuilder.Entity<KsPageModuleSecurityEntity>()
				.HasOne(entity => entity.Role)
				.WithMany()
				.HasForeignKey(target => target.RoleNo);

			// Relation 'PageModule' from KsPageModuleSetting to KsPageModule in this domain
			// Cardinality: to one (owner)
			modelBuilder.Entity<KsPageModuleSettingEntity>()
				.HasOne(entity => entity.PageModule)
				.WithMany()
				.HasForeignKey(target => target.PageModuleNo);
		}
	}
}
