using GarbageCanApi.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCanApi.Interfaces
{
    public interface IRequest
    {
        RequestViewModel CreatePickupRequest(RequestViewModel reqModel);
        bool UpdatePickupRequest(RequestViewModel reqModel);
        bool DeletePickupRequest(RequestViewModel reqModel);
        IEnumerable<RequestViewModel> GetAllRequestsByUserId(string userId);
        RequestViewModel GetRequestsById(string requestId);
        IEnumerable<RequestViewModel> GetAllRequestsByStatus(string status);
    }
}
