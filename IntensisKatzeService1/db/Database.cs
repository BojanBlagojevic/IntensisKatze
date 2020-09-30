using log4net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace IntensisKatzeService1.db
{
    public class Database
    {
        public static string sqlDataSource = "Data Source=INT-VIZ03\\SQLEXPRESS01; Initial Catalog=Intens;Trusted_Connection=true;";

        public Database()
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                       typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
        }
    


        public DataTable GetData(string str)
        {
            var logger = LogManager.GetLogger(typeof(Database));
            logger.Info("Getting data from katze");
            DataTable objresutl = new DataTable();
            try
            {
                SqlDataReader myReader;

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(str, myCon))
                    {
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

            return objresutl;

        }
        public int ExecuteData(string str, params IDataParameter[] sqlParams)
        {
            var logger = LogManager.GetLogger(typeof(Database));
            logger.Info("Insert in katze");
            int rows = -1;
            try
            {

                using (SqlConnection conn = new SqlConnection(sqlDataSource))
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
                            rows = cmd.ExecuteNonQuery();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw ex;
               

            }

            return rows;
        }
    }




}
