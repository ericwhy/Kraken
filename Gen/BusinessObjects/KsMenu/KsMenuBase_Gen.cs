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

using Koretech.Domains.KsMenus.Entities;
using Koretech.Platform.BusinessObjects;
using System.Collections;

namespace Koretech.Domains.KsMenus.BusinessObjects
{
	/// <summary>
	/// This business object class wraps the domain entity KsMenuEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsMenuBase : BusinessObject
	{

		private KsMenuEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected KsMenuBase(KsMenuEntity entity)
		{
			_entity = entity;
		}

		public KsMenuBase()
		{
			_entity = new();
		}

		/// <summary>
		/// Initializes a new instance of the business object class.
		/// Override when you need to do work in the KsMenu(entity) constructor.
		/// </summary>
		protected virtual void Initialize() { }

		internal KsMenuEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public virtual int? MenuNo
		{
			get => _entity.MenuNo;
			set => _entity.MenuNo = value;
		}

		public virtual string? MenuId
		{
			get => _entity.MenuId;
			set => _entity.MenuId = value;
		}

		public virtual string? MenuName
		{
			get => _entity.MenuName;
			set => _entity.MenuName = value;
		}

		public virtual string? FooterHtml
		{
			get => _entity.FooterHtml;
			set => _entity.FooterHtml = value;
		}

		public virtual string? Style
		{
			get => _entity.Style;
			set => _entity.Style = value;
		}

		public virtual char? TitleVisibleFlg
		{
			get => _entity.TitleVisibleFlg;
			set => _entity.TitleVisibleFlg = value;
		}

		#endregion Entity Properties

		#region Relationships

		#endregion Relationships

	}
}
