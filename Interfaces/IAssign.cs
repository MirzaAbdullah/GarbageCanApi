using GarbageCanApi.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCanApi.Interfaces
{
    public interface IAssign
    {
        bool AssignPickups(string driverId, IEnumerable<string> requestIds);
        bool AcceptAssignedPickup(string assignId);
        AssignViewModel GetAssignPickupsById(string assignId);
        IEnumerable<AssignViewModel> GetAllAssignPickupsByDriverId(string driverId);
        IEnumerable<AssignViewModel> GetInProcAssignPickupsByDriverId(string driverId);
    }
}
