using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SummerBootCampTask.CoreModels
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; }
        public string UserIdentifier { get; set; }
        public int Key { get; set; }
        public bool PrivateProfile { get; set; }

        public User() { }
    }
}
