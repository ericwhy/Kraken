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
	public abstract class PasswordHistoryBase
	{

		private PasswordHistoryEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected PasswordHistoryBase(PasswordHistoryEntity entity)
		{
			_entity = entity;
		}

		public PasswordHistoryBase()
		{
			_entity = new();
		}

		internal PasswordHistoryEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public virtual string KsUserId
		{
			get => _entity.KsUserId;
			set => _entity.KsUserId = value;
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

		public virtual DateTime CreateDt
		{
			get => _entity.CreateDt;
			set => _entity.CreateDt = value;
		}

		#endregion Entity Properties

		#region Relationships

		public virtual List<KsUser> KsUser = new();

		#endregion Relationships

	}
}
