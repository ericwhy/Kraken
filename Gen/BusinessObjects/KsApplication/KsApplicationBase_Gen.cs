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

using Koretech.Domains.KsApplications.Entities;
using Koretech.Platform.BusinessObjects;
using System.Collections;

namespace Koretech.Domains.KsApplications.BusinessObjects
{
	/// <summary>
	/// This business object class wraps the domain entity KsApplicationEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsApplicationBase : BusinessObject
	{

		private KsApplicationEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected KsApplicationBase(KsApplicationEntity entity)
		{
			_entity = entity;
		}

		public KsApplicationBase()
		{
			_entity = new();
		}

		/// <summary>
		/// Initializes a new instance of the business object class.
		/// Override when you need to do work in the KsApplication(entity) constructor.
		/// </summary>
		protected virtual void Initialize() { }

		internal KsApplicationEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public virtual int? AppNo
		{
			get => _entity.AppNo;
			set => _entity.AppNo = value;
		}

		public virtual string? AppName
		{
			get => _entity.AppName;
			set => _entity.AppName = value;
		}

		public virtual Guid? AppKey
		{
			get => _entity.AppKey;
			set => _entity.AppKey = value;
		}

		#endregion Entity Properties

		#region Relationships

		#endregion Relationships

	}
}
