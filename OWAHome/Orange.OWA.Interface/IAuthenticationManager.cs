using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Orange.OWA.Interface
{
    public interface IAuthenticationManager
    {
        IList<Cookie> CookieCache { get; }

        IAccount Account { get; }
    }
}
