using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntensisKatzeService1.Models;
using IntensisKatzeService1.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace cardwareAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeeReportController : ControllerBase
    {
        private readonly KatzeRepository _katze;
        private readonly IConfiguration _configuration;

        public EmployeeReportController(KatzeRepository kr, IConfiguration c)
        {
            _katze = kr;
            _configuration = c;
        }

        // GET <EmployeeReportController>/filter?

        [HttpGet("filter")]
        public string Get(DateTime startDate, DateTime endDate, string operation, int hours)
        {
            ReportFilter rf = new ReportFilter(startDate, endDate, operation, hours);
            List <EmployeeReport> list = _katze.GetEmployeeReport(rf);

            var json = JsonSerializer.Serialize(list, typeof(List<EmployeeReport>), 
                new JsonSerializerOptions() { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All) }); //ovo treba zbog non-Unicode izvornog enkodiranja

            return json;
        }
    }
}
