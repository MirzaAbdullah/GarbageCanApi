using System;
using System.Collections.Generic;

#nullable disable

namespace GarbageCanApi.Models
{
    public partial class Request
    {
        public Request()
        {
            Assigns = new HashSet<Assign>();
        }

        public string IdRequest { get; set; }
        public string IdUser { get; set; }
        public string Latitudes { get; set; }
        public string Longitudes { get; set; }
        public DateTime PickupDate { get; set; }
        public TimeSpan PickupTime { get; set; }
        public string PickupItem { get; set; }
        public string PickupWeight { get; set; }
        public string PickupCost { get; set; }
        public string PickupStatus { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual User IdUserNavigation { get; set; }
        public virtual ICollection<Assign> Assigns { get; set; }
    }
}
