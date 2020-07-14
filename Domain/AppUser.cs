using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class AppUser: IdentityUser
    {
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public virtual ICollection<UserActivity> UserActivities { get; set; }  // Specifies one AppUser can have several entries in UserActivities table.
        public virtual ICollection<Photo> Photos { get; set; }
        public virtual Consumer Consumer { get; set; }                        // Specifies one AppUser corresponds to only one or zero consumer.
    }
}
