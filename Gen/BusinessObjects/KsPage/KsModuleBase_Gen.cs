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
	/// This business object class wraps the domain entity KsModuleEntity and provides access to the entity's data
	/// through accessor properties.  It also provides a place for business logic related to the domain entity.
	/// </summary>
	public abstract class KsModuleBase : BusinessObject
	{

		private KsModuleEntity _entity;

		/// <summary>
		/// Constructor.  Protected to force use of the static factory method NewInstance().
		/// </summary>
		/// <param name="entity">An entity that provides data for the business object</param>
		protected KsModuleBase(KsModuleEntity entity)
		{
			_entity = entity;
		}

		public KsModuleBase()
		{
			_entity = new();
		}

		/// <summary>
		/// Initializes a new instance of the business object class.
		/// Override when you need to do work in the KsModule(entity) constructor.
		/// </summary>
		protected virtual void Initialize() { }

		internal KsModuleEntity Entity
		{
			get => _entity;
		}

		#region Entity Properties

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

		#endregion Entity Properties

		#region Relationships

		#endregion Relationships

	}
}
