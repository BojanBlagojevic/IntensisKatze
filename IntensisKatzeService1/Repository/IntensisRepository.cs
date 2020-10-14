using IntensisKatzeService1.Models;
using log4net;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace IntensisKatzeService1.Repository
{
    public class IntensisRepository 
    {
        private readonly IMongoCollection<RemoteWork> _remoteWork;
        private readonly IMongoCollection<User> _user;
        private readonly IMongoCollection<Employee> _employee;
        public IntensisRepository(IIntenseISDatabaseSettings settings)
        {
            if (settings.ConnectionString != null)
            {
                var client = new MongoClient(settings.ConnectionString);
                var database = client.GetDatabase(settings.DatabaseName);
                _remoteWork = database.GetCollection<RemoteWork>(settings.CollectionName);
                _user = database.GetCollection<User>("user");
                _employee = database.GetCollection<Employee>("employee");
            }
            else {
                throw new Exception($"Connection string is empty ");
            }
              
        }
        public List<RemoteWork> GetRemoteWork()
        {

            List<RemoteWork> remoteWork;
            remoteWork = _remoteWork.Find(emp => true).ToList();
            //Create(zaposleni);
            return remoteWork;
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
    }
}
