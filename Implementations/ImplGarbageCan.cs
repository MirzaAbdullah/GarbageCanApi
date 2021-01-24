using GarbageCanApi.Interfaces;
using GarbageCanApi.Models;
using GarbageCanApi.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                udModel.IdUserDetail = Convert.ToInt32(id);

                var userDetails = SyncUserDetailsToUserDetailsViewModel(udModel);

                DbContext.UserDetails.Add(userDetails);
                var count = DbContext.SaveChanges();

                if (count > 0)
                {
                    return DbContext.UserDetails.Where(uDetails => uDetails.IdUserDetail == Convert.ToInt32(id)).SingleOrDefault();
                }
            }

            return null;
        }

        public bool DeleteUserDetails(UserDetailsViewModel udModel)
        {
            var userDetails = DbContext.UserDetails.Where(uDetails => uDetails.IdUser == udModel.IdUser).SingleOrDefault();

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
                    IdUserDetail = uDetails.IdUserDetail,
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
                var uDetails = SyncUserDetailsToUserDetailsViewModel(udModel);

                //Update in DB
                DbContext.UserDetails.Update(uDetails);
                DbContext.SaveChanges();

                return true;
            }

            return false;
        }

        private UserDetail SyncUserDetailsToUserDetailsViewModel(UserDetailsViewModel udModel)
        {
            return new UserDetail { 
                IdUserDetail = udModel.IdUserDetail,
                IdUser = udModel.IdUser,
                Address1 = udModel.Address1,
                Address2 = udModel.Address2,
                City = udModel.City,
                Province = udModel.Province,
                Country = udModel.Country
            };
        }
    }
}
