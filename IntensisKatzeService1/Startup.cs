using IntensisKatzeService1.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using IntensisKatzeService1.Repository;
using IntensisKatzeService1.Models;

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

            services.AddSingleton<IntensisRepository>();

            services.Configure<IntensisRepository>(
              Configuration.GetSection(nameof(IntensisRepository)));


            services.AddSingleton<KatzeRepository>();


            services.Configure<IntensisDatabaseSettings>(
               Configuration.GetSection(nameof(IntensisDatabaseSettings)));

            services.AddSingleton<IIntenseISDatabaseSettings>(sp =>
               sp.GetRequiredService<IOptions<IntensisDatabaseSettings>>().Value);

            services.AddSingleton<RemoteWorkService>();

            services.AddHostedService<SyncRemoteWorkService>();


            services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

        }

      
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddLog4Net();


            app.UseRouting();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}
