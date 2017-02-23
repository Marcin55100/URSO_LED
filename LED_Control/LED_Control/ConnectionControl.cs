using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using SimpleWifi;
using System.IO;

namespace LED_Control
{
    class ConnectionControl
    {
        const int port = 23;

        public static bool ConnectBluegiga(TcpClient tcp)
        {
            bool connection = false;
            Wifi wifi = new Wifi();
            IPAddress IP;
            string ssid;
            string password;
            
            if (!wifi.NoWifiAvailable)
            {
                if (ReadMemory(out IP, out ssid, out password))
                {
                    if (wifi.GetAccessPoints().Find(item => item.IsConnected == true).Name == ssid)
                    {
                        tcp = CreateTCPConnection(IP, port, tcp);
                        if (tcp.Connected) connection = true;
                    }
                    else if (wifi.GetAccessPoints().Exists(item => item.Name == ssid))
                    {
                        ConnectNetwork(ssid, password, wifi);
                        if (wifi.ConnectionStatus == WifiStatus.Connected)
                        {
                            tcp = CreateTCPConnection(IP, port, tcp);
                            if (tcp.Connected) connection = true;
                        }
                    }
                }
                if (connection) SaveMemory(((IPEndPoint)tcp.Client.RemoteEndPoint).Address, ssid, password);
            }
            return connection;
        }

        private static bool ReadMemory(out IPAddress IP, out string ssid, out string password)
        {
            bool memory = true;
            var systemPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            try
            {
                StreamReader file = new StreamReader(systemPath + @"\ConnectionInfo.txt");
                memory = IPAddress.TryParse(file.ReadLine(), out IP);
                ssid = file.ReadLine();
                password = file.ReadLine();
                file.Close();
            }
            catch (Exception)
            {
                IP = IPAddress.None;
                ssid = "";
                password = "";
                memory = false;
            }
            return memory;
        }

        private static void SaveMemory(IPAddress IP, string ssid, string password)
        {
            var systemPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            using (StreamWriter outputFile = new StreamWriter(systemPath + @"\ConnectionInfo.txt"))
            {
                outputFile.WriteLine(IP.ToString());
                outputFile.WriteLine(ssid);
                outputFile.WriteLine(password);
            }
        }

        private static TcpClient CreateTCPConnection(IPAddress IP, int port, TcpClient tcp)
        {
            //TcpClient tcp = new TcpClient();
            try
            {
                tcp.Connect(IP, port);
            }
            catch (Exception)
            {
                try
                {
                    IP = UDPListener();
                    tcp.Connect(IP, port);
                }
                catch (Exception) { }
            }
            return tcp;
        }

        private static IPAddress UDPListener()
        {
            const int listenPort = 11000;
            bool done = false;
            string ServerIP = null;

            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
            IPEndPoint broadcast = new IPEndPoint(IPAddress.Broadcast, port);
            listener.Send(new byte[] { 1, 2, 3, 4, 5 }, 5, broadcast);
            try
            {
                while (!done)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);

                    Console.WriteLine("Received broadcast from {0} :\n {1}\n",
                        groupEP.ToString(),
                        Encoding.ASCII.GetString(bytes, 0, bytes.Length));
                    string response = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

                    if (response == "HELLO")
                    {
                        ServerIP = groupEP.ToString().Split(':')[0];
                        done = true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
            }
            return IPAddress.Parse(ServerIP);
        }

        private static void ConnectNetwork(string ssid, string password, Wifi wifi)
        {
            if (wifi.NoWifiAvailable) Console.WriteLine("No WiFi card was found");
            else
            {
                var accessPoint = wifi.GetAccessPoints().Find(item => item.Name == ssid);
                AuthRequest authRequest = new AuthRequest(accessPoint);
                bool overwrite = true;
                if (authRequest.IsPasswordRequired)
                {
                    if (accessPoint.HasProfile) overwrite = false;
                    else authRequest.Password = password;
                }
                accessPoint.Connect(authRequest, overwrite);
            }
        }

        public static List<string> GetWifiNetworks()
        {
            List<string> networks = new List<string>();
            Wifi wifi = new Wifi();
            if (!wifi.NoWifiAvailable)
            {
                foreach (var accessPoint in wifi.GetAccessPoints())
                {
                    networks.Add(accessPoint.Name);
                }
            }
            return networks;
        }
    }
}
