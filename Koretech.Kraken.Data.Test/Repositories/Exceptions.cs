using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koretech.Kraken.Repositories
{
    public abstract class NotFoundException : Exception
    {
        protected NotFoundException(string message)
            : base(message)
        {
        }
    }

    public sealed class KsUserNotFoundException : NotFoundException
    {
        public KsUserNotFoundException(string ksUserId)
            : base($"The user with the identifier {ksUserId} was not found.")
        {
        }
    }
}
