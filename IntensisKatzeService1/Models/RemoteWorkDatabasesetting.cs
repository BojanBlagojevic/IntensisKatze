using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntensisKatzeService1.Models
{
    public class RemoteWorkDatabasesetting
    { 
        public class RemoteWorkDatabasesettings : IRemoteWorkDatabasesettings
        {
            public string RemoteWorkCollectionName { get; set; }
            public string ConnectionString { get; set; }
            public string DatabaseName { get; set; }
        }

        public interface IRemoteWorkDatabasesettings
        {
            string RemoteWorkCollectionName { get; set; }
            string ConnectionString { get; set; }
            string DatabaseName { get; set; }
        }
    }
}
