using System;
using System.Collections.Generic;

#nullable disable

namespace GarbageCanApi.Models
{
    public partial class User
    {
        public User()
        {
            Assigns = new HashSet<Assign>();
            Requests = new HashSet<Request>();
            UserDetails = new HashSet<UserDetail>();
        }

        public string IdUser { get; set; }
        public int? IdRole { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public bool? IsVerified { get; set; }
        public int? CodeVerification { get; set; }

        public virtual Role IdRoleNavigation { get; set; }
        public virtual ICollection<Assign> Assigns { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
        public virtual ICollection<UserDetail> UserDetails { get; set; }
    }
}
