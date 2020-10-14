using System;
using System.Data;
using System.Linq;
using MongoDB.Driver;
using System.Data.SqlClient;
using log4net;
using System.Xml;
using System.Reflection;
using IntensisKatzeService1.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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
            string commandText1 = "SELECT IDZaposlenog FROM Zaposleni WHERE email = @email";


            string commandText2 = "SELECT  substring(IDNo,7,8) FROM [Zaposleni_IDMemorija]"
                                + " WHERE DatumDodeljivanja = (SELECT max(DatumDodeljivanja) from Zaposleni_IDMemorija WHERE IDZaposlenog = @IDZaposlenog)"
                                + "  and IDZaposlenog = @IDZaposlenog  ";


            using (SqlConnection conn = new SqlConnection(_configuration["ConnectionString"]))
            {
                string memoryID;
                SqlCommand cmd = new SqlCommand(commandText1, conn);
                conn.Open();
                cmd.Parameters.AddWithValue("@email", email);
                var katzeEmployeeID = cmd.ExecuteScalar();
                if (katzeEmployeeID != null)
                {
                    cmd.CommandText = commandText2;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@IDZaposlenog", katzeEmployeeID.ToString());
                    var objmemID = cmd.ExecuteScalar();
                    memoryID = objmemID == null ? throw new Exception($"Can't find memory id for {email}") : objmemID.ToString();
                }
                else
                    throw new Exception($"Can't find katze id for {email} ");

                return memoryID;
            }
        }

        public int InsertRemoteWork(int minutes, string memory, string userName, DateTime? createdTime)
        {

            string commandTextU = "INSERT INTO [dbo].[tblReg] ([IDNo] ,[KorisnickoIme] ,[IDTerminala] ,[TerminalskoVremeRegistracije],"
                                + "[IDLogickeAktivnosti],[Nevidljiva],[RegType]) "
                                + " VALUES(@MemoryID, @UserName, -10,  DATEADD(minute,-@Minutes,@EntryDateTime)  ,"
                                + " 'U', 0, 0) ";
            string commandTextI = "INSERT INTO [dbo].[tblReg] ([IDNo] ,[KorisnickoIme] ,[IDTerminala] ,[TerminalskoVremeRegistracije],"
                                + "[IDLogickeAktivnosti],[Nevidljiva],[RegType]) "
                                + " VALUES(@MemoryID,  @UserName, -10, @EntryDateTime ,"
                                + " 'I', 0, 0) ";
            using (SqlConnection conn = new SqlConnection(_configuration["ConnectionString"]))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction("InsertKatze");
                using (SqlCommand cmd = new SqlCommand(commandTextU, conn))
                {
                    cmd.Transaction = transaction;
                    cmd.Parameters.AddWithValue("@MemoryID", memory);
                    cmd.Parameters.Add("@EntryDateTime", System.Data.SqlDbType.DateTime);
                    cmd.Parameters["@EntryDateTime"].Value = createdTime;
                    cmd.Parameters.AddWithValue("@Minutes", minutes);
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    try
                    {
                        var cnt = cmd.ExecuteNonQuery();
                        cmd.CommandText = commandTextI;
                        cnt = cmd.ExecuteNonQuery();
                        transaction.Commit();
                        return cnt;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }

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

        /// <summary>
        /// Poziv uskladištene procedure
        /// </summary>
        /// <param name="rf">Parametri za filter</param>
        /// <returns>Lista zaposlenih</returns>
        public List<EmployeeReport> GetEmployeeReport(ReportFilter rf)
        {
            List<EmployeeReport> list = new List<EmployeeReport>();

            string storedProcedure = "p_CW_ObracunSatniceZaPeriod";

            using (SqlConnection conn = new SqlConnection(_configuration["ConnectionString"]))
            {
                SqlCommand cmd = new SqlCommand(storedProcedure, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                cmd.Parameters.AddWithValue("VremeOd", rf.StartDate);
                cmd.Parameters.AddWithValue("VremeDo", rf.EndDate);
                cmd.Parameters.AddWithValue("Operator", rf.Operation);
                cmd.Parameters.AddWithValue("Sati", rf.Hours);

                using (var reader = cmd.ExecuteReader())
                {
                    list = reader.Cast<IDataRecord>()
                        .Select(x => new EmployeeReport(
                                        x.GetInt32(0),
                                        x.GetString(1),
                                        x.GetDateTime(2),
                                        x.GetString(3),
                                        x.GetInt32(4),
                                        x.GetString(5)
                                    )
                                )
                        .ToList();
                }
            }

            return list;
        }
    }
}
