using GarbageCanApi.Interfaces;
using GarbageCanApi.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCanApi.Implementations
{
    public class ImplRequest : IRequest
    {
        public bool ChangeRequestStatus(string requestId, string status)
        {
            throw new NotImplementedException();
        }

        public RequestViewModel CreatePickupRequest(RequestViewModel reqModel)
        {
            throw new NotImplementedException();
        }

        public bool DeletePickupRequest(RequestViewModel reqModel)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RequestViewModel> GetAllRequestsByStatus(string status)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RequestViewModel> GetAllRequestsByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public RequestViewModel GetRequestsById(string requestId)
        {
            throw new NotImplementedException();
        }

        public bool UpdatePickupRequest(RequestViewModel reqModel)
        {
            throw new NotImplementedException();
        }
    }
}
