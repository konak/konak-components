using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Security
{
    public class KonakUserPrincipal : IPrincipal
    {
        private KonakUserIdentity _identity;

        public KonakUserPrincipal()
        {
            _identity = new KonakUserIdentity();
        }

        public IIdentity Identity { get { return _identity; } }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }
    }
}
