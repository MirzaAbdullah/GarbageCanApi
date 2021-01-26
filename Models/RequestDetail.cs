using System;
using System.Collections.Generic;

#nullable disable

namespace GarbageCanApi.Models
{
    public partial class RequestDetail
    {
        public string IdRequestDetail { get; set; }
        public string IdItem { get; set; }
        public string IdRequest { get; set; }
        public double ItemWeight { get; set; }
        public double ItemCost { get; set; }

        public virtual Item IdItemNavigation { get; set; }
        public virtual Request IdRequestNavigation { get; set; }
    }
}
