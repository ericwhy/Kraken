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
using Koretech.Platform.BusinessObjects;
using System.Collections;

namespace Koretech.Domains.KsUsers.BusinessObjects
{
	/// <summary>
	/// This business object class wraps the domain entity KsUserTokenEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsUserTokenBase : BusinessObject
	{

		private KsUserTokenEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected KsUserTokenBase(KsUserTokenEntity entity)
		{
			_entity = entity;
		}

		public KsUserTokenBase()
		{
			_entity = new();
		}

		/// <summary>
		/// Initializes a new instance of the business object class.
		/// Override when you need to do work in the KsUserToken(entity) constructor.
		/// </summary>
		protected virtual void Initialize() { }

		internal KsUserTokenEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public virtual int TokenNo
		{
			get => _entity.TokenNo;
			set => _entity.TokenNo = value;
		}

		public virtual Guid Selector
		{
			get => _entity.Selector;
			set => _entity.Selector = value;
		}

		public virtual byte[] ValidatorHash
		{
			get => _entity.ValidatorHash;
			set => _entity.ValidatorHash = value;
		}

		public virtual string KsUserId
		{
			get => _entity.KsUserId;
			set => _entity.KsUserId = value;
		}

		public virtual DateTime ExpirationDt
		{
			get => _entity.ExpirationDt;
			set => _entity.ExpirationDt = value;
		}

		#endregion Entity Properties

		#region Relationships

		public KsUser? KsUser;

		#endregion Relationships

	}
}
