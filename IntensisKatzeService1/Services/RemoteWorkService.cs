using IntensisKatzeService1.Models;
using IntensisKatzeService1.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace IntensisKatzeService1.Services
{
    public class RemoteWorkService
    {
        private readonly IMongoCollection<RemoteWork> _remoteWork;
        private readonly IMongoCollection<User> _user;
        private readonly KatzeRepository _katze;


        public RemoteWorkService(IIntenseISDatabaseSettings settings, KatzeRepository katze)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _remoteWork = database.GetCollection<RemoteWork>(settings.CollectionName);
            _user = database.GetCollection<User>("user");
            _katze = katze;

        }

        public List<RemoteWork> Get()
        {

            List<RemoteWork> remoteWork;
            remoteWork = _remoteWork.Find(emp => true).ToList();
            return remoteWork;
        }

    }
}
