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
using System.Collections;
using Koretech.Domains.KsUsers.BusinessObjects;
using Koretech.Domains.KsObjects.BusinessObjects;
using Koretech.Domains.KsPages.BusinessObjects;


namespace Koretech.Domains.KsRoles.BusinessObjects
{
	/// <summary>
	/// This business object class wraps the domain entity KsRoleUserEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public partial class KsRoleUser : KsRoleUserBase
	{
		#region Static Methods

		/// <summary>
		/// Creates a business object from an entity.  Any child entities
		/// will also have business objects created for them and attached to the parent business
		/// object.  This will result in a business object tree that mirrors the entity tree.
		/// </summary>
		/// <param name="entity">The entity to create a business object from</param>
		/// <returns>A newly created business object wrapping the provided entity</returns>
		public static KsRoleUser NewInstance(KsRoleUserEntity entity)
		{
			KsRoleUser businessObject = new(entity);

			// Recursively create business objects from the entities that have relationships with this one
			// and link to them through the relationship properties in this class.

			if (entity.KsBindRoles != null)
			{
				IList<KsBindRole> newKsBindRole = BusinessObjects.KsBindRole.NewInstance(entity.KsBindRoles);
				businessObject.KsBindRoles.AddRange(newKsBindRole);
			}

			if (entity.KsUsers != null)
			{
				IList<KsUserRole> newKsUserRole = KsUserRole.NewInstance(entity.KsUsers);
				businessObject.KsUsers.AddRange(newKsUserRole);
			}

			if (entity.KsObjectMethodSecurities != null)
			{
				IList<KsObjectMethodSecurity> newKsObjectMethodSecurity = KsObjectMethodSecurity.NewInstance(entity.KsObjectMethodSecurities);
				businessObject.KsObjectMethodSecurities.AddRange(newKsObjectMethodSecurity);
			}

			if (entity.KsObjectPropertySecurities != null)
			{
				IList<KsObjectPropertySecurity> newKsObjectPropertySecurity = KsObjectPropertySecurity.NewInstance(entity.KsObjectPropertySecurities);
				businessObject.KsObjectPropertySecurities.AddRange(newKsObjectPropertySecurity);
			}

			if (entity.KsPageSecurities != null)
			{
				IList<KsPageSecurity> newKsPageSecurity = KsPageSecurity.NewInstance(entity.KsPageSecurities);
				businessObject.KsPageSecurities.AddRange(newKsPageSecurity);
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
		public static IList<KsRoleUser> NewInstance(IList<KsRoleUserEntity> entities)
		{
			List<KsRoleUser> businessObjects = new();

			foreach (KsRoleUserEntity entity in entities)
			{
				KsRoleUser newBusinessObject = new(entity);
				businessObjects.Add(newBusinessObject);

				// Recursively create business objects from the entities that have relationships with this one
				// and link to them through the relationship properties in this class.

				if (entity.KsBindRoles != null)
				{
					IList<KsBindRole> newKsBindRole = BusinessObjects.KsBindRole.NewInstance(entity.KsBindRoles);
					newBusinessObject.KsBindRoles.AddRange(newKsBindRole);
				}

				if (entity.KsUsers != null)
				{
					IList<KsUserRole> newKsUserRole = KsUserRole.NewInstance(entity.KsUsers);
					newBusinessObject.KsUsers.AddRange(newKsUserRole);
				}

				if (entity.KsObjectMethodSecurities != null)
				{
					IList<KsObjectMethodSecurity> newKsObjectMethodSecurity = KsObjectMethodSecurity.NewInstance(entity.KsObjectMethodSecurities);
					newBusinessObject.KsObjectMethodSecurities.AddRange(newKsObjectMethodSecurity);
				}

				if (entity.KsObjectPropertySecurities != null)
				{
					IList<KsObjectPropertySecurity> newKsObjectPropertySecurity = KsObjectPropertySecurity.NewInstance(entity.KsObjectPropertySecurities);
					newBusinessObject.KsObjectPropertySecurities.AddRange(newKsObjectPropertySecurity);
				}

				if (entity.KsPageSecurities != null)
				{
					IList<KsPageSecurity> newKsPageSecurity = KsPageSecurity.NewInstance(entity.KsPageSecurities);
					newBusinessObject.KsPageSecurities.AddRange(newKsPageSecurity);
				}
			}

			return businessObjects;
		}

		#endregion Static Methods

		/// <summary>
		/// Create a new instance of this class by wrapping an entity.
		/// </summary>
		/// <param name="entity">the entity to wrap</param>
		protected KsRoleUser(KsRoleUserEntity entity) : base(entity)
		{
			Initialize();
		}
	}
}
