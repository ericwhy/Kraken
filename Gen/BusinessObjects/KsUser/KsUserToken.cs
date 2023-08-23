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
	/// This business object class wraps the domain entity KsUserTokenEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public class KsUserToken
	{
		#region Static Methods

		/// <summary>
		/// Creates a business object from an entity.  Any child entities
		/// will also have business objects created for them and attached to the parent business
		/// object.  This will result in a business object tree that mirrors the entity tree.
		/// </summary>
		/// <param name="entity">The entity to create a business object from</param>
		/// <returns>A newly created business object wrapping the provided entity</returns>
		public static KsUserToken NewInstance(KsUserTokenEntity entity)
		{
			KsUserToken businessObject = new(entity);

			// Recursively create business objects from the entities that have relationships with this one
			// and link to them through the relationship properties in this class.

			businessObject.KsUser = KsUser.NewInstance(entity.KsUser);

			return businessObject;
		}

		/// <summary>
		/// Creates business objects from a set of entities by wrapping each entity.  Any child entities
		/// will also have business objects created for them and attached to the appropriate parent business
		/// object.  This will result in a business object tree that mirrors the entity tree.
		/// </summary>
		/// <param name="entities">The entities to create business objects from</param>
		/// <returns>A newly created business object(s) wrapping the provided entities</returns>
		public static IList<KsUserToken> NewInstance(IList<KsUserTokenEntity> entities)
		{
			List<KsUserToken> businessObjects = new();

			foreach (KsUserTokenEntity entity in entities)
			{
				KsUserToken newBusinessObject = new(entity);
				businessObjects.Add(newBusinessObject);

				// Recursively create business objects from the entities that have relationships with this one
				// and link to them through the relationship properties in this class.

				newBusinessObject.KsUser = KsUser.NewInstance(entity.KsUser);
			}

			return businessObjects;
		}

		#endregion Static Methods

		private KsUserTokenEntity _entity;

		/// <summary>
		/// Constructor.  Private to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		private KsUserToken(KsUserTokenEntity entity)
		{
			_entity = entity;
		}

		internal KsUserTokenEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public int TokenNo
		{
			get => _entity.TokenNo;
			set => _entity.TokenNo = value;
		}

		public Guid Selector
		{
			get => _entity.Selector;
			set => _entity.Selector = value;
		}

		public byte[] ValidatorHash
		{
			get => _entity.ValidatorHash;
			set => _entity.ValidatorHash = value;
		}

		public string KsUserId
		{
			get => _entity.KsUserId;
			set => _entity.KsUserId = value;
		}

		public DateTime ExpirationDt
		{
			get => _entity.ExpirationDt;
			set => _entity.ExpirationDt = value;
		}

		#endregion Entity Properties

		#region Relationships

		public KsUser KsUser;

		#endregion Relationships

	}
}
