using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EMRDataLayer.Model
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Password { get; set; }

        // Navigation property for one-to-one relationship with Address
        public Address Address { get; set; }

    }
}
