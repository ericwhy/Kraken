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

using Koretech.Infrastructure.Services.KsUser.Entities;
using System.Collections;

namespace Koretech.Infrastructure.Services.KsUser.BusinessObjects
{
	/// <summary>
	/// This business object class wraps the domain entity KsUserRoleEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsUserRoleBase
	{

		private KsUserRoleEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected KsUserRoleBase(KsUserRoleEntity entity)
		{
			_entity = entity;
		}

		public KsUserRoleBase()
		{
			_entity = new();
		}

		internal KsUserRoleEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public virtual string KsUserId
		{
			get => _entity.KsUserId;
			set => _entity.KsUserId = value;
		}

		public virtual string ResourceType
		{
			get => _entity.ResourceType;
			set => _entity.ResourceType = value;
		}

		public virtual string ResourceName
		{
			get => _entity.ResourceName;
			set => _entity.ResourceName = value;
		}

		public virtual int RoleNo
		{
			get => _entity.RoleNo;
			set => _entity.RoleNo = value;
		}

		#endregion Entity Properties

		#region Relationships

		public virtual List<KsUser> User = new();

		public virtual KsRoleUser Role;

		#endregion Relationships

	}
}
