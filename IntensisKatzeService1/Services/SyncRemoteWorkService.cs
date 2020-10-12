using IntensisKatzeService1.Models;
using IntensisKatzeService1.Repository;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace IntensisKatzeService1.Services
{

    public class SyncRemoteWorkService
   : BackgroundService
    {
        private readonly IntensisRepository _intensis;
        private readonly KatzeRepository _katze;
        private readonly ILogger<SyncRemoteWorkService> _logger;
        private readonly IConfiguration _configuration;
          private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SyncRemoteWorkService(IntensisRepository intensis, KatzeRepository katze, ILogger<SyncRemoteWorkService> logger, IConfiguration Configuration)
        {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _intensis = intensis;
            _katze = katze;
            _configuration = Configuration;


            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                       typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            int interval = Convert.ToInt32(_configuration["RemoteWorkSyncSrvInterval"]);
           

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug(" background task is doing background work.");
                try
                {
                    SyncKatze(); 
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

                await Task.Delay(TimeSpan.FromMinutes(interval), stoppingToken);

            }
        }

        private void InsertIntoKatzeForEmployee(RemoteWork intensISRemoteWork)
        {
            try
            {
                int cnt;
                var email = _intensis.GetEmployeeEmail(intensISRemoteWork.employeeId);
                var memoryID = _katze.GetMemoryIdByEmail(email);

                if (!_katze.IsEntryInKatze(intensISRemoteWork.CreatedAt))
                {
                    cnt = _katze.InsertRemoteWork(intensISRemoteWork.minutes, memoryID, "KatzeIntensisService", intensISRemoteWork.CreatedAt);
                    log.Info($"Remote work is synced for {email}");
                }
                else
                    log.Info($"Entry already existed for {email}");
            }
            catch (Exception ex)
            {

                log.Error(ex.Message);
            }
          
        }


        private void SyncKatze()
        {

            List<RemoteWork> rwl = _intensis.GetRemoteWork();
      
            foreach (var rwork in rwl.Where(a => a.CreatedAt.Value.Date >= DateTime.Now.Date.AddDays(-2)))
            {
                InsertIntoKatzeForEmployee(rwork);        
            }
      
        }

    }
}

