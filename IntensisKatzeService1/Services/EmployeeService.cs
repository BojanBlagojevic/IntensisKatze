//using IntensisKatzeService1.Models;
//using MongoDB.Driver;
//using System.Collections.Generic;
//using System.Linq;

//namespace IntensisKatzeService1.Services
//{
//    public class EmployeeService
//    {
//        private readonly IMongoCollection<Employee> _employees;
//        public EmployeeService(EmployeeDatabasesetting.IEmployeeDatabaseSettings settings)
//        {
//            var client = new MongoClient(settings.ConnectionString);
//            var database = client.GetDatabase(settings.DatabaseName);


//            _employees = database.GetCollection<Employee>(settings.EmployeesCollectionName);

//        }

//        public List<Employee> Get()
//        {

//            List<Employee> employees;
//            employees = _employees.Find(emp => true).ToList();
//            return employees;
//        }

//        //public Employee Get(string id) =>
//        //    _employees.Find<Employee>(emp => emp.Id.ToString() == id).FirstOrDefault();

//    }
//}
