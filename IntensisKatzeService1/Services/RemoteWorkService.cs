//using IntensisKatzeService1.db;
//using IntensisKatzeService1.Models;
//using IntensisKatzeService1.Models.EntityModels;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Hosting;
//using MongoDB.Driver;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.IO;
//using System.Linq;

//namespace IntensisKatzeService1.Services
//{
//    public class RemoteWorkService
//    {
//        private readonly IMongoCollection<RemoteWork> _remoteWork;
//        private readonly IMongoCollection<User> _user;


//        public RemoteWorkService(RemoteWorkDatabasesetting.IRemoteWorkDatabasesettings settings)
//        {
//            var client = new MongoClient(settings.ConnectionString);
//            var database = client.GetDatabase(settings.DatabaseName);

//            _remoteWork = database.GetCollection<RemoteWork>(settings.RemoteWorkCollectionName);
//            _user = database.GetCollection<User>("user");

//        }

//        public List<RemoteWork> Get()
//        {

//            //using (var cursor = _remoteWork.Watch())
//            //{
//            //    foreach (var change in cursor.ToEnumerable())
//            //    {
//            //        // process change event
//            //    }
//            //}

//            List<RemoteWork> remoteWork;
//            remoteWork = _remoteWork.Find(emp => true).ToList();
//            //Create(zaposleni);
//            return remoteWork;
//        }

//        public List<RemoteWork> Post(Employee employee)
//        {

//            List<RemoteWork> remoteWork;
//            remoteWork = _remoteWork.Find(emp => true).ToList();
//            //var employeeModel = GetEmployee(employee.email);
//            //var zaposleni = new Zaposleni { KorisnickoIme = employee.email, Nevidljiva = 1, RegType = 0, TerminalskoVremeRegistracije = DateTime.Now, IDNo = "0F37D097        ", Email = employee.email };
//            SyncKatze();
//           // InsertIntoKatzeForEmployee(zaposleni);
//            return remoteWork;
//        }


//        public void InsertIntoKatzeForEmployee(Zaposleni value)
//        {


//            Database db = new Database();
//            var serialize = JsonConvert.SerializeObject(value);
//            JObject jobject = JObject.Parse(serialize);
//            string query = "insert into tblReg (KorisnickoIme,Nevidljiva,RegType, TerminalskoVremeRegistracije, IDNo) values (@KorisnickoIme,@Nevidljiva,@RegType, @TerminalskoVremeRegistracije, @IDNo);";
//            var parameters = new IDataParameter[]
//           {
//                new SqlParameter("@KorisnickoIme", jobject["KorisnickoIme"].ToString()),
//                new SqlParameter("@Nevidljiva", jobject["Nevidljiva"].ToString()),
//                new SqlParameter("@RegType", jobject["RegType"].ToString()),
//                new SqlParameter("@TerminalskoVremeRegistracije", jobject["TerminalskoVremeRegistracije"].ToString()),
//                new SqlParameter("@IDNo", jobject["IDNo"].ToString()),

//           };
//            db.ExecuteData(query, parameters);

//        }

//        public ObjectResult GetKatzeData()
//        {

//            Database db = new Database();
//            string query = "select * from Employee";
//            DataTable dt = db.GetData(query);
//            var result = new ObjectResult(dt);
//            return result;
//        }


//        public string GetEmployee(string email)
//        {
//            Database db = new Database();
//            string query = "select * from Zaposleni where email='" + email + "'";
//            DataTable dt = db.GetData(query);
//            var idZaposlenog  = (from DataRow dr in dt.Rows
//                         select (string)dr["IDZaposlenog"]).FirstOrDefault();
//            string query2 = "select * from Zaposleni_IDMemorija where IDZaposlenog='" + idZaposlenog + "'";
//            DataTable dt2 = db.GetData(query2);
//            var idNo = (from DataRow dr in dt2.Rows
//                                           select (string)dr["IDNo"]).FirstOrDefault();
//            return idNo;
//            //var result = new ObjectResult(dt);
//            //return result;
//        }

//        private void SyncKatze()
//        {
//            List<RemoteWork> rwl = GetRemoteWork();
//            var rec = (from rw in rwl
//                       where rw.employeeId == "5e2021222ab79c0001dd6d95"
//                       select rw).FirstOrDefault();
//            var msg = InsertIntoKatzeForEmployee(rec);
//        }

//        public List<RemoteWork> GetRemoteWork()
//        {

//            List<RemoteWork> remoteWork;
//            remoteWork = _remoteWork.Find(emp => true).ToList();
//            //Create(zaposleni);
//            return remoteWork;
//        }

//        private string InsertIntoKatzeForEmployee(RemoteWork intensISRemoteWork)
//        {
//            var email = GetEmployeeEmail(intensISRemoteWork.employeeId);
//            //var email = GetEmployeeEmail("5e2021222ab79c0001dd6d95");
//            var memoryID = GetMemoryIdByEmail(email);
//            var msg = InsertRemoteWork(intensISRemoteWork.minutes, memoryID, "KatzeIntensisService");
//            return msg;

//        }

//        public string GetEmployeeEmail(string employeeId)
//        {
//            List<User> users;
//            users = _user.Find(emp => true).ToList();
//            var user = (from us in users
//                        where us._id == employeeId
//                        select us).FirstOrDefault();

//            return user.email;
//        }

//        public string GetMemoryIdByEmail(string email)
//        {
//            Database db = new Database();
//            string query = "select * from Zaposleni where email='" + email + "'";
//            DataTable dt = db.GetData(query);
//            var idZaposlenog = (from DataRow dr in dt.Rows
//                                select (string)dr["IDZaposlenog"]).FirstOrDefault();
//            string query2 = "select * from Zaposleni_IDMemorija where IDZaposlenog='" + idZaposlenog + "'";
//            DataTable dt2 = db.GetData(query2);
//            var idNo = (from DataRow dr in dt2.Rows
//                        select (string)dr["IDNo"]).FirstOrDefault();
//            return idNo;
//            //var result = new ObjectResult(dt);
//            //return result;
//        }

//        public string InsertRemoteWork(int minutes, string memory, string userName)
//        {


//            Database db = new Database();
//            //var serialize = JsonConvert.SerializeObject(value);
//            //JObject jobject = JObject.Parse(serialize);
//            string query = "insert into tblReg (KorisnickoIme, IDNo) values (@KorisnickoIme, @IDNo);";
//            var parameters = new IDataParameter[]
//           {
//                new SqlParameter("@KorisnickoIme", userName.ToString()),
//                new SqlParameter("@IDNo", memory.ToString()),


//           };
//            db.ExecuteData(query, parameters);
//            return "ok";

//        }


//    }
//}
