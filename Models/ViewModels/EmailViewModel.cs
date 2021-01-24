using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCanApi.Models.ViewModels
{
    public class EmailViewModel
    {
        public string to { get; set; }
        public string from { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
    }
}
