using GarbageCanApi.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GarbageCanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignController : ControllerBase
    {
        private readonly IAssign IAssignServices;
        private readonly ILogger<AssignController> _logger;

        /// <summary>
        /// Constructor method resposible for DI between interfaces
        /// </summary>
        /// <param name="IAssignServices">Pass interface class of type ISuecrity which is responsbile for all security related operations</param>
        /// <param name="logger">Interface for NLog's</param>
        public AssignController(IAssign IAssignServices, ILogger<AssignController> logger)
        {
            this.IAssignServices = IAssignServices;
            _logger = logger;
        }
    }
}
