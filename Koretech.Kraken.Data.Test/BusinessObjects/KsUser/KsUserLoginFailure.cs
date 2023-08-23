//
// Created by Kraken KAML BO Generator
//
// DO NOT MODIFY
//

using Koretech.Kraken.Entities.KsUser;
using System.Collections;

namespace Koretech.Kraken.BusinessObjects.KsUser
{
	/// <summary>
	/// This business object class wraps the domain entity KsUserLoginFailureEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public class KsUserLoginFailure
	{
		#region Static Methods

		/// <summary>
		/// Creates a business object from an entity.  Any child entities
		/// will also have business objects created for them and attached to the parent business
		/// object.  This will result in a business object tree that mirrors the entity tree.
		/// </summary>
		/// <param name="entity">The entity to create a business object from</param>
		/// <returns>A newly created business object wrapping the provided entity</returns>
		public static KsUserLoginFailure NewInstance(KsUserLoginFailureEntity entity)
		{
			KsUserLoginFailure businessObject = new(entity);

			// Recursively create business objects from the entities that have relationships with this one
			// and link to them through the relationship properties in this class.

			IList<KsUser> newKsUser = Koretech.Kraken.BusinessObjects.KsUser.KsUser.NewInstance(entity.User);
			businessObject.User.AddRange(newKsUser);

			return businessObject;
		}

		/// <summary>
		/// Creates business objects from a set of entities by wrapping each entity.  Any child entities
		/// will also have business objects created for them and attached to the appropriate parent business
		/// object.  This will result in a business object tree that mirrors the entity tree.
		/// </summary>
		/// <param name="entities">The entities to create business objects from</param>
		/// <returns>A newly created business object(s) wrapping the provided entities</returns>
		public static IList<KsUserLoginFailure> NewInstance(IList<KsUserLoginFailureEntity> entities)
		{
			List<KsUserLoginFailure> businessObjects = new();

			foreach (KsUserLoginFailureEntity entity in entities)
			{
				KsUserLoginFailure newBusinessObject = new(entity);
				businessObjects.Add(newBusinessObject);

				// Recursively create business objects from the entities that have relationships with this one
				// and link to them through the relationship properties in this class.

				IList<KsUser> newKsUser = Koretech.Kraken.BusinessObjects.KsUser.KsUser.NewInstance(entity.User);
				newBusinessObject.User.AddRange(newKsUser);
			}

			return businessObjects;
		}

		#endregion Static Methods

		private KsUserLoginFailureEntity _entity;

		/// <summary>
		/// Constructor.  Private to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		private KsUserLoginFailure(KsUserLoginFailureEntity entity)
		{
			_entity = entity;
		}

		internal KsUserLoginFailureEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public string KsUserId
		{
			get => _entity.KsUserId;
			set => _entity.KsUserId = value;
		}

		public DateTime FailDt
		{
			get => _entity.FailDt;
			set => _entity.FailDt = value;
		}

		#endregion Entity Properties

		#region Relationships

		public List<KsUser> User = new();

		#endregion Relationships

	}
}
