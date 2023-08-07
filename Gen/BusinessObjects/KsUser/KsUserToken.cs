//
// Created by Kraken KAML BO Generator
//

using System.Collections;

namespace Koretech.Kraken.BusinessObjects.KsUser
{
	public class KsUserToken
	{
		#region Properties

		public int TokenNo { get; set; }

		public Guid Selector { get; set; }

		public byte[] ValidatorHash { get; set; }

		public string KsUserId { get; set; }

		public DateTime ExpirationDt { get; set; }

		#endregion Properties

		#region Relationships

		public KsUser KsUser;

		#endregion Relationships
		}
	}
}
