using GarbageCanApi.Interfaces;
using GarbageCanApi.Models;
using GarbageCanApi.Models.ViewModels;
using GarbageCanApi.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GarbageCanApi.Implementations
{
    public class ImplAssign : IAssign
    {
        private readonly databaseIEContext DbContext;
        private readonly ILogger<ImplAssign> _logger;
        private readonly Email email;
        public ImplAssign(databaseIEContext DbContext, ILogger<ImplAssign> logger, Email email)
        {
            this.DbContext = DbContext;
            this._logger = logger;
            this.email = email;
        }

        public bool AcceptAssignedPickup(string assignId)
        {
            try
            {
                var assignedPickup = DbContext.Assigns.Where(assign => assign.IdAssign == assignId).SingleOrDefault();

                var reqModel = DbContext.Requests.Where(req => req.IdRequest == assignedPickup.IdRequest).SingleOrDefault();
                reqModel.PickupStatus = EnumPickupStatus.pickupStatus.OutForPickup.ToString();
                DbContext.SaveChanges();

                //Send Email
                var body = @"The Driver is out for picking up the garbage and will be at your doorstep in approx 1-4 hours";

                EmailViewModel emailObj = new EmailViewModel()
                {
                    to = DbContext.Users.Where(user => user.IdUser == reqModel.IdUser).Select(user => user.Email).SingleOrDefault(),
                    subject = "Welcome",
                    body = email.htmlEmailBody(body)
                };

                email.sendEmail(emailObj);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Exception while accepting the assignment");
            }

            return false;
        }

        public bool AssignPickups(string driverId, IEnumerable<string> requestIds)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (string reqId in requestIds)
                    {
                        //Change Pickup Status
                        var req = DbContext.Requests.Where(reqIds => reqIds.IdRequest == reqId).SingleOrDefault();
                        req.PickupStatus = EnumPickupStatus.pickupStatus.AssignedToDriver.ToString();

                        //Update the Request Table for Pickup Status
                        DbContext.SaveChanges();

                        DbContext.Assigns.Add(new Assign {
                            IdAssign = Guid.NewGuid().ToString("N"),
                            IdUser = driverId,
                            IdRequest = reqId,
                            CreatedDate = DateTime.Now
                        });
                        DbContext.SaveChanges();
                    }

                    //Commit the transaction
                    transaction.Commit();

                    return true;

                }
                catch (Exception ex)
                {
                    //Rollback the transaction
                    transaction.Rollback();

                    _logger.LogError(ex.Message, "Exception while assigning pickups to driver.");
                }
            }

            return false;
        }

        public IEnumerable<AssignViewModel> GetAllAssignPickupsByDriverId(string driverId)
        {
            return DbContext.Assigns
                .Include(assign => assign.IdRequestNavigation)
                .Include(assign => assign.IdUserNavigation)
                .Where(assign => assign.IdUser == driverId && (assign.IdRequestNavigation.PickupStatus == EnumPickupStatus.pickupStatus.AssignedToDriver.ToString() || assign.IdRequestNavigation.PickupStatus == EnumPickupStatus.pickupStatus.OutForPickup.ToString()))
                .Select(assign => new AssignViewModel { 
                    IdAssign = assign.IdAssign,
                    IdUser = assign.IdUser,
                    IdRequest = assign.IdRequest,
                    CreatedDate = assign.CreatedDate,
                    User = new UserViewModel
                    {
                        IdUser = assign.IdUserNavigation.IdUser,
                        Name = assign.IdUserNavigation.Name,
                        Email = assign.IdUserNavigation.Email,
                        FirstName = assign.IdUserNavigation.FirstName,
                        LastName = assign.IdUserNavigation.LastName,
                        PhoneNo = assign.IdUserNavigation.PhoneNo,
                        IsVerified = assign.IdUserNavigation.IsVerified,
                        CreatedDate = assign.IdUserNavigation.CreatedDate,
                        IdRole = assign.IdUserNavigation.IdRole,
                        NameRole = assign.IdUserNavigation.IdRoleNavigation.RoleName
                    },
                    UserDetails = DbContext.UserDetails.Where(user => user.IdUser == assign.IdUserNavigation.IdUser).Select(userDetails => new UserDetailsViewModel { 
                        IdUser = userDetails.IdUser,
                        Address1 = userDetails.Address1,
                        Address2 = userDetails.Address2,
                        City = userDetails.City,
                        Province = userDetails.Province,
                        Country = userDetails.Country
                    }).SingleOrDefault(),
                    Request = new PickupRequestViewModel
                    {
                        IdRequest = assign.IdRequestNavigation.IdRequest,
                        IdUser = assign.IdRequestNavigation.IdUser,
                        Latitudes = assign.IdRequestNavigation.Latitudes,
                        Longitudes = assign.IdRequestNavigation.Longitudes,
                        PickupDate = assign.IdRequestNavigation.PickupDate,
                        PickupStatus = assign.IdRequestNavigation.PickupStatus,
                        PickupTime = assign.IdRequestNavigation.PickupTime,
                        Pickup_Cost = assign.IdRequestNavigation.PickupCost,
                        IsActive = assign.IdRequestNavigation.IsActive,
                        CreatedDate = assign.IdRequestNavigation.CreatedDate,
                        RequestDetails = DbContext.RequestDetails.Where(reqDetail => reqDetail.IdRequest == assign.IdRequestNavigation.IdRequest).Select(reqDetail => new PickupRequestDetailsViewModel { 
                            IdRequestDetail = reqDetail.IdRequestDetail,
                            IdRequest = reqDetail.IdRequest,
                            IdItem = reqDetail.IdItem,
                            ItemName = reqDetail.IdItemNavigation.ItemName,
                            ItemCost = reqDetail.ItemCost,
                            ItemWeight = reqDetail.ItemWeight
                        }).ToList(),
                        UserData = new User
                        {
                            FirstName = assign.IdRequestNavigation.IdUserNavigation.FirstName,
                            LastName = assign.IdRequestNavigation.IdUserNavigation.LastName,
                            UserDetails = DbContext.UserDetails.Where(userId => userId.IdUser == assign.IdRequestNavigation.IdUser).ToList()
                        }
                    }
                })
                .ToList();
        }

        public AssignViewModel GetAssignPickupsById(string assignId)
        {
            return DbContext.Assigns
               .Include(assign => assign.IdRequestNavigation)
               .Include(assign => assign.IdUserNavigation)
               .Where(assign => assign.IdAssign == assignId)
               .Select(assign => new AssignViewModel
               {
                   IdAssign = assign.IdAssign,
                   IdUser = assign.IdUser,
                   IdRequest = assign.IdRequest,
                   CreatedDate = assign.CreatedDate,
                   User = new UserViewModel
                   {
                       IdUser = assign.IdUserNavigation.IdUser,
                       Name = assign.IdUserNavigation.Name,
                       Email = assign.IdUserNavigation.Email,
                       FirstName = assign.IdUserNavigation.FirstName,
                       LastName = assign.IdUserNavigation.LastName,
                       PhoneNo = assign.IdUserNavigation.PhoneNo,
                       IsVerified = assign.IdUserNavigation.IsVerified,
                       CreatedDate = assign.IdUserNavigation.CreatedDate,
                       IdRole = assign.IdUserNavigation.IdRole,
                       NameRole = assign.IdUserNavigation.IdRoleNavigation.RoleName
                   },
                   UserDetails = DbContext.UserDetails.Where(user => user.IdUser == assign.IdUserNavigation.IdUser).Select(userDetails => new UserDetailsViewModel
                   {
                       IdUser = userDetails.IdUser,
                       Address1 = userDetails.Address1,
                       Address2 = userDetails.Address2,
                       City = userDetails.City,
                       Province = userDetails.Province,
                       Country = userDetails.Country
                   }).SingleOrDefault(),
                   Request = new PickupRequestViewModel
                   {
                       IdRequest = assign.IdRequestNavigation.IdRequest,
                       IdUser = assign.IdRequestNavigation.IdUser,
                       Latitudes = assign.IdRequestNavigation.Latitudes,
                       Longitudes = assign.IdRequestNavigation.Longitudes,
                       PickupDate = assign.IdRequestNavigation.PickupDate,
                       PickupStatus = assign.IdRequestNavigation.PickupStatus,
                       PickupTime = assign.IdRequestNavigation.PickupTime,
                       Pickup_Cost = assign.IdRequestNavigation.PickupCost,
                       IsActive = assign.IdRequestNavigation.IsActive,
                       CreatedDate = assign.IdRequestNavigation.CreatedDate,
                       RequestDetails = DbContext.RequestDetails.Where(reqDetail => reqDetail.IdRequest == assign.IdRequestNavigation.IdRequest).Select(reqDetail => new PickupRequestDetailsViewModel
                       {
                           IdRequestDetail = reqDetail.IdRequestDetail,
                           IdRequest = reqDetail.IdRequest,
                           IdItem = reqDetail.IdItem,
                           ItemName = reqDetail.IdItemNavigation.ItemName,
                           ItemCost = reqDetail.ItemCost,
                           ItemWeight = reqDetail.ItemWeight
                       }).ToList()
                   }
               })
               .SingleOrDefault();
        }

        public IEnumerable<AssignViewModel> GetInProcAssignPickupsByDriverId(string driverId)
        {
            return DbContext.Assigns
               .Include(assign => assign.IdRequestNavigation)
               .Include(assign => assign.IdUserNavigation)
               .Where(assign => assign.IdUser == driverId && assign.IdRequestNavigation.PickupStatus == EnumPickupStatus.pickupStatus.InProcess.ToString())
               .Select(assign => new AssignViewModel
               {
                   IdAssign = assign.IdAssign,
                   IdUser = assign.IdUser,
                   IdRequest = assign.IdRequest,
                   CreatedDate = assign.CreatedDate,
                   User = new UserViewModel
                   {
                       IdUser = assign.IdUserNavigation.IdUser,
                       Name = assign.IdUserNavigation.Name,
                       Email = assign.IdUserNavigation.Email,
                       FirstName = assign.IdUserNavigation.FirstName,
                       LastName = assign.IdUserNavigation.LastName,
                       PhoneNo = assign.IdUserNavigation.PhoneNo,
                       IsVerified = assign.IdUserNavigation.IsVerified,
                       CreatedDate = assign.IdUserNavigation.CreatedDate,
                       IdRole = assign.IdUserNavigation.IdRole,
                       NameRole = assign.IdUserNavigation.IdRoleNavigation.RoleName
                   },
                   UserDetails = DbContext.UserDetails.Where(user => user.IdUser == assign.IdUserNavigation.IdUser).Select(userDetails => new UserDetailsViewModel
                   {
                       IdUser = userDetails.IdUser,
                       Address1 = userDetails.Address1,
                       Address2 = userDetails.Address2,
                       City = userDetails.City,
                       Province = userDetails.Province,
                       Country = userDetails.Country
                   }).SingleOrDefault(),
                   Request = new PickupRequestViewModel
                   {
                       IdRequest = assign.IdRequestNavigation.IdRequest,
                       IdUser = assign.IdRequestNavigation.IdUser,
                       Latitudes = assign.IdRequestNavigation.Latitudes,
                       Longitudes = assign.IdRequestNavigation.Longitudes,
                       PickupDate = assign.IdRequestNavigation.PickupDate,
                       PickupStatus = assign.IdRequestNavigation.PickupStatus,
                       PickupTime = assign.IdRequestNavigation.PickupTime,
                       Pickup_Cost = assign.IdRequestNavigation.PickupCost,
                       IsActive = assign.IdRequestNavigation.IsActive,
                       CreatedDate = assign.IdRequestNavigation.CreatedDate,
                       RequestDetails = DbContext.RequestDetails.Where(reqDetail => reqDetail.IdRequest == assign.IdRequestNavigation.IdRequest).Select(reqDetail => new PickupRequestDetailsViewModel
                       {
                           IdRequestDetail = reqDetail.IdRequestDetail,
                           IdRequest = reqDetail.IdRequest,
                           IdItem = reqDetail.IdItem,
                           ItemName = reqDetail.IdItemNavigation.ItemName,
                           ItemCost = reqDetail.ItemCost,
                           ItemWeight = reqDetail.ItemWeight
                       }).ToList()
                   }
               })
               .ToList();
        }
    }
}
