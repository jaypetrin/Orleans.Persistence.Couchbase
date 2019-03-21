using GrainInterfaces;
using Orleans;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SampleClient
{
    public class Shell
    {
        private Guid grainId = Guid.Parse("288FF26C-299D-4CC7-9265-C7D1F5550B91");

        public Shell(IClusterClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        private readonly IClusterClient client;
        private IVolunteerListGrain volunteerListGrain;

        public async Task RunAsync(IClusterClient client)
        {
            this.ShowHelp(true);

            while (true)
            {
                var command = Console.ReadLine();
                if (command == "/help")
                {
                    this.ShowHelp();
                }
                else if (command == "/quit")
                {
                    return;
                }
                else if (command.StartsWith("/add "))
                {
                    var match = Regex.Match(command, @"/add (?<name>\w{1,100})");
                    if (match.Success)
                    {
                        var name = match.Groups["name"].Value;
                        await client.GetGrain<IVolunteerListGrain>(grainId).AddNameAsync(name);

                        Console.WriteLine($"The name: [{name}] has been added to Volunteer List");
                    }
                    else
                    {
                        Console.WriteLine("Invalid command. Try again or type /help for a list of commands.");
                    }
                }
                else if (command.StartsWith("/list"))
                {
                    var names = await client.GetGrain<IVolunteerListGrain>(grainId).GetNames();
                    foreach(var name in names)
                    { 
                        Console.WriteLine(name);
                    }
                }
                else
                {
                    Console.WriteLine("Unknown command. Type /help for list of commands.");
                }
            }
        }

        private void ShowHelp(bool title = false)
        {
            if (title)
            {
                Console.WriteLine();
                Console.WriteLine("Welcome to the Couchbase State Sample!");
                Console.WriteLine("These are the available commands:");
            }

            Console.WriteLine("/help: Shows this list.");
            Console.WriteLine("/add <name>: adds name to state");
            Console.WriteLine("/list: Shows all names in state");
            Console.WriteLine("/quit: Closes this client.");
            Console.WriteLine();
        }
    }
}

