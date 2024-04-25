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

using Koretech.Domains.KsApplications.BusinessObjects;
using Koretech.Domains.KsApplications.Repositories;

namespace Koretech.Domains.KsApplications
{
	public partial interface IKsApplicationService
	{
		public Task<IEnumerable<KsApplication>> GetAllAsync();

		public Task<KsApplication?> GetByPrimaryKeyAsync(int AppNo);

		public void Insert(KsApplication businessObject);

		public void Update(KsApplication businessObject);

		public void Delete(KsApplication businessObject);
	}
}
