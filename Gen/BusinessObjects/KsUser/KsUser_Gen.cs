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

namespace Koretech.Domains.KsUsers.BusinessObjects
{
	/// <summary>
	/// This business object class wraps the domain entity KsUserEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public partial class KsUser : KsUserBase
	{
		#region Static Methods

		/// <summary>
		/// Creates a business object from an entity.  Any child entities
		/// will also have business objects created for them and attached to the parent business
		/// object.  This will result in a business object tree that mirrors the entity tree.
		/// </summary>
		/// <param name="entity">The entity to create a business object from</param>
		/// <returns>A newly created business object wrapping the provided entity</returns>
		public static KsUser NewInstance(KsUserEntity entity)
		{
			KsUser businessObject = new(entity);

			// Recursively create business objects from the entities that have relationships with this one
			// and link to them through the relationship properties in this class.

			if (entity.LoginFailures != null)
			{
				IList<KsUserLoginFailure> newKsUserLoginFailure = BusinessObjects.KsUserLoginFailure.NewInstance(entity.LoginFailures);
				businessObject.LoginFailures.AddRange(newKsUserLoginFailure);
			}

			if (entity.PasswordHistory != null)
			{
				IList<PasswordHistory> newPasswordHistory = BusinessObjects.PasswordHistory.NewInstance(entity.PasswordHistory);
				businessObject.PasswordHistory.AddRange(newPasswordHistory);
			}

			if (entity.UserRoles != null)
			{
				IList<KsUserRole> newKsUserRole = BusinessObjects.KsUserRole.NewInstance(entity.UserRoles);
				businessObject.UserRoles.AddRange(newKsUserRole);
			}

			if (entity.UserToken != null)
			{
				businessObject.UserToken = BusinessObjects.KsUserToken.NewInstance(entity.UserToken);
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
		public static IList<KsUser> NewInstance(IList<KsUserEntity> entities)
		{
			List<KsUser> businessObjects = new();

			foreach (KsUserEntity entity in entities)
			{
				KsUser newBusinessObject = new(entity);
				businessObjects.Add(newBusinessObject);

				// Recursively create business objects from the entities that have relationships with this one
				// and link to them through the relationship properties in this class.

				if (entity.LoginFailures != null)
				{
					IList<KsUserLoginFailure> newKsUserLoginFailure = BusinessObjects.KsUserLoginFailure.NewInstance(entity.LoginFailures);
					newBusinessObject.LoginFailures.AddRange(newKsUserLoginFailure);
				}

				if (entity.PasswordHistory != null)
				{
					IList<PasswordHistory> newPasswordHistory = BusinessObjects.PasswordHistory.NewInstance(entity.PasswordHistory);
					newBusinessObject.PasswordHistory.AddRange(newPasswordHistory);
				}

				if (entity.UserRoles != null)
				{
					IList<KsUserRole> newKsUserRole = BusinessObjects.KsUserRole.NewInstance(entity.UserRoles);
					newBusinessObject.UserRoles.AddRange(newKsUserRole);
				}

				if (entity.UserToken != null)
				{
					newBusinessObject.UserToken =  BusinessObjects.KsUserToken.NewInstance(entity.UserToken);
				}
			}

			return businessObjects;
		}

		#endregion Static Methods

		/// <summary>
		/// Create a new instance of this class by wrapping an entity.
		/// </summary>
		/// <param name="entity">the entity to wrap</param>
		protected KsUser(KsUserEntity entity) : base(entity)
		{
			Initialize();
		}
	}
}
