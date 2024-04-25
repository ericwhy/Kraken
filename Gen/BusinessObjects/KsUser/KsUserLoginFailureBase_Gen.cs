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
	/// This business object class wraps the domain entity KsUserLoginFailureEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsUserLoginFailureBase : BusinessObject
	{

		private KsUserLoginFailureEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected KsUserLoginFailureBase(KsUserLoginFailureEntity entity)
		{
			_entity = entity;
		}

		public KsUserLoginFailureBase()
		{
			_entity = new();
		}

		/// <summary>
		/// Initializes a new instance of the business object class.
		/// Override when you need to do work in the KsUserLoginFailure(entity) constructor.
		/// </summary>
		protected virtual void Initialize() { }

		internal KsUserLoginFailureEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public virtual string KsUserId
		{
			get => _entity.KsUserId;
			set => _entity.KsUserId = value;
		}

		public virtual DateTime FailDt
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
