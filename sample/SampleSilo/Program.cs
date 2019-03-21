using Orleans.Configuration;
using Orleans.Hosting;
using Orleans;
using System;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Grains;
using Microsoft.Extensions.Logging;
using Orleans.Persistence.Couchbase;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ProcessSilo
{
    class Program
    {
        private static ISiloHost _silo;
        private static readonly ManualResetEvent SiloStopped = new ManualResetEvent(false);
        
        static async Task Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables();
            Configuration = configBuilder.Build();
            var uris = new List<Uri>();
            Configuration.GetSection("CouchbaseUris").Bind(uris);

            _silo = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "BatchService";
                })
                .AddCouchbaseGrainStorage("default", options=>
                {
                    options.Uris = uris;
                    options.BucketName = Configuration.GetValue<string>("CouchbaseBucketName");
                    options.UserName = Configuration.GetValue<string>("CouchbaseUserName");
                    options.Password = Configuration.GetValue<string>("CouchbasePassword");
                })
                //Dependency Injection
                .ConfigureLogging(logger => logger.AddConsole())
                .ConfigureApplicationParts(parts =>
                    parts.AddApplicationPart(typeof(VolunteerListGrain).Assembly).WithReferences())
                .Build();

            await StartSilo();


            AssemblyLoadContext.Default.Unloading += async context =>
            {
                await StopSilo();
                SiloStopped.WaitOne();
            };

            SiloStopped.WaitOne();
        }

        private static async Task StartSilo()
        {
            try
            {
                await _silo.StartAsync();
                Console.WriteLine("Silo started");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public static IConfigurationRoot Configuration { get; set; }

        private static async Task StopSilo()
        {
            await _silo.StopAsync();
            Console.WriteLine("Silo stopped");
            SiloStopped.Set();
        }
    }
}
