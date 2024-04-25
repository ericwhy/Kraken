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

namespace Koretech.Domains.KsRoles.BusinessObjects
{
	/// <summary>
	/// This business object class wraps the domain entity KsBindRoleEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsBindRoleBase : BusinessObject
	{

		private KsBindRoleEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected KsBindRoleBase(KsBindRoleEntity entity)
		{
			_entity = entity;
		}

		public KsBindRoleBase()
		{
			_entity = new();
		}

		/// <summary>
		/// Initializes a new instance of the business object class.
		/// Override when you need to do work in the KsBindRole(entity) constructor.
		/// </summary>
		protected virtual void Initialize() { }

		internal KsBindRoleEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public virtual int? RoleNo
		{
			get => _entity.RoleNo;
			set => _entity.RoleNo = value;
		}

		public virtual string? IfColumnName
		{
			get => _entity.IfColumnName;
			set => _entity.IfColumnName = value;
		}

		public virtual string? IfAttrName
		{
			get => _entity.IfAttrName;
			set => _entity.IfAttrName = value;
		}

		public virtual string? IfOperator
		{
			get => _entity.IfOperator;
			set => _entity.IfOperator = value;
		}

		public virtual string? IfValue
		{
			get => _entity.IfValue;
			set => _entity.IfValue = value;
		}

		#endregion Entity Properties

		#region Relationships

		public List<KsRoleUser> KsRoleUser = new();

		#endregion Relationships

	}
}
