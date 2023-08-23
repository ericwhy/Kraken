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
	/// This business object class wraps the domain entity PasswordHistoryEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public class PasswordHistory
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

			IList<KsUser> newKsUser = Koretech.Kraken.BusinessObjects.KsUser.KsUser.NewInstance(entity.KsUser);
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

				IList<KsUser> newKsUser = Koretech.Kraken.BusinessObjects.KsUser.KsUser.NewInstance(entity.KsUser);
				newBusinessObject.KsUser.AddRange(newKsUser);
			}

			return businessObjects;
		}

		#endregion Static Methods

		private PasswordHistoryEntity _entity;

		/// <summary>
		/// Constructor.  Private to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		private PasswordHistory(PasswordHistoryEntity entity)
		{
			_entity = entity;
		}

		internal PasswordHistoryEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public string KsUserId
		{
			get => _entity.KsUserId;
			set => _entity.KsUserId = value;
		}

		public string? Password
		{
			get => _entity.Password;
			set => _entity.Password = value;
		}

		public string? PasswordSalt
		{
			get => _entity.PasswordSalt;
			set => _entity.PasswordSalt = value;
		}

		public DateTime CreateDt
		{
			get => _entity.CreateDt;
			set => _entity.CreateDt = value;
		}

		#endregion Entity Properties

		#region Relationships

		public List<KsUser> KsUser = new();

		#endregion Relationships

	}
}
