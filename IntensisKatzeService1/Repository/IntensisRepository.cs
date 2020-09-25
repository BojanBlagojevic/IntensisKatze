using IntensisKatzeService1.db;
using IntensisKatzeService1.Models;
using log4net;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

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

            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                       typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

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


        public string GetEmployeeEmail(string employeeId)
        {


            var logger = LogManager.GetLogger(typeof(IntensisRepository));

            logger.Info("Find employee email!");
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
