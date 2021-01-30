using GarbageCanApi.Interfaces;
using GarbageCanApi.Models;
using GarbageCanApi.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public bool DeletePickupRequest(PickupRequestViewModel reqModel)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<PickupRequestViewModel> GetAllRequestsByStatus(string status)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<PickupRequestViewModel> GetAllRequestsByUserId(string userId)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            reqDBModel.IdRequestDetail = reqModel.IdRequestDetail;
            reqDBModel.IdRequest = reqModel.IdRequest;
            reqDBModel.ItemCost = reqModel.ItemCost;
            reqDBModel.ItemWeight = reqModel.ItemWeight;
            reqDBModel.IdItem = reqDBModel.IdItem;

            return reqDBModel;
        }
    }
}
