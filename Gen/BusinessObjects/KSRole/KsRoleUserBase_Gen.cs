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

using Koretech.Domains.KsRoles.Entities;
using Koretech.Platform.BusinessObjects;
using System.Collections;
using Koretech.Domains.KsUsers.BusinessObjects;
using Koretech.Domains.KsObjects.BusinessObjects;
using Koretech.Domains.KsPages.BusinessObjects;


namespace Koretech.Domains.KsRoles.BusinessObjects
{
	/// <summary>
	/// This business object class wraps the domain entity KsRoleUserEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsRoleUserBase : BusinessObject
	{

		private KsRoleUserEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected KsRoleUserBase(KsRoleUserEntity entity)
		{
			_entity = entity;
		}

		public KsRoleUserBase()
		{
			_entity = new();
		}

		/// <summary>
		/// Initializes a new instance of the business object class.
		/// Override when you need to do work in the KsRoleUser(entity) constructor.
		/// </summary>
		protected virtual void Initialize() { }

		internal KsRoleUserEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public virtual int? RoleNo
		{
			get => _entity.RoleNo;
			set => _entity.RoleNo = value;
		}

		public virtual string? RoleDesc
		{
			get => _entity.RoleDesc;
			set => _entity.RoleDesc = value;
		}

		public virtual char? WebAssignFlg
		{
			get => _entity.WebAssignFlg;
			set => _entity.WebAssignFlg = value;
		}

		public virtual char? ImplicitAssignFlg
		{
			get => _entity.ImplicitAssignFlg;
			set => _entity.ImplicitAssignFlg = value;
		}

		public virtual string? HomePage
		{
			get => _entity.HomePage;
			set => _entity.HomePage = value;
		}

		public virtual string? HomePageQueryString
		{
			get => _entity.HomePageQueryString;
			set => _entity.HomePageQueryString = value;
		}

		#endregion Entity Properties

		#region Relationships

		public List<KsBindRole> KsBindRoles = new();

		public List<KsUserRole> KsUsers = new();

		public List<KsObjectMethodSecurity> KsObjectMethodSecurities = new();

		public List<KsObjectPropertySecurity> KsObjectPropertySecurities = new();

		public List<KsPageSecurity> KsPageSecurities = new();

		#endregion Relationships

	}
}
