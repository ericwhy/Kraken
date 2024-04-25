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
using System.Collections;
using Koretech.Domains.KsRoles.BusinessObjects;


namespace Koretech.Domains.KsUsers.BusinessObjects
{
	/// <summary>
	/// This business object class wraps the domain entity KsUserRoleEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public partial class KsUserRole : KsUserRoleBase
	{
		#region Static Methods

		/// <summary>
		/// Creates a business object from an entity.  Any child entities
		/// will also have business objects created for them and attached to the parent business
		/// object.  This will result in a business object tree that mirrors the entity tree.
		/// </summary>
		/// <param name="entity">The entity to create a business object from</param>
		/// <returns>A newly created business object wrapping the provided entity</returns>
		public static KsUserRole NewInstance(KsUserRoleEntity entity)
		{
			KsUserRole businessObject = new(entity);

			// Recursively create business objects from the entities that have relationships with this one
			// and link to them through the relationship properties in this class.

			if (entity.User != null)
			{
				IList<KsUser> newKsUser = BusinessObjects.KsUser.NewInstance(entity.User);
				businessObject.User.AddRange(newKsUser);
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
		public static IList<KsUserRole> NewInstance(IList<KsUserRoleEntity> entities)
		{
			List<KsUserRole> businessObjects = new();

			foreach (KsUserRoleEntity entity in entities)
			{
				KsUserRole newBusinessObject = new(entity);
				businessObjects.Add(newBusinessObject);

				// Recursively create business objects from the entities that have relationships with this one
				// and link to them through the relationship properties in this class.

				if (entity.User != null)
				{
					IList<KsUser> newKsUser = BusinessObjects.KsUser.NewInstance(entity.User);
					newBusinessObject.User.AddRange(newKsUser);
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
		protected KsUserRole(KsUserRoleEntity entity) : base(entity)
		{
			Initialize();
		}
	}
}
