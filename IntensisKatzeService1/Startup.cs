using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntensisKatzeService1.Models;
using IntensisKatzeService1.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using static IntensisKatzeService1.Models.EmployeeDatabasesetting;
using static IntensisKatzeService1.Models.RemoteWorkDatabasesetting;
using IntensisKatzeService1.Repository;

namespace IntensisKatzeService1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<EmployeeDatabaseSettings>(
               Configuration.GetSection(nameof(EmployeeDatabaseSettings)));

            services.AddSingleton<IEmployeeDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<EmployeeDatabaseSettings>>().Value);

            services.AddSingleton<EmployeeService>();

            services.AddSingleton<IntensisRepository>();

            services.Configure<IntensisRepository>(
              Configuration.GetSection(nameof(IntensisRepository)));


            services.AddSingleton<KatzeRepository>();


            services.Configure<RemoteWorkDatabasesettings>(
               Configuration.GetSection(nameof(RemoteWorkDatabasesettings)));

            services.AddSingleton<IRemoteWorkDatabasesettings>(sp =>
               sp.GetRequiredService<IOptions<RemoteWorkDatabasesettings>>().Value);

            services.AddSingleton<RemoteWorkService>();

            services.AddHostedService<SyncRemoteWorkService>();

            var ConnectionString = Configuration.GetConnectionString("MbkDbConstr");

            //Entity Framework  
            //services.AddDbContext<KatzeContext>(options => options.UseSqlServer(ConnectionString));

            services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddLog4Net();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}
