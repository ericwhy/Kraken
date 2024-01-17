//
// Created by Kraken KAML BO Generator
//
// DO NOT MODIFY
//

using Koretech.Kraken.Entities.KsUser;

namespace Koretech.Kraken.BusinessObjects.KsUser
{
    /// <summary>
    /// This business object class wraps the domain entity KsUserEntity and provides access to the entity's data
    /// through accessor properties.  It also provides a place for business logic related to the domain entity.
    /// </summary>
    public partial class KsUser
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
			if (entity == null) { return null; }

			KsUser businessObject = new(entity);

			// Recursively create business objects from the entities that have relationships with this one
			// and link to them through the relationship properties in this class.

			IList<KsUserLoginFailure> newKsUserLoginFailure = Koretech.Kraken.BusinessObjects.KsUser.KsUserLoginFailure.NewInstance(entity.LoginFailures);
			businessObject.LoginFailures.AddRange(newKsUserLoginFailure);

			IList<PasswordHistory> newPasswordHistory = Koretech.Kraken.BusinessObjects.KsUser.PasswordHistory.NewInstance(entity.PasswordHistory);
			businessObject.PasswordHistory.AddRange(newPasswordHistory);

			IList<KsUserRole> newKsUserRole = Koretech.Kraken.BusinessObjects.KsUser.KsUserRole.NewInstance(entity.UserRoles);
			businessObject.UserRoles.AddRange(newKsUserRole);

			businessObject.UserToken = KsUserToken.NewInstance(entity.UserToken);

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

				IList<KsUserLoginFailure> newKsUserLoginFailure = Koretech.Kraken.BusinessObjects.KsUser.KsUserLoginFailure.NewInstance(entity.LoginFailures);
				newBusinessObject.LoginFailures.AddRange(newKsUserLoginFailure);

				IList<PasswordHistory> newPasswordHistory = Koretech.Kraken.BusinessObjects.KsUser.PasswordHistory.NewInstance(entity.PasswordHistory);
				newBusinessObject.PasswordHistory.AddRange(newPasswordHistory);

				IList<KsUserRole> newKsUserRole = Koretech.Kraken.BusinessObjects.KsUser.KsUserRole.NewInstance(entity.UserRoles);
				newBusinessObject.UserRoles.AddRange(newKsUserRole);

				newBusinessObject.UserToken = KsUserToken.NewInstance(entity.UserToken);
			}

			return businessObjects;
		}

		#endregion Static Methods

		private KsUserEntity _entity;

		/// <summary>
		/// Constructor.  Private to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		private KsUser(KsUserEntity entity)
		{
			_entity = entity;
		}

		public KsUser() 
		{
			_entity = new();
		}

		internal KsUserEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public string KsUserId
		{
			get => _entity.KsUserId;
			set => _entity.KsUserId = value;
		}

		public string? DisplayName
		{
			get => _entity.DisplayName;
			set => _entity.DisplayName = value;
		}

		public string EmailAddress
		{
			get => _entity.EmailAddress;
			set => _entity.EmailAddress = value;
		}

		public string? PasswordHints
		{
			get => _entity.PasswordHints;
			set => _entity.PasswordHints = value;
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

		public DateTime PasswordDt
		{
			get => _entity.PasswordDt;
			set => _entity.PasswordDt = value;
		}

		public char AllowAccessFlg
		{
			get => _entity.AllowAccessFlg;
			set => _entity.AllowAccessFlg = value;
		}

		public char IntegratedAuth
		{
			get => _entity.IntegratedAuth;
			set => _entity.IntegratedAuth = value;
		}

		public char AuthPrompt
		{
			get => _entity.AuthPrompt;
			set => _entity.AuthPrompt = value;
		}

		public char PwresetFlg
		{
			get => _entity.PwresetFlg;
			set => _entity.PwresetFlg = value;
		}

		public byte? FailedLoginCnt
		{
			get => _entity.FailedLoginCnt;
			set => _entity.FailedLoginCnt = value;
		}

		public DateTime? FailedLoginDt
		{
			get => _entity.FailedLoginDt;
			set => _entity.FailedLoginDt = value;
		}

		#endregion Entity Properties

		#region Relationships

		public List<KsUserLoginFailure> LoginFailures = new();

		public List<PasswordHistory> PasswordHistory = new();

		public List<KsUserRole> UserRoles = new();

		public KsUserToken UserToken;

		#endregion Relationships

	}
}
