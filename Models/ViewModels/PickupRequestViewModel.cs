using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCanApi.Models.ViewModels
{
    public class PickupRequestViewModel
    {
        public string IdRequest { get; set; }
        public string IdUser { get; set; }
        public User UserData { get; set; }
        public string Latitudes { get; set; }
        public string Longitudes { get; set; }
        public DateTime PickupDate { get; set; }
        public TimeSpan PickupTime { get; set; }
        public double? Pickup_Cost { get; set; }
        public string PickupStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public List<PickupRequestDetailsViewModel> RequestDetails { get; set; }
    }
}
