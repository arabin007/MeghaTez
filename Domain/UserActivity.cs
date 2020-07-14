using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class UserActivity
    {
        public string AppUserId { get; set; }
        public Guid ActivityId { get; set; }

        public virtual AppUser AppUser { get; set; }    // Specifying this since AppUserId is a Foreign Key here.
        public virtual Activity Activity { get; set; }  // Specifying this since ActivityId is a Foreign Key here.

        public DateTime DateJoined { get; set; }
        public bool IsHost { get; set; }
    }
}
