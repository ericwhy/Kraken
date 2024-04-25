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
	/// This business object class wraps the domain entity KsControlEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsControlBase : BusinessObject
	{

		private KsControlEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected KsControlBase(KsControlEntity entity)
		{
			_entity = entity;
		}

		public KsControlBase()
		{
			_entity = new();
		}

		/// <summary>
		/// Initializes a new instance of the business object class.
		/// Override when you need to do work in the KsControl(entity) constructor.
		/// </summary>
		protected virtual void Initialize() { }

		internal KsControlEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public virtual int? CategoryCd
		{
			get => _entity.CategoryCd;
			set => _entity.CategoryCd = value;
		}

		public virtual int? KeySeq
		{
			get => _entity.KeySeq;
			set => _entity.KeySeq = value;
		}

		public virtual int? KeyIntValue
		{
			get => _entity.KeyIntValue;
			set => _entity.KeyIntValue = value;
		}

		public virtual string? KeyStrValue
		{
			get => _entity.KeyStrValue;
			set => _entity.KeyStrValue = value;
		}

		public virtual DateTime? KeyDateValue
		{
			get => _entity.KeyDateValue;
			set => _entity.KeyDateValue = value;
		}

		public virtual string? KeyType
		{
			get => _entity.KeyType;
			set => _entity.KeyType = value;
		}

		public virtual string? KeyDesc
		{
			get => _entity.KeyDesc;
			set => _entity.KeyDesc = value;
		}

		#endregion Entity Properties

		#region Relationships

		#endregion Relationships

	}
}
