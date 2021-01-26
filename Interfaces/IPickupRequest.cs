using GarbageCanApi.Models;
using GarbageCanApi.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCanApi.Interfaces
{
    public interface IPickupRequest
    {
        Request CreatePickupRequest(PickupRequestViewModel reqModel);
        bool DeletePickupRequest(PickupRequestViewModel reqModel);
        IEnumerable<PickupRequestViewModel> GetAllRequestsByUserId(string userId);
        PickupRequestViewModel GetRequestsById(string requestId);
        IEnumerable<PickupRequestViewModel> GetAllRequestsByStatus(string status);
        bool ChangeRequestStatus(string requestId, string status);
        bool UpdateRequestDetailsByDriver(PickupRequestViewModel reqModel);
    }
}
