using GarbageCanApi.Interfaces;
using GarbageCanApi.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCanApi.Implementations
{
    public class ImplAssign : IAssign
    {
        public bool AssignPickups(string driverId, IEnumerable<string> requestIds)
        {
            throw new NotImplementedException();
        }

        public bool DeletePickups(IEnumerable<string> assignIds)
        {
            throw new NotImplementedException();
        }

        public bool EditPickups(string driverId, IEnumerable<string> requestIds)
        {
            throw new NotImplementedException();
        }

        public AssignViewModel GetAssignPickupsByDriverId(string driverId)
        {
            throw new NotImplementedException();
        }

        public AssignViewModel GetAssignPickupsById(string assignId)
        {
            throw new NotImplementedException();
        }
    }
}
