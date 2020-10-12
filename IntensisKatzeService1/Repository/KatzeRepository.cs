using System;
using System.Data;
using System.Linq;
using MongoDB.Driver;
using System.Data.SqlClient;
using log4net;
using System.Xml;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace IntensisKatzeService1.Repository
{
    public class KatzeRepository
    {
        private readonly IConfiguration _configuration;
        public KatzeRepository(IConfiguration Configuration)
        {
            _configuration = Configuration;

        }
        public string GetMemoryIdByEmail(string email)
        {
  
            string query = "select * from Zaposleni where email='" + email + "'";
            DataTable dt = GetData(query);
            var idZaposlenog = (from DataRow dr in dt.Rows
                                select (string)dr["IDZaposlenog"]).FirstOrDefault();
            string query2 = "SELECT  IDNo FROM [Zaposleni_IDMemorija]" +
                                 "WHERE DatumDodeljivanja = (SELECT max(DatumDodeljivanja) from Zaposleni_IDMemorija WHERE IDZaposlenog ="
                                 + idZaposlenog + ") and IDZaposlenog =" + idZaposlenog;
            DataTable dt2 = GetData(query2);
            var idNo = (from DataRow dr in dt2.Rows
                        select (string)dr["IDNo"]).FirstOrDefault();
            return idNo;
        }

        public int InsertRemoteWork(int minutes, string memory, string userName, DateTime? createdTime)
        {

            string query = "insert into tblReg (KorisnickoIme, IDNo, TerminalskoVremeRegistracije, IDLogickeAktivnosti) values (@KorisnickoIme, substring(@IDNo,7,8), @TerminalskoVremeRegistracije, @IDLogickeAktivnosti);";
            var parameters = new IDataParameter[]
           {
                new SqlParameter("@KorisnickoIme", userName.ToString()),
                new SqlParameter("@IDNo", memory.ToString()),
                new SqlParameter("@TerminalskoVremeRegistracije", createdTime.HasValue ? createdTime.Value.AddHours(-minutes/60) : (DateTime?)null),
                new SqlParameter("@IDLogickeAktivnosti", 'U'),

           };
            ExecuteData(query, parameters);

            string query2 = "insert into tblReg (KorisnickoIme, IDNo, TerminalskoVremeRegistracije, IDLogickeAktivnosti) values (@KorisnickoIme, substring(@IDNo,7,8), @TerminalskoVremeRegistracije, @IDLogickeAktivnosti);";
            var parameters2 = new IDataParameter[]
           {
                new SqlParameter("@KorisnickoIme", userName.ToString()),
                new SqlParameter("@IDNo", memory.ToString()),
                new SqlParameter("@TerminalskoVremeRegistracije", createdTime ?? (DateTime?)null),
                new SqlParameter("@IDLogickeAktivnosti", 'I'),

           };
            var cnt = ExecuteData(query2, parameters2);
            return cnt;

        }

        public DataTable GetData(string str)
        {
            var logger = LogManager.GetLogger(typeof(KatzeRepository));
            logger.Info("Getting data from katze");
            DataTable objresutl = new DataTable();
            try
            {
                SqlDataReader myReader;

                using (SqlConnection myCon = new SqlConnection(_configuration["ConnectionString"]))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(str, myCon))
                    {
                        logger.Info("Connection is open");
                        logger.Info("ExecuteReader");
                        myReader = myCommand.ExecuteReader();
                        objresutl.Load(myReader);

                        myReader.Close();
                        myCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                throw ex;

            }

            logger.Info(objresutl.Rows);
            return objresutl;

        }
        public int ExecuteData(string str, params IDataParameter[] sqlParams)
        {
            var logger = LogManager.GetLogger(typeof(KatzeRepository));
            logger.Info("Insert in katze");

            using (SqlConnection conn = new SqlConnection(_configuration["ConnectionString"]))
            {
                conn.Open();
           
                using (SqlCommand cmd = new SqlCommand(str, conn))
                {
                    if (sqlParams != null)
                    {
                        foreach (IDataParameter para in sqlParams)
                        {
                            cmd.Parameters.Add(para);
                        }
                        try
                         {
                            var cnt = cmd.ExecuteNonQuery();                       
                          return cnt;
                        } catch (Exception ex)
                        {                        
                            throw ex;
                        }
                    }
                    return 0;
                }
            }
        }


        public bool IsEntryInKatze(DateTime? dateOfActivity)
        {
            string commandText1 = "SELECT * FROM tblReg WHERE KorisnickoIme='KatzeIntensisService' and TerminalskoVremeRegistracije='" + dateOfActivity + "'";

            using (SqlConnection conn = new SqlConnection(_configuration["ConnectionString"]))
            {
                SqlCommand cmd = new SqlCommand(commandText1, conn);
                conn.Open();
                cmd.Parameters.Add("@DateOfActivity", System.Data.SqlDbType.DateTime);
                cmd.Parameters["@DateOfActivity"].Value = dateOfActivity;
                var katzeEntry = cmd.ExecuteScalar();

                return katzeEntry == null ? false : true;
            }
        }


    }
}
