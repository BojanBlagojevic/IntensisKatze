using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntensisKatzeService1.Services;
using Microsoft.AspNetCore.Mvc;
using IntensisKatzeService1.Models;
using IntensisKatzeService1.Repository;
using Microsoft.Extensions.Configuration;

namespace IntensisKatzeService1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RemoteWorkController : ControllerBase
    {
        private readonly RemoteWorkService _remoteWorkService;
        private readonly KatzeRepository _katze;
        private readonly IConfiguration _configuration;


        public RemoteWorkController(RemoteWorkService remoteWorkService, KatzeRepository katze, IConfiguration Configuration)

        {
            _remoteWorkService = remoteWorkService;
            _katze = katze;
            _configuration = Configuration;

        }

        [HttpGet]
        public IEnumerable<RemoteWork> Get() =>
            _remoteWorkService.Get();

    
    }
}
