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
    public class RequestController : ControllerBase
    {
        private readonly IRequest IRequestServices;
        private readonly ILogger<RequestController> _logger;

        /// <summary>
        /// Constructor method resposible for DI between interfaces
        /// </summary>
        /// <param name="IRequestServices">Pass interface class of type ISuecrity which is responsbile for all security related operations</param>
        /// <param name="logger">Interface for NLog's</param>
        public RequestController(IRequest IRequestServices, ILogger<RequestController> logger)
        {
            this.IRequestServices = IRequestServices;
            _logger = logger;
        }
    }
}
