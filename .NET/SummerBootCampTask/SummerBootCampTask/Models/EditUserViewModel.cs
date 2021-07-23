using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SummerBootCampTask.Models
{
    public class EditUserViewModel
    {
        [Display(Name = "Username")]
        public string Name { get; set; }

        [Display(Name = "Private profile")]
        public bool PrivateProfile { get; set; } 
    }
}
