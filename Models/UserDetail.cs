using System;
using System.Collections.Generic;

#nullable disable

namespace GarbageCanApi.Models
{
    public partial class UserDetail
    {
        public int IdUserDetail { get; set; }
        public string IdUser { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }

        public virtual User IdUserNavigation { get; set; }
    }
}
