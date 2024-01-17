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
	/// This business object class wraps the domain entity KsUserEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsUserBase
	{

		private KsUserEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected KsUserBase(KsUserEntity entity)
		{
			_entity = entity;
		}

		public KsUserBase()
		{
			_entity = new();
		}

		internal KsUserEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public virtual string KsUserId
		{
			get => _entity.KsUserId;
			set => _entity.KsUserId = value;
		}

		public virtual string? DisplayName
		{
			get => _entity.DisplayName;
			set => _entity.DisplayName = value;
		}

		public virtual string EmailAddress
		{
			get => _entity.EmailAddress;
			set => _entity.EmailAddress = value;
		}

		public virtual string? PasswordHints
		{
			get => _entity.PasswordHints;
			set => _entity.PasswordHints = value;
		}

		public virtual string? Password
		{
			get => _entity.Password;
			set => _entity.Password = value;
		}

		public virtual string? PasswordSalt
		{
			get => _entity.PasswordSalt;
			set => _entity.PasswordSalt = value;
		}

		public virtual DateTime PasswordDt
		{
			get => _entity.PasswordDt;
			set => _entity.PasswordDt = value;
		}

		public virtual char AllowAccessFlg
		{
			get => _entity.AllowAccessFlg;
			set => _entity.AllowAccessFlg = value;
		}

		public virtual char IntegratedAuth
		{
			get => _entity.IntegratedAuth;
			set => _entity.IntegratedAuth = value;
		}

		public virtual char AuthPrompt
		{
			get => _entity.AuthPrompt;
			set => _entity.AuthPrompt = value;
		}

		public virtual char PwresetFlg
		{
			get => _entity.PwresetFlg;
			set => _entity.PwresetFlg = value;
		}

		public virtual byte? FailedLoginCnt
		{
			get => _entity.FailedLoginCnt;
			set => _entity.FailedLoginCnt = value;
		}

		public virtual DateTime? FailedLoginDt
		{
			get => _entity.FailedLoginDt;
			set => _entity.FailedLoginDt = value;
		}

		#endregion Entity Properties

		#region Relationships

		public virtual List<KsUserLoginFailure> LoginFailures = new();

		public virtual List<PasswordHistory> PasswordHistory = new();

		public virtual List<KsUserRole> UserRoles = new();

		public virtual KsUserToken UserToken;

		#endregion Relationships

	}
}
