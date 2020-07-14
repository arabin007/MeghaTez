using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities
{
    public class ActivityDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }

        //public ICollection<AttendeeDTO> attendees { get; set; }   Returns attendees: null coz it cannot map with UserActivities in Activity.cs due to name mismatch.

        [JsonProperty("attendees")]
        public ICollection<AttendeeDTO> UserActivities { get; set; }    //Here Match with UserActivities but change it name afterwards to Attendees
    }
}
