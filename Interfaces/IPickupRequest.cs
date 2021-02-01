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
        PickupRequestViewModel CreatePickupRequest(PickupRequestViewModel reqModel);
        bool DeletePickupRequest(string requestId);
        IEnumerable<PickupRequestViewModel> GetAllRequestsByUserId(string userId);
        PickupRequestViewModel GetRequestsById(string requestId);
        IEnumerable<PickupRequestViewModel> GetAllRequestsByStatus(string status);
        bool ChangeRequestStatus(string requestId, string status);
        bool UpdateRequestDetailsByDriver(PickupRequestViewModel reqModel);
    }
}
