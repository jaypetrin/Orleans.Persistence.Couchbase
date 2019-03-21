using GrainInterfaces;
using Orleans;
using Orleans.Configuration;
using System;
using System.Threading.Tasks;

namespace SampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = nameof(SampleClient);


            var client = new ClientBuilder()
               .UseLocalhostClustering()
               .Configure<ClusterOptions>(options=>
               {
                   options.ClusterId = "dev";
                   options.ServiceId = "SampleService";
               })
               //.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IVolunteerListGrain).Assembly).WithReferences())
               .Build();
            var retries = 100;

            client.Connect(async error =>
            {
                if (--retries < 0)
                {
                        //failed too many times
                        Console.WriteLine($"Could not connect to cluster: {error.Message}");
                    return false;
                }
                else
                {
                        //try again
                        Console.WriteLine($"Error Connecting {error.Message}. Retries Remaining {retries}");
                }
                await Task.Delay(1000);
                return true;
            }).Wait();

            Console.WriteLine("Connected.");

            new Shell(client)
                .RunAsync(client)
                .Wait();
        }


    }
}
