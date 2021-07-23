using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SummerBootCampTask.Models
{
    public class HomeViewModel
    {
        public List<FriendRequestViewModel> FriendRequests { get; set; }
        public List<UserViewModel> Friends { get; set; }
        public UserViewModel User { get; set; }
    }
}
