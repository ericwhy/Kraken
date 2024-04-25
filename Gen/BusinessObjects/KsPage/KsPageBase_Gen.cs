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
	/// This business object class wraps the domain entity KsPageEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsPageBase : BusinessObject
	{

		private KsPageEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected KsPageBase(KsPageEntity entity)
		{
			_entity = entity;
		}

		public KsPageBase()
		{
			_entity = new();
		}

		/// <summary>
		/// Initializes a new instance of the business object class.
		/// Override when you need to do work in the KsPage(entity) constructor.
		/// </summary>
		protected virtual void Initialize() { }

		internal KsPageEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public virtual string? PageName
		{
			get => _entity.PageName;
			set => _entity.PageName = value;
		}

		public virtual string? Title
		{
			get => _entity.Title;
			set => _entity.Title = value;
		}

		public virtual string? LayoutName
		{
			get => _entity.LayoutName;
			set => _entity.LayoutName = value;
		}

		public virtual string? TypeCd
		{
			get => _entity.TypeCd;
			set => _entity.TypeCd = value;
		}

		public virtual string? MetaDescription
		{
			get => _entity.MetaDescription;
			set => _entity.MetaDescription = value;
		}

		public virtual string? MetaKeywords
		{
			get => _entity.MetaKeywords;
			set => _entity.MetaKeywords = value;
		}

		public virtual string? AdditionalMetaElements
		{
			get => _entity.AdditionalMetaElements;
			set => _entity.AdditionalMetaElements = value;
		}

		public virtual string? Controller
		{
			get => _entity.Controller;
			set => _entity.Controller = value;
		}

		public virtual string? Action
		{
			get => _entity.Action;
			set => _entity.Action = value;
		}

		public virtual byte[]? MetadataHash
		{
			get => _entity.MetadataHash;
			set => _entity.MetadataHash = value;
		}

		public virtual char? TitleVisibleFlg
		{
			get => _entity.TitleVisibleFlg;
			set => _entity.TitleVisibleFlg = value;
		}

		public virtual char? FullWidthFlg
		{
			get => _entity.FullWidthFlg;
			set => _entity.FullWidthFlg = value;
		}

		public virtual string? LogicalPageName
		{
			get => _entity.LogicalPageName;
			set => _entity.LogicalPageName = value;
		}

		public virtual string? RedirectPageName
		{
			get => _entity.RedirectPageName;
			set => _entity.RedirectPageName = value;
		}

		#endregion Entity Properties

		#region Relationships

		#endregion Relationships

	}
}
