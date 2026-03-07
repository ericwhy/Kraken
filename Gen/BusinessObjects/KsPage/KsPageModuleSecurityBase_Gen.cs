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
using Koretech.Domains.KsRoles.BusinessObjects;


namespace Koretech.Domains.KsPages.BusinessObjects
{
	/// <summary>
	/// This business object class wraps the domain entity KsPageModuleSecurityEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsPageModuleSecurityBase : BusinessObject
	{

		private KsPageModuleSecurityEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected KsPageModuleSecurityBase(KsPageModuleSecurityEntity entity)
		{
			_entity = entity;
		}

		public KsPageModuleSecurityBase()
		{
			_entity = new();
		}

		/// <summary>
		/// Initializes a new instance of the business object class.
		/// Override when you need to do work in the KsPageModuleSecurity(entity) constructor.
		/// </summary>
		protected virtual void Initialize() { }

		internal KsPageModuleSecurityEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public virtual int? PageModuleNo
		{
			get => _entity.PageModuleNo;
			set => _entity.PageModuleNo = value;
		}

		public virtual int? RoleNo
		{
			get => _entity.RoleNo;
			set => _entity.RoleNo = value;
		}

		public virtual char? DenyAccessOverrideFlg
		{
			get => _entity.DenyAccessOverrideFlg;
			set => _entity.DenyAccessOverrideFlg = value;
		}

		#endregion Entity Properties

		#region Relationships

		public KsPageModule? PageModule;

		public KsRoleUser? Role;

		#endregion Relationships

	}
}
