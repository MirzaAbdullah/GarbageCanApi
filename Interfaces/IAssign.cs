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
        bool EditPickups(string driverId, IEnumerable<string> requestIds);
        bool DeletePickups(IEnumerable<string> assignIds);
        AssignViewModel GetAssignPickupsById(string assignId);
        AssignViewModel GetAssignPickupsByDriverId(string driverId);
    }
}
