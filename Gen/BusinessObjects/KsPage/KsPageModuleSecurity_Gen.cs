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
using System.Collections;
using Koretech.Domains.KsRoles.BusinessObjects;


namespace Koretech.Domains.KsPages.BusinessObjects
{
	/// <summary>
	/// This business object class wraps the domain entity KsPageModuleSecurityEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public partial class KsPageModuleSecurity : KsPageModuleSecurityBase
	{
		#region Static Methods

		/// <summary>
		/// Creates a business object from an entity.  Any child entities
		/// will also have business objects created for them and attached to the parent business
		/// object.  This will result in a business object tree that mirrors the entity tree.
		/// </summary>
		/// <param name="entity">The entity to create a business object from</param>
		/// <returns>A newly created business object wrapping the provided entity</returns>
		public static KsPageModuleSecurity NewInstance(KsPageModuleSecurityEntity entity)
		{
			KsPageModuleSecurity businessObject = new(entity);

			// Recursively create business objects from the entities that have relationships with this one
			// and link to them through the relationship properties in this class.

			if (entity.PageModule != null)
			{
				businessObject.PageModule = BusinessObjects.KsPageModule.NewInstance(entity.PageModule);
			}

			if (entity.Role != null)
			{
				businessObject.Role = KsRoleUser.NewInstance(entity.Role);
			}

			return businessObject;
		}

		/// <summary>
		/// Creates business objects from a set of entities by wrapping each entity.  Any child entities
		/// will also have business objects created for them and attached to the appropriate parent business
		/// object.  This will result in a business object tree that mirrors the entity tree.
		/// </summary>
		/// <param name="entities">The entities to create business objects from</param>
		/// <returns>A newly created business object(s) wrapping the provided entities</returns>
		public static IList<KsPageModuleSecurity> NewInstance(IList<KsPageModuleSecurityEntity> entities)
		{
			List<KsPageModuleSecurity> businessObjects = new();

			foreach (KsPageModuleSecurityEntity entity in entities)
			{
				KsPageModuleSecurity newBusinessObject = new(entity);
				businessObjects.Add(newBusinessObject);

				// Recursively create business objects from the entities that have relationships with this one
				// and link to them through the relationship properties in this class.

				if (entity.PageModule != null)
				{
					newBusinessObject.PageModule =  BusinessObjects.KsPageModule.NewInstance(entity.PageModule);
				}

				if (entity.Role != null)
				{
					newBusinessObject.Role =  KsRoleUser.NewInstance(entity.Role);
				}
			}

			return businessObjects;
		}

		#endregion Static Methods

		/// <summary>
		/// Create a new instance of this class by wrapping an entity.
		/// </summary>
		/// <param name="entity">the entity to wrap</param>
		protected KsPageModuleSecurity(KsPageModuleSecurityEntity entity) : base(entity)
		{
			Initialize();
		}
	}
}
