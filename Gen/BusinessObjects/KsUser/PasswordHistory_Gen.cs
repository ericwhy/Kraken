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

using Koretech.Infrastructure.Services.KsUser.Entities;
using System.Collections;

namespace Koretech.Infrastructure.Services.KsUser.BusinessObjects
{
	/// <summary>
	/// This business object class wraps the domain entity PasswordHistoryEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public partial class PasswordHistory : PasswordHistoryBase
	{
		#region Static Methods

		/// <summary>
		/// Creates a business object from an entity.  Any child entities
		/// will also have business objects created for them and attached to the parent business
		/// object.  This will result in a business object tree that mirrors the entity tree.
		/// </summary>
		/// <param name="entity">The entity to create a business object from</param>
		/// <returns>A newly created business object wrapping the provided entity</returns>
		public static PasswordHistory NewInstance(PasswordHistoryEntity entity)
		{
			PasswordHistory businessObject = new(entity);

			// Recursively create business objects from the entities that have relationships with this one
			// and link to them through the relationship properties in this class.

			IList<KsUser> newKsUser = Koretech.Infrastructure.Services.KsUser.BusinessObjects.KsUser.NewInstance(entity.KsUser);
			businessObject.KsUser.AddRange(newKsUser);

			return businessObject;
		}

		/// <summary>
		/// Creates business objects from a set of entities by wrapping each entity.  Any child entities
		/// will also have business objects created for them and attached to the appropriate parent business
		/// object.  This will result in a business object tree that mirrors the entity tree.
		/// </summary>
		/// <param name="entities">The entities to create business objects from</param>
		/// <returns>A newly created business object(s) wrapping the provided entities</returns>
		public static IList<PasswordHistory> NewInstance(IList<PasswordHistoryEntity> entities)
		{
			List<PasswordHistory> businessObjects = new();

			foreach (PasswordHistoryEntity entity in entities)
			{
				PasswordHistory newBusinessObject = new(entity);
				businessObjects.Add(newBusinessObject);

				// Recursively create business objects from the entities that have relationships with this one
				// and link to them through the relationship properties in this class.

				IList<KsUser> newKsUser = Koretech.Infrastructure.Services.KsUser.BusinessObjects.KsUser.NewInstance(entity.KsUser);
				newBusinessObject.KsUser.AddRange(newKsUser);
			}

			return businessObjects;
		}

		#endregion Static Methods
	}
}
