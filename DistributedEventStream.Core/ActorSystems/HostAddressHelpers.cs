using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace DistributedEventStream.Core.ActorSystems
{
    internal static class HostAddressHelpers
    {
        public static int GetNextFreeTcpPort()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();

            var ipEndPoint = (IPEndPoint)tcpListener.LocalEndpoint;
            var port = ipEndPoint.Port;

            tcpListener.Stop();

            return port;
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