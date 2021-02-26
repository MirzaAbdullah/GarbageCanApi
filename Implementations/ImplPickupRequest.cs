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
    public class ImplPickupRequest : IPickupRequest
    {
        private readonly databaseIEContext DbContext;
        private readonly ILogger<ImplPickupRequest> _logger;
        public ImplPickupRequest(databaseIEContext DbContext, ILogger<ImplPickupRequest> logger)
        {
            this.DbContext = DbContext;
            this._logger = logger;
        }
        public bool ChangeRequestStatus(string requestId, string status)
        {
            var req = DbContext.Requests.Where(req => req.IdRequest == requestId).SingleOrDefault();
            if (req != null)
            {
                req.PickupStatus = status;

                //Update the Pickup Status
                var count = DbContext.SaveChanges();

                if (count > 0)
                {
                    return true;
                }
            }

            return false;
        }
        public PickupRequestViewModel CreatePickupRequest(PickupRequestViewModel reqModel)
        {
            if (reqModel != null)
            {
                using (var transaction = DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        //Adding new Id
                        var id = Guid.NewGuid().ToString("N");
                        reqModel.IdRequest = id;

                        //Setting Up the Pickup Status
                        reqModel.PickupStatus = EnumPickupStatus.pickupStatus.InProcess.ToString();

                        var req = SyncRequestToRequestViewModel(reqModel, new Request());

                        DbContext.Requests.Add(req);
                        var count = DbContext.SaveChanges();

                        if (count > 0)
                        {
                            foreach (PickupRequestDetailsViewModel reqDetail in reqModel.RequestDetails)
                            {
                                var reqDBModel = SyncRequestDetailToRequestDetailViewModel(reqDetail, new RequestDetail());

                                //Adding new Request Detail Id
                                reqDBModel.IdRequestDetail = Guid.NewGuid().ToString("N");

                                //Pass Master Request Id
                                reqDBModel.IdRequest = id;

                                DbContext.RequestDetails.Add(reqDBModel);
                                DbContext.SaveChanges();
                            }

                            // Commit transaction if all commands succeed, transaction will auto-rollback
                            // when disposed if either commands fails
                            transaction.Commit();
                        }

                        return GetRequestsById(id);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        _logger.LogError(ex.Message, "Exception thrown while saving request for pickup");
                    }
                }
            }

            return null;
        }
        public bool DeletePickupRequest(string requestId)
        {
            var pickupStatus = DbContext.Requests.Where(req => req.IdRequest == requestId).Select(reqStatus => reqStatus.PickupStatus).SingleOrDefault();

            if (pickupStatus == EnumPickupStatus.pickupStatus.InProcess.ToString())
            {
                using (var transaction = DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var reqDetails = DbContext.RequestDetails.Where(reqDetails => reqDetails.IdRequest == requestId).ToList();
                        DbContext.RequestDetails.RemoveRange(reqDetails);
                        DbContext.SaveChanges();

                        var req = DbContext.Requests.Where(reqDetails => reqDetails.IdRequest == requestId).SingleOrDefault();
                        DbContext.Requests.Remove(req);
                        DbContext.SaveChanges();

                        transaction.Commit();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        _logger.LogError(ex.Message, "Exception while deleting request and its details.");
                    }
                }
            }

            return false;
            
        }
        public IEnumerable<PickupRequestViewModel> GetAllRequestsByStatus(string status)
        {
            return DbContext.Requests
                            .Include(reqDetail => reqDetail.IdUserNavigation)
                            .Where(req => req.PickupStatus == status)
                            .Select(reqModel => new PickupRequestViewModel
                            {
                                IdRequest = reqModel.IdRequest,
                                IdUser = reqModel.IdUser,
                                UserData = new User
                                {
                                    IdUser = reqModel.IdUserNavigation.IdUser,
                                    Name = reqModel.IdUserNavigation.Name,
                                    FirstName = reqModel.IdUserNavigation.FirstName,
                                    LastName = reqModel.IdUserNavigation.LastName,
                                    PhoneNo = reqModel.IdUserNavigation.PhoneNo,
                                    Email = reqModel.IdUserNavigation.Email,
                                    IsVerified = reqModel.IdUserNavigation.IsVerified,
                                    IdRole = reqModel.IdUserNavigation.IdRole,
                                    IdRoleNavigation = new Role
                                    {
                                        IdRole = reqModel.IdUserNavigation.IdRoleNavigation.IdRole,
                                        RoleName = reqModel.IdUserNavigation.IdRoleNavigation.RoleName
                                    }
                                },
                                Latitudes = reqModel.Latitudes,
                                Longitudes = reqModel.Longitudes,
                                PickupDate = reqModel.PickupDate,
                                PickupStatus = reqModel.PickupStatus,
                                Pickup_Cost = reqModel.PickupCost,
                                PickupTime = reqModel.PickupTime,
                                CreatedDate = reqModel.CreatedDate,
                                IsActive = reqModel.IsActive,
                                RequestDetails = reqModel.RequestDetails.Select(reqDetails => new PickupRequestDetailsViewModel
                                {
                                    IdRequestDetail = reqDetails.IdRequestDetail,
                                    IdRequest = reqDetails.IdRequest,
                                    IdItem = reqDetails.IdItem,
                                    ItemName = reqDetails.IdItemNavigation.ItemName,
                                    ItemCost = reqDetails.ItemCost,
                                    ItemWeight = reqDetails.ItemWeight
                                }).ToList()
                            }).ToList();
        }
        public IEnumerable<PickupRequestViewModel> GetAllRequestsByUserId(string userId)
        {
            return DbContext.Requests
                            .Include(reqDetail => reqDetail.IdUserNavigation)
                            .Where(req => req.IdUser == userId)
                            .Select(reqModel => new PickupRequestViewModel
                            {
                                IdRequest = reqModel.IdRequest,
                                IdUser = reqModel.IdUser,
                                UserData = new User
                                {
                                    IdUser = reqModel.IdUserNavigation.IdUser,
                                    Name = reqModel.IdUserNavigation.Name,
                                    FirstName = reqModel.IdUserNavigation.FirstName,
                                    LastName = reqModel.IdUserNavigation.LastName,
                                    PhoneNo = reqModel.IdUserNavigation.PhoneNo,
                                    Email = reqModel.IdUserNavigation.Email,
                                    IsVerified = reqModel.IdUserNavigation.IsVerified,
                                    IdRole = reqModel.IdUserNavigation.IdRole,
                                    IdRoleNavigation = new Role
                                    {
                                        IdRole = reqModel.IdUserNavigation.IdRoleNavigation.IdRole,
                                        RoleName = reqModel.IdUserNavigation.IdRoleNavigation.RoleName
                                    }
                                },
                                Latitudes = reqModel.Latitudes,
                                Longitudes = reqModel.Longitudes,
                                PickupDate = reqModel.PickupDate,
                                PickupStatus = reqModel.PickupStatus,
                                Pickup_Cost = reqModel.PickupCost,
                                PickupTime = reqModel.PickupTime,
                                CreatedDate = reqModel.CreatedDate,
                                IsActive = reqModel.IsActive,
                                RequestDetails = reqModel.RequestDetails.Select(reqDetails => new PickupRequestDetailsViewModel
                                {
                                    IdRequestDetail = reqDetails.IdRequestDetail,
                                    IdRequest = reqDetails.IdRequest,
                                    IdItem = reqDetails.IdItem,
                                    ItemName = reqDetails.IdItemNavigation.ItemName,
                                    ItemCost = reqDetails.ItemCost,
                                    ItemWeight = reqDetails.ItemWeight
                                }).ToList()
                            }).OrderByDescending(order => order.CreatedDate).OrderByDescending(order => order.PickupDate).ToList();
        }
        public PickupRequestViewModel GetRequestsById(string requestId)
        {
            var request = DbContext.Requests.Where(req => req.IdRequest == requestId).SingleOrDefault();
            if (request != null)
            {
                return DbContext.Requests
                            .Include(reqDetail => reqDetail.IdUserNavigation)
                            .Where(req => req.IdRequest == requestId)
                            .Select(reqModel => new PickupRequestViewModel
                            {
                                IdRequest = reqModel.IdRequest,
                                IdUser = reqModel.IdUser,
                                UserData = new User
                                {
                                    IdUser = reqModel.IdUserNavigation.IdUser,
                                    Name = reqModel.IdUserNavigation.Name,
                                    FirstName = reqModel.IdUserNavigation.FirstName,
                                    LastName = reqModel.IdUserNavigation.LastName,
                                    PhoneNo = reqModel.IdUserNavigation.PhoneNo,
                                    Email = reqModel.IdUserNavigation.Email,
                                    IsVerified = reqModel.IdUserNavigation.IsVerified,
                                    CreatedDate = reqModel.IdUserNavigation.CreatedDate,
                                    IdRole = reqModel.IdUserNavigation.IdRole,
                                    IdRoleNavigation = new Role
                                    {
                                        IdRole = reqModel.IdUserNavigation.IdRoleNavigation.IdRole,
                                        RoleName = reqModel.IdUserNavigation.IdRoleNavigation.RoleName
                                    }
                                },
                                Latitudes = reqModel.Latitudes,
                                Longitudes = reqModel.Longitudes,
                                PickupDate = reqModel.PickupDate,
                                PickupStatus = reqModel.PickupStatus,
                                Pickup_Cost = reqModel.PickupCost,
                                PickupTime = reqModel.PickupTime,
                                CreatedDate = reqModel.CreatedDate,
                                IsActive = reqModel.IsActive,
                                RequestDetails = reqModel.RequestDetails.Select(reqDetails => new PickupRequestDetailsViewModel
                                {
                                    IdRequestDetail = reqDetails.IdRequestDetail,
                                    IdRequest = reqDetails.IdRequest,
                                    IdItem = reqDetails.IdItem,
                                    ItemName = reqDetails.IdItemNavigation.ItemName,
                                    ItemCost = reqDetails.ItemCost,
                                    ItemWeight = reqDetails.ItemWeight
                                }).ToList()
                            })
                            .SingleOrDefault();
            }

            return null;
        }
        public bool UpdateRequestDetailsByDriver(PickupRequestViewModel reqModel)
        {
            var reqDetails = DbContext.RequestDetails.Where(reqDetail => reqDetail.IdRequest == reqModel.IdRequest).ToList();
            if (reqDetails.Count > 0)
            {
                using (var transaction = DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        //Update Shipping Cost in Request Table
                        var req = DbContext.Requests.Where(reqId => reqId.IdRequest == reqModel.IdRequest).SingleOrDefault();

                        //Change Shipping Cost
                        req.PickupCost = reqModel.Pickup_Cost;

                        //Change Pickup Status
                        req.PickupStatus = EnumPickupStatus.pickupStatus.Completed.ToString();

                        //Update Request Table
                        DbContext.SaveChanges();


                        //Update Request Details Records
                        foreach (PickupRequestDetailsViewModel reqDetailItem in reqModel.RequestDetails)
                        {
                            var reqDBModel = DbContext.RequestDetails.Where(reqDetail => reqDetail.IdRequestDetail == reqDetailItem.IdRequestDetail).SingleOrDefault();
                            var reqDetail = SyncRequestDetailToRequestDetailViewModel(reqDetailItem, reqDBModel);

                            DbContext.SaveChanges();
                        }

                        transaction.Commit();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        _logger.LogError(ex.Message, "Exception while updating request details.");
                    }
                }
            }

            return false;
        }
        private Request SyncRequestToRequestViewModel(PickupRequestViewModel reqModel, Request reqDBModel)
        {
            reqDBModel.IdRequest = reqModel.IdRequest;
            reqDBModel.IdUser = reqModel.IdUser;
            reqDBModel.Latitudes = reqModel.Latitudes;
            reqDBModel.Longitudes = reqModel.Longitudes;
            reqDBModel.IsActive = reqModel.IsActive;
            reqDBModel.PickupDate = reqModel.PickupDate;
            reqDBModel.PickupStatus = reqModel.PickupStatus;
            reqDBModel.PickupCost = reqDBModel.PickupCost;
            reqDBModel.PickupTime = reqModel.PickupTime;
            reqDBModel.CreatedDate = reqModel.CreatedDate;

            return reqDBModel;
        }
        private RequestDetail SyncRequestDetailToRequestDetailViewModel(PickupRequestDetailsViewModel reqModel, RequestDetail reqDBModel)
        {
            //Getting item Price from database
            var itemPrice = DbContext.Items.Where(item => item.IdItem == reqModel.IdItem).Select(item => item.ItemPrice).SingleOrDefault();

            reqDBModel.IdRequestDetail = reqModel.IdRequestDetail;
            reqDBModel.IdRequest = reqModel.IdRequest;
            reqDBModel.ItemCost = reqModel.ItemWeight * itemPrice;
            reqDBModel.ItemWeight = reqModel.ItemWeight;
            reqDBModel.IdItem = reqModel.IdItem;

            return reqDBModel;
        }
    }
}
