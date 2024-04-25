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

using Koretech.Domains.KsPages.Entities;
using Koretech.Platform.BusinessObjects;
using System.Collections;

namespace Koretech.Domains.KsPages.BusinessObjects
{
	/// <summary>
	/// This business object class wraps the domain entity KsPageObjectEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsPageObjectBase : BusinessObject
	{

		private KsPageObjectEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected KsPageObjectBase(KsPageObjectEntity entity)
		{
			_entity = entity;
		}

		public KsPageObjectBase()
		{
			_entity = new();
		}

		/// <summary>
		/// Initializes a new instance of the business object class.
		/// Override when you need to do work in the KsPageObject(entity) constructor.
		/// </summary>
		protected virtual void Initialize() { }

		internal KsPageObjectEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public virtual string? PageName
		{
			get => _entity.PageName;
			set => _entity.PageName = value;
		}

		public virtual string? ObjectId
		{
			get => _entity.ObjectId;
			set => _entity.ObjectId = value;
		}

		public virtual char? PrimaryFlg
		{
			get => _entity.PrimaryFlg;
			set => _entity.PrimaryFlg = value;
		}

		#endregion Entity Properties

		#region Relationships

		#endregion Relationships

	}
}
