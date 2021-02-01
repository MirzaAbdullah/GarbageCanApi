using GarbageCanApi.Models;
using GarbageCanApi.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCanApi.Interfaces
{
    public interface IGarbageCan
    {
        UserDetailsViewModel GetUserDetailsById(string userId);
        UserDetail CreateUserDetails(UserDetailsViewModel udModel);
        bool UpdateUserDetails(UserDetailsViewModel udModel);
        bool DeleteUserDetails(UserDetailsViewModel udModel);
        IEnumerable<RolesViewModel> ddlRoles();
        IEnumerable<Item> ddlItems();
    }
}
