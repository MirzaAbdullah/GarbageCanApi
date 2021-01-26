using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCanApi.Utilities
{
    public static class EnumPickupStatus
    {
        public enum pickupStatus { 
            InProcess,
            AssignedToDriver,
            OutForPickup,
            Completed
        }
    }
}
