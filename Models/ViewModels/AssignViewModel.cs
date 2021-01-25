using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCanApi.Models.ViewModels
{
    public class AssignViewModel
    {
        public string IdAssign { get; set; }
        public string IdUser { get; set; }
        public string IdRequest { get; set; }
        public DateTime CreatedDate { get; set; }
        public RequestViewModel Request { get; set; }
        public UserViewModel User { get; set; }
        public UserDetailsViewModel UserDetails { get; set; }
    }
}
