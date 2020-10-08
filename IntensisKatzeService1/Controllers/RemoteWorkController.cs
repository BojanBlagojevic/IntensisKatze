//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using IntensisKatzeService1.Services;
//using Microsoft.AspNetCore.Mvc;
//using IntensisKatzeService1.Models;

//namespace IntensisKatzeService1.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class RemoteWorkController : ControllerBase
//    {
//        private readonly RemoteWorkService _remoteWorkService;
  

//        public RemoteWorkController(RemoteWorkService remoteWorkService)

//        {
//            _remoteWorkService = remoteWorkService;
            
//        }

//        [HttpGet]
//        public IEnumerable<RemoteWork> Get() =>
//            _remoteWorkService.Get();

//        //[HttpPost]
//        //public async Task<ActionResult<Employee>> PostTodoItem(Employee employee)
//        //{
//        //    _context.TodoItems.Add(todoItem);
//        //    await _context.SaveChangesAsync();

//        //    //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
//        //    return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
//        //}

//        [HttpPost]
//        public IEnumerable<RemoteWork> Post(Employee employee) =>
//          _remoteWorkService.Post(employee);


//        //[HttpGet("{id:length(24)}", Name = "GetEmployee")]
//        //public ActionResult<Employee> Get(string id)
//        //{
//        //    var emp = _employeeService.Get(id);

//        //    if (emp == null)
//        //    {
//        //        return NotFound();
//        //    }

//        //    return emp;
//        //}


//    }
//}
