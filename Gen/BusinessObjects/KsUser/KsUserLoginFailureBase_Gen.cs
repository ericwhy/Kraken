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
	/// This business object class wraps the domain entity KsUserLoginFailureEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsUserLoginFailureBase
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

		public virtual List<KsUser> User = new();

		#endregion Relationships

	}
}
