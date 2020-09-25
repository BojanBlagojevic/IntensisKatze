using IntensisKatzeService1.db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using IntensisKatzeService1.Models.EntityModels;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using log4net;
using System.Xml;
using System.Reflection;
using System.IO;

namespace IntensisKatzeService1.Repository
{
    public class KatzeRepository
    {

        public KatzeRepository()
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                       typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
        }
        public string GetMemoryIdByEmail(string email)
        {
            var logger = LogManager.GetLogger(typeof(KatzeRepository));

            logger.Info("Finding idNo from Katze!");
            Database db = new Database();
            string query = "select * from Zaposleni where email='" + email + "'";
            DataTable dt = db.GetData(query);
            var idZaposlenog = (from DataRow dr in dt.Rows
                                select (string)dr["IDZaposlenog"]).FirstOrDefault();
            string query2 = "SELECT  IDNo FROM [Zaposleni_IDMemorija]" +
                                 "WHERE DatumDodeljivanja = (SELECT max(DatumDodeljivanja) from Zaposleni_IDMemorija WHERE IDZaposlenog ="
                                 + idZaposlenog + ") and IDZaposlenog =" + idZaposlenog;
            DataTable dt2 = db.GetData(query2);
            var idNo = (from DataRow dr in dt2.Rows
                        select (string)dr["IDNo"]).FirstOrDefault();
            return idNo;
            //var result = new ObjectResult(dt);
            //return result;
        }

         public string InsertRemoteWork(int minutes, string memory, string userName, DateTime? createdTime)
        {

            var logger = LogManager.GetLogger(typeof(KatzeRepository));

            logger.Info("Insert RemoteWork into katze");
            Database db = new Database();
            //var serialize = JsonConvert.SerializeObject(value);
            //JObject jobject = JObject.Parse(serialize);
            string query = "insert into tblReg (KorisnickoIme, IDNo, TerminalskoVremeRegistracije, IDLogickeAktivnosti) values (@KorisnickoIme, @IDNo, @TerminalskoVremeRegistracije, @IDLogickeAktivnosti);";
            var parameters = new IDataParameter[]
           {
                new SqlParameter("@KorisnickoIme", userName.ToString()),
                new SqlParameter("@IDNo", memory.ToString()),
                new SqlParameter("@TerminalskoVremeRegistracije", createdTime.HasValue ? createdTime.Value.AddHours(-minutes/60) : (DateTime?)null),
                new SqlParameter("@IDLogickeAktivnosti", 'U'),

           };
            db.ExecuteData(query, parameters);

            string query2 = "insert into tblReg (KorisnickoIme, IDNo, TerminalskoVremeRegistracije, IDLogickeAktivnosti) values (@KorisnickoIme, @IDNo, @TerminalskoVremeRegistracije, @IDLogickeAktivnosti);";
            var parameters2 = new IDataParameter[]
           {
                new SqlParameter("@KorisnickoIme", userName.ToString()),
                new SqlParameter("@IDNo", memory.ToString()),
                new SqlParameter("@TerminalskoVremeRegistracije", createdTime ?? (DateTime?)null),
                new SqlParameter("@IDLogickeAktivnosti", 'I'),

           };
            db.ExecuteData(query2, parameters2);
            return "ok";

        }


    }
}
