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
	/// This business object class wraps the domain entity KsMenuItemEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsMenuItemBase : BusinessObject
	{

		private KsMenuItemEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected KsMenuItemBase(KsMenuItemEntity entity)
		{
			_entity = entity;
		}

		public KsMenuItemBase()
		{
			_entity = new();
		}

		/// <summary>
		/// Initializes a new instance of the business object class.
		/// Override when you need to do work in the KsMenuItem(entity) constructor.
		/// </summary>
		protected virtual void Initialize() { }

		internal KsMenuItemEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public virtual int? MenuItemNo
		{
			get => _entity.MenuItemNo;
			set => _entity.MenuItemNo = value;
		}

		public virtual string? MenuTitle
		{
			get => _entity.MenuTitle;
			set => _entity.MenuTitle = value;
		}

		public virtual string? MenuHref
		{
			get => _entity.MenuHref;
			set => _entity.MenuHref = value;
		}

		public virtual int? MenuSeq
		{
			get => _entity.MenuSeq;
			set => _entity.MenuSeq = value;
		}

		public virtual string? ResourceName
		{
			get => _entity.ResourceName;
			set => _entity.ResourceName = value;
		}

		public virtual int? ParentMenuItemNo
		{
			get => _entity.ParentMenuItemNo;
			set => _entity.ParentMenuItemNo = value;
		}

		public virtual char? DisplaySeparatorBeforeFlg
		{
			get => _entity.DisplaySeparatorBeforeFlg;
			set => _entity.DisplaySeparatorBeforeFlg = value;
		}

		public virtual string? IconCssClass
		{
			get => _entity.IconCssClass;
			set => _entity.IconCssClass = value;
		}

		public virtual string? IconName
		{
			get => _entity.IconName;
			set => _entity.IconName = value;
		}

		public virtual int? MenuNo
		{
			get => _entity.MenuNo;
			set => _entity.MenuNo = value;
		}

		public virtual string? PageName
		{
			get => _entity.PageName;
			set => _entity.PageName = value;
		}

		public virtual char? PopupFlg
		{
			get => _entity.PopupFlg;
			set => _entity.PopupFlg = value;
		}

		public virtual string? BodyHtml
		{
			get => _entity.BodyHtml;
			set => _entity.BodyHtml = value;
		}

		#endregion Entity Properties

		#region Relationships

		#endregion Relationships

	}
}
