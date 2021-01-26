using GarbageCanApi.Interfaces;
using GarbageCanApi.Models;
using GarbageCanApi.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCanApi.Implementations
{
    public class ImplPickupRequest : IPickupRequest
    {
        private readonly databaseIEContext DbContext;
        public ImplPickupRequest(databaseIEContext DbContext)
        {
            this.DbContext = DbContext;
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

        public Request CreatePickupRequest(PickupRequestViewModel reqModel)
        {
            if (reqModel != null)
            {
                //Adding new Id
                var id = Guid.NewGuid().ToString("N");
                reqModel.IdRequest = id;

                var req = SyncRequestToRequestViewModel(reqModel, new Request());

                DbContext.Requests.Add(req);
                var count = DbContext.SaveChanges();

                if (count > 0)
                {
                    /*
                     * Write Code for requestDetails
                     */

                    return DbContext.Requests.Where(req => req.IdRequest == id).SingleOrDefault();
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
            throw new NotImplementedException();
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
    }
}
