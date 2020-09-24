using IntensisKatzeService1.db;
using IntensisKatzeService1.Models;
using IntensisKatzeService1.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IntensisKatzeService1.Services
{

    public class SyncRemoteWorkService
   : BackgroundService
    {
        private readonly IntensisRepository _intensis;
        private readonly KatzeRepository _katze;
        private readonly ILogger<SyncRemoteWorkService> _logger;
        private readonly IConfiguration _configuration;
        public SyncRemoteWorkService(IntensisRepository intensis, KatzeRepository katze, ILogger<SyncRemoteWorkService> logger, IConfiguration Configuration)
        {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _intensis = intensis;
            _katze = katze;
            _configuration = Configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug(" background task is doing background work.");


                var vreme = DateTime.Now;
                _logger.LogInformation("----- Insert: {vreme}", vreme.ToString());
                try
                { SyncKatze(); }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

                await Task.Delay(6000, stoppingToken);
            }
        }

        private string InsertIntoKatzeForEmployee(RemoteWork intensISRemoteWork)
        {
            var email = _intensis.GetEmployeeEmail(intensISRemoteWork.employeeId);
            //var email = GetEmployeeEmail("5e2021222ab79c0001dd6d95");
            var memoryID = _katze.GetMemoryIdByEmail(email);
            var msg = _katze.InsertRemoteWork(intensISRemoteWork.minutes, memoryID, "KatzeIntensisService", intensISRemoteWork.CreatedAt);
            return msg;
          
        }


        private void SyncKatze()
        {

            List<RemoteWork> rwl = _intensis.GetRemoteWork();
          

            foreach (var rwork in rwl.Where(a => a.CreatedAt.Value.Date == DateTime.Now.Date))
            {
                Database db = new Database();
                string query = "select * from tblReg where KorisnickoIme='KatzeIntensisService' and TerminalskoVremeRegistracije='" + rwork.CreatedAt + "'";
                DataTable dt = db.GetData(query);
                var tVremeRegistracije = (from DataRow dr in dt.Rows
                                    select dr["TerminalskoVremeRegistracije"]).FirstOrDefault();

                if (tVremeRegistracije == null)
                {
                   
                    var msg = InsertIntoKatzeForEmployee(rwork);
                }
            }
      
        }


    }
}

