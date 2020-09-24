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


namespace IntensisKatzeService1.Repository
{
    public class KatzeRepository
    {
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

         public string InsertRemoteWork(int minutes, string memory, string userName, DateTime? createdTime)
        {


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
