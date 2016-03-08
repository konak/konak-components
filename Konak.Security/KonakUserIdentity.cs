using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Konak.Security
{
    public class KonakUserIdentity : IIdentity
    {
        public const string AUTH_TYPE = "KONAK-AUTH";

        #region private properties
        private bool _isAuthenticated;
        private string _name;
        #endregion

        public KonakUserIdentity()
        {
            _isAuthenticated = false;
            _name = "guest";
        }

        #region public properties
        public string AuthenticationType { get { return AUTH_TYPE; } }

        public bool IsAuthenticated { get { return _isAuthenticated; } }

        public string Name { get { throw new NotImplementedException(); } }
        #endregion

    }
}
