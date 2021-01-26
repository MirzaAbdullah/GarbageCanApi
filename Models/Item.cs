using System;
using System.Collections.Generic;

#nullable disable

namespace GarbageCanApi.Models
{
    public partial class Item
    {
        public Item()
        {
            RequestDetails = new HashSet<RequestDetail>();
        }

        public string IdItem { get; set; }
        public string ItemName { get; set; }
        public double ItemPrice { get; set; }

        public virtual ICollection<RequestDetail> RequestDetails { get; set; }
    }
}
