﻿using SummerBootCampTask.CoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SummerBootCampTask.Models
{
    public class ChatViewModel
    {
        public int Id { get; set; }

        public int FriendId { get; set; }

        public IEnumerable<MessageViewModel> Messages { get; set; }

        public MessageViewModel NewMessage { get; set; }
    }
}
