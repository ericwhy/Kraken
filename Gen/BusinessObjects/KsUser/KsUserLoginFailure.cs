//
// Created by Kraken KAML BO Generator
//

using System.Collections;

namespace Koretech.Kraken.BusinessObjects.KsUser
{
	public class KsUserLoginFailure
	{
		#region Properties

		public string KsUserId { get; set; }

		public DateTime FailDt { get; set; }

		#endregion Properties

		#region Relationships

		public List<KsUser> User = new();

		#endregion Relationships
		}
	}
}
