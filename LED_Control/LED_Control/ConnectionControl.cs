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
            
            if (!wifi.NoWifiAvailable)
            {
                //próba połączenia z BG w sieci do której urządzenie jest podłączone
                if (wifi.ConnectionStatus == WifiStatus.Connected)
                {
                    tcp = CreateTCPConnection(UDPListener(), port, tcp);
                    if (tcp.Connected) connection = true;
                }
                //sprawdzenie czy aplikacja ma w pamięci parametry połączenia
                if (!connection && ReadMemory(out IP, out ssid))
                {
                    //próba połącznia z BG w sieci z pamięci aplikacji (jeśli sieć znajduje się na liście dostępnych AP)
                    if (!connection && wifi.GetAccessPoints().Exists(item => item.Name == ssid))
                    {
                        ConnectNetwork(wifi, ssid);
                        if (wifi.ConnectionStatus == WifiStatus.Connected)
                        {
                            tcp = CreateTCPConnection(IP, port, tcp);
                            if (tcp.Connected) connection = true;
                        }
                    }
                }
                if (connection) SaveMemory(((IPEndPoint)tcp.Client.RemoteEndPoint).Address, wifi.GetAccessPoints().Find(item => item.IsConnected).Name);
            }
            return connection;
        }

        private static bool ReadMemory(out IPAddress IP, out string ssid)
        {
            bool memory = true;
            var systemPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            try
            {
                StreamReader file = new StreamReader(systemPath + @"\ConnectionInfo.txt");
                memory = IPAddress.TryParse(file.ReadLine(), out IP);
                ssid = file.ReadLine();
                file.Close();
            }
            catch (Exception)
            {
                IP = IPAddress.None;
                ssid = "";
                memory = false;
            }
            return memory;
        }

        private static void SaveMemory(IPAddress IP, string ssid)
        {
            var systemPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            using (StreamWriter outputFile = new StreamWriter(systemPath + @"\ConnectionInfo.txt"))
            {
                outputFile.WriteLine(IP.ToString());
                outputFile.WriteLine(ssid);
            }
        }

        private static TcpClient CreateTCPConnection(IPAddress IP, int port, TcpClient tcp)
        {
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
            IPAddress ServerIP = IPAddress.Any;

            UdpClient udp = new UdpClient(listenPort);
            udp.Client.ReceiveTimeout = 1000;
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
            IPEndPoint broadcast = new IPEndPoint(IPAddress.Broadcast, port);
            udp.Send(new byte[] { 1, 2, 3, 4, 5 }, 5, broadcast);
            try
            {
                byte[] bytes = udp.Receive(ref groupEP);
                string response = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

                if (response == "HELLO") ServerIP = IPAddress.Parse(groupEP.ToString().Split(':')[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                udp.Close();
            }
            return ServerIP;
        }

        public static void ConnectNetwork(Wifi wifi, string ssid, string password = "")
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
