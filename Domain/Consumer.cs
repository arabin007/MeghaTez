using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Consumer
    {
        public string Id { get; set; } 
        public string ConsumerId { get; set; } 
        public string TypeOfConsumer { get; set; }
        public string NationalId { get; set; }
        public string MeterId { get; set; }
        public string MeterCapacity { get; set; }

        public string AppIdFK { get; set; }           //Foreign Key Column
        public virtual AppUser AppUser { get; set; }  //Above Foreign Key references AppUser
    }
}
