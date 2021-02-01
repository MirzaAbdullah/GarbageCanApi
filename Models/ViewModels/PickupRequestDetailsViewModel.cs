using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCanApi.Models.ViewModels
{
    public class PickupRequestDetailsViewModel
    {
        public string IdRequestDetail { get; set; }
        public string IdRequest { get; set; }
        public string IdItem { get; set; }
        public string ItemName { get; set; }
        public double ItemWeight { get; set; }
        public double ItemCost { get; set; }
    }
}
