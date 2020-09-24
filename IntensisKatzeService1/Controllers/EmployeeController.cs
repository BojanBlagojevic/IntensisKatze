using IntensisKatzeService1.Models;
using IntensisKatzeService1.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntensisKatzeService1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeService _employeeService;

        public EmployeesController(EmployeeService employeeService)
        
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public IEnumerable<Employee> Get() =>
            _employeeService.Get();

        //[HttpGet("{id:length(24)}", Name = "GetEmployee")]
        //public ActionResult<Employee> Get(string id)
        //{
        //    var emp = _employeeService.Get(id);

        //    if (emp == null)
        //    {
        //        return NotFound();
        //    }

        //    return emp;
        //}

    }
}
