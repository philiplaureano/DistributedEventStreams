using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Akka.Configuration;
using Fclp;

namespace SampleClientNode2
{
    class Program
    {
        static void Main(string[] args)
        {
            var systemName = "MyActorSystem";

            var defaultPort = 8080;
            int? port = defaultPort;
            var parser = new FluentCommandLineParser();

            // Parse the Port number
            parser.Setup<int>('p', "port")
                .Callback(value => port = value)
                .WithDescription("Set the listening port for the actor system");

            // Parse the long list of actor addresses
            var otherActors = new List<string>();
            parser.Setup<List<string>>('o', "otheractors")
                .Callback(items => otherActors = items);


            // This is a list of ports to use as a shortcut so that I don't have to type every address
            // by hand
            var portList = new List<int>();
            parser.Setup<List<int>>('l', "portlist")
                .Callback(items => portList = items);

            var actorPath = string.Empty;
            parser.Setup<string>('a', "actorpath")
                .Callback(value => actorPath = value)
                .SetDefault("/user/local-publisher");

            parser.Parse(args);

            // Expand the port list into a list of actors
            if (portList.Any())
            {
                var currentIpAddress = GetLocalIPAddress();
                otherActors.Clear();
                otherActors =
                    portList.Select(number => $"akka.tcp://{systemName}@{currentIpAddress}:{number}{actorPath}")
                        .ToList();
            }

            Console.WriteLine("Commandline Args:");

            var selectedPort = port ?? defaultPort;

            Console.WriteLine($"Port: {port}");
            Console.WriteLine($"Other Actors: {string.Join(", ", otherActors)}");

            IActorSystemHost host = new OtherHost(selectedPort, otherActors);

            host.Run(systemName);
        }
        public static string GetLocalIPAddress()
        {
            return Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .FirstOrDefault(o => o.AddressFamily == AddressFamily.InterNetwork)?
                .ToString();
        }
    }
}
