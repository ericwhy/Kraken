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
	/// This business object class wraps the domain entity KsPageModuleSettingEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsPageModuleSettingBase : BusinessObject
	{

		private KsPageModuleSettingEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected KsPageModuleSettingBase(KsPageModuleSettingEntity entity)
		{
			_entity = entity;
		}

		public KsPageModuleSettingBase()
		{
			_entity = new();
		}

		/// <summary>
		/// Initializes a new instance of the business object class.
		/// Override when you need to do work in the KsPageModuleSetting(entity) constructor.
		/// </summary>
		protected virtual void Initialize() { }

		internal KsPageModuleSettingEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

		public virtual int? PageModuleNo
		{
			get => _entity.PageModuleNo;
			set => _entity.PageModuleNo = value;
		}

		public virtual string? Name
		{
			get => _entity.Name;
			set => _entity.Name = value;
		}

		public virtual string? Value
		{
			get => _entity.Value;
			set => _entity.Value = value;
		}

		#endregion Entity Properties

		#region Relationships

		public KsPageModule? PageModule;

		#endregion Relationships

	}
}
