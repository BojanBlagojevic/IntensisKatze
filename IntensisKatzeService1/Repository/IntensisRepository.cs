using IntensisKatzeService1.db;
using IntensisKatzeService1.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace IntensisKatzeService1.Repository
{
    public class IntensisRepository
    {
        private readonly IMongoCollection<RemoteWork> _remoteWork;
        private readonly IMongoCollection<User> _user;
        private readonly IMongoCollection<Employee> _employee;

        public IntensisRepository(RemoteWorkDatabasesetting.IRemoteWorkDatabasesettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _remoteWork = database.GetCollection<RemoteWork>(settings.RemoteWorkCollectionName);
            _user = database.GetCollection<User>("user");
            _employee = database.GetCollection<Employee>("employee");
        }

        public List<RemoteWork> GetRemoteWork()
        {

            List<RemoteWork> remoteWork;
            remoteWork = _remoteWork.Find(emp => true).ToList();
            //Create(zaposleni);
            return remoteWork;
        }

        public string GetMemoryIdByEmail(string email)
        {
         
            Database db = new Database();
            string query = "select * from Zaposleni where email='" + email + "'";
            DataTable dt = db.GetData(query);
            var idZaposlenog = (from DataRow dr in dt.Rows
                                select (string)dr["IDZaposlenog"]).FirstOrDefault();
            string query2 = "select * from Zaposleni_IDMemorija where IDZaposlenog='" + idZaposlenog + "'";
            DataTable dt2 = db.GetData(query2);
            var idNo = (from DataRow dr in dt2.Rows
                        select (string)dr["IDNo"]).FirstOrDefault();
            return idNo;
            //var result = new ObjectResult(dt);
            //return result;
        }

        public string GetEmployeeEmail(string employeeId)
        {
            List<Employee> employee;
            employee = _employee.Find(emp => true).ToList();
            var userId = (from us in employee
                        where us.Id == employeeId
                        select us.user).FirstOrDefault();

            List<User> users;
            users = _user.Find(emp => true).ToList();
            var userEmail = (from us in users
                         where us._id == userId
                             select us.email).FirstOrDefault();

            return userEmail;
        }

        //public string GetEmployeeEmail(string id)
        //{

        //}

    }
}
