using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SummerBootCampTask.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string UserIdentifier { get; set; }
        public bool PrivateProfile { get; set; }
    }
}
