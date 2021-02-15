using GarbageCanApi.Interfaces;
using GarbageCanApi.Models;
using GarbageCanApi.Models.ViewModels;
using GarbageCanApi.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GarbageCanApi.Implementations
{
    public class ImplGarbageCan : IGarbageCan
    {
        private readonly databaseIEContext DbContext;
        public ImplGarbageCan(databaseIEContext DbContext)
        {
            this.DbContext = DbContext;
        }
        public UserDetail CreateUserDetails(UserDetailsViewModel udModel)
        {
            if (udModel != null)
            {
                //Adding new Id
                var id = Guid.NewGuid().ToString("N");
                udModel.IdUserDetail = id;

                var userDetails = SyncUserDetailsToUserDetailsViewModel(udModel, new UserDetail());

                DbContext.UserDetails.Add(userDetails);
                var count = DbContext.SaveChanges();

                if (count > 0)
                {
                    return DbContext.UserDetails.Where(uDetails => uDetails.IdUserDetail == id).SingleOrDefault();
                }
            }

            return null;
        }

        public IEnumerable<Item> ddlItems()
        {
            return DbContext.Items.Select(item => new Item { IdItem = item.IdItem, ItemName = item.ItemName }).ToList();
        }

        public IEnumerable<RolesViewModel> ddlRoles()
        {
            return Enum.GetValues(typeof(EnumRoles.Roles))
               .Cast<EnumRoles.Roles>()
               .Select(role => new RolesViewModel
               {
                   RoleId = (int)role,
                   RoleName = role.ToString()
               }).ToList();
        }

        public bool DeleteUserDetails(string userId)
        {
            var userDetails = DbContext.UserDetails.Where(uDetails => uDetails.IdUser == userId).SingleOrDefault();

            if (userDetails != null)
            {
                //Update in DB
                DbContext.UserDetails.Remove(userDetails);
                DbContext.SaveChanges();

                return true;
            }

            return false;
        }

        public UserDetailsViewModel GetUserDetailsById(string userId)
        {
            var userDetails = DbContext.UserDetails.Where(uDetails => uDetails.IdUser == userId).SingleOrDefault();

            if (userDetails != null)
            {
                return DbContext.UserDetails.Where(uDetails => uDetails.IdUser == userId).Select(uDetails => new UserDetailsViewModel
                {
                    IdUserDetail = uDetails.IdUserDetail.ToString(),
                    IdUser = uDetails.IdUser,
                    Address1 = uDetails.Address1,
                    Address2 = uDetails.Address2,
                    City = uDetails.City,
                    Province = uDetails.Province,
                    Country = uDetails.Country
                }).SingleOrDefault();
            }

            return null;
        }

        public bool UpdateUserDetails(UserDetailsViewModel udModel)
        {
            var userDetails = DbContext.UserDetails.Where(uDetails => uDetails.IdUser == udModel.IdUser).SingleOrDefault();

            if (userDetails != null)
            {
                var uDetails = SyncUserDetailsToUserDetailsViewModel(udModel, userDetails);
                DbContext.SaveChanges();

                return true;
            }

            return false;
        }

        private UserDetail SyncUserDetailsToUserDetailsViewModel(UserDetailsViewModel udModel, UserDetail udDBModel)
        {
            udDBModel.IdUserDetail = udModel.IdUserDetail;
            udDBModel.IdUser = udModel.IdUser;
            udDBModel.Address1 = udModel.Address1;
            udDBModel.Address2 = udModel.Address2;
            udDBModel.City = udModel.City;
            udDBModel.Province = udModel.Province;
            udDBModel.Country = udModel.Country;

            return udDBModel;
        }
    }
}
