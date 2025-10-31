using System.Net.Sockets;
using System.Text;

namespace aflevering7777
{
    /// <summary>Minimal TCP-sender til UR (Dashboard 29999 + URScript 30002).</summary>
    public class Robot
    {
        public const int UrscriptPort = 30002;
        public const int DashboardPort = 29999;

        /// <summary>Sæt til "localhost" for URSim, ellers robot/VM IP.</summary>
        public string IpAddress = "localhost";

        public void SendString(int port, string message)
        {
            using var client = new TcpClient(IpAddress, port);
            using var stream = client.GetStream();
            var bytes = Encoding.ASCII.GetBytes(message);
            stream.Write(bytes, 0, bytes.Length);
        }

        public void SendUrscript(string urscript)
        {
            // Frigiv bremsen i URSim og send programmet
            SendString(DashboardPort, "brake release\n");
            SendString(UrscriptPort, urscript);
        }
    }
}