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
	/// This business object class wraps the domain entity KsPageContentEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsPageContentBase : BusinessObject
	{

		private KsPageContentEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected KsPageContentBase(KsPageContentEntity entity)
		{
			_entity = entity;
		}

		public KsPageContentBase()
		{
			_entity = new();
		}

		/// <summary>
		/// Initializes a new instance of the business object class.
		/// Override when you need to do work in the KsPageContent(entity) constructor.
		/// </summary>
		protected virtual void Initialize() { }

		internal KsPageContentEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public virtual string? PageName
		{
			get => _entity.PageName;
			set => _entity.PageName = value;
		}

		public virtual string? ZoneName
		{
			get => _entity.ZoneName;
			set => _entity.ZoneName = value;
		}

		public virtual string? ControlId
		{
			get => _entity.ControlId;
			set => _entity.ControlId = value;
		}

		#endregion Entity Properties

		#region Relationships

		#endregion Relationships

	}
}
