using System;
using System.Collections.Generic;
using System.Text;

namespace Orange.OWA.Interface
{
    public interface IAccount
    {
        string UserName { get; set; }
        string Password { get; set; }
        string Host { get; set; }
        string Email { get; set; }
        bool IsDefault { get; set; }
        Guid Id { get; set; }
    }
}
