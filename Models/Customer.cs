using System;
using System.Collections.Generic;

#nullable disable

namespace GarbageCanApi.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Contacts = new HashSet<Contact>();
        }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; }
    }
}
