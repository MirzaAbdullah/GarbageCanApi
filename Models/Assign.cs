using System;
using System.Collections.Generic;

#nullable disable

namespace GarbageCanApi.Models
{
    public partial class Assign
    {
        public string IdAssign { get; set; }
        public string IdUser { get; set; }
        public string IdRequest { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Request IdRequestNavigation { get; set; }
        public virtual User IdUserNavigation { get; set; }
    }
}
