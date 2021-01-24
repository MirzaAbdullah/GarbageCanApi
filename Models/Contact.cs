using System;
using System.Collections.Generic;

#nullable disable

namespace GarbageCanApi.Models
{
    public partial class Contact
    {
        public int ContactId { get; set; }
        public int? CustomerId { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
