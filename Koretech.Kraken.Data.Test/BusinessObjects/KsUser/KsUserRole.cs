//
// Created by Kraken KAML BO Generator
//
// DO NOT MODIFY
//

using Koretech.Kraken.BusinessObjects.KsRole;
using Koretech.Kraken.Entities.KsUser;
using System.Collections;

namespace Koretech.Kraken.BusinessObjects.KsUser
{
	/// <summary>
	/// This business object class wraps the domain entity KsUserRoleEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public class KsUserRole
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

			IList<KsUser> newKsUser = Koretech.Kraken.BusinessObjects.KsUser.KsUser.NewInstance(entity.User);
			businessObject.User.AddRange(newKsUser);

			businessObject.Role = KsRoleUser.NewInstance(entity.Role);

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

				IList<KsUser> newKsUser = Koretech.Kraken.BusinessObjects.KsUser.KsUser.NewInstance(entity.User);
				newBusinessObject.User.AddRange(newKsUser);

				newBusinessObject.Role = KsRoleUser.NewInstance(entity.Role);
			}

			return businessObjects;
		}

		#endregion Static Methods

		private KsUserRoleEntity _entity;

		/// <summary>
		/// Constructor.  Private to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		private KsUserRole(KsUserRoleEntity entity)
		{
			_entity = entity;
		}

		internal KsUserRoleEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public string KsUserId
		{
			get => _entity.KsUserId;
			set => _entity.KsUserId = value;
		}

		public string ResourceType
		{
			get => _entity.ResourceType;
			set => _entity.ResourceType = value;
		}

		public string ResourceName
		{
			get => _entity.ResourceName;
			set => _entity.ResourceName = value;
		}

		public int RoleNo
		{
			get => _entity.RoleNo;
			set => _entity.RoleNo = value;
		}

		#endregion Entity Properties

		#region Relationships

		public List<KsUser> User = new();

		public KsRoleUser Role;

		#endregion Relationships

	}
}
