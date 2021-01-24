using System;
using System.Collections.Generic;

#nullable disable

namespace GarbageCanApi.Models
{
    public partial class Assign
    {
        public int IdAssign { get; set; }
        public string IdUser { get; set; }
        public int? IdRequest { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Request IdRequestNavigation { get; set; }
        public virtual User IdUserNavigation { get; set; }
    }
}
