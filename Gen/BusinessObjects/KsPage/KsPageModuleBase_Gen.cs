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
	/// This business object class wraps the domain entity KsPageModuleEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsPageModuleBase : BusinessObject
	{

		private KsPageModuleEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected KsPageModuleBase(KsPageModuleEntity entity)
		{
			_entity = entity;
		}

		public KsPageModuleBase()
		{
			_entity = new();
		}

		/// <summary>
		/// Initializes a new instance of the business object class.
		/// Override when you need to do work in the KsPageModule(entity) constructor.
		/// </summary>
		protected virtual void Initialize() { }

		internal KsPageModuleEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public virtual int? PageModuleNo
		{
			get => _entity.PageModuleNo;
			set => _entity.PageModuleNo = value;
		}

		public virtual string? PageName
		{
			get => _entity.PageName;
			set => _entity.PageName = value;
		}

		public virtual int? ModuleNo
		{
			get => _entity.ModuleNo;
			set => _entity.ModuleNo = value;
		}

		public virtual string? Title
		{
			get => _entity.Title;
			set => _entity.Title = value;
		}

		public virtual string? ZoneName
		{
			get => _entity.ZoneName;
			set => _entity.ZoneName = value;
		}

		public virtual int? Sequence
		{
			get => _entity.Sequence;
			set => _entity.Sequence = value;
		}

		public virtual string? CssClass
		{
			get => _entity.CssClass;
			set => _entity.CssClass = value;
		}

		#endregion Entity Properties

		#region Relationships

		public KsPage? Page;

		public KsModule? Module;

		#endregion Relationships

	}
}
