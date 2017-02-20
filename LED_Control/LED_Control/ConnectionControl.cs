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
        public static void ConnectBluegiga()
        {
            string APssid = "Bluegiga";
            string APpassword = "bluegiga";   //te dane mogą być zmieniane i muszą być gdzieś zapamiętane
            IPAddress IP = null;
            ReadMemoryIP(out IP);//, APssid, APpassword);
            Wifi wifi = new Wifi();
            if (wifi.NoWifiAvailable) Console.WriteLine("No WiFi card was found");
            else if (wifi.GetAccessPoints().Find(item => item.IsConnected == true).Name == APssid) CreateTCPConnection(IP, 23);
            else if (wifi.GetAccessPoints().Exists(item => item.Name == APssid))
            {
                ConnectNetwork(APssid, APpassword, wifi);
                CreateTCPConnection(IP, 23);
            }
        }

        public static bool ReadMemoryIP(out IPAddress IP)//, string APssid, string APpassword)
        {
            bool memory = true;
            var systemPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            try
            {
                StreamReader file = new StreamReader(systemPath + @"\ConnectionInfo.txt");
                memory = IPAddress.TryParse(file.ReadLine(), out IP);
                //APssid = file.ReadLine();
                //APpassword = file.ReadLine();
                file.Close();
            }
            catch (Exception)
            {
                IP = IPAddress.None;
                memory = false;
            }
            return memory;
        }

        public static TcpClient CreateTCPConnection(IPAddress IP, int port)
        {
            TcpClient tcp = new TcpClient();
            try
            {
                tcp.Connect(IP, port);
            }
            catch (Exception) { }
            return tcp;
        }

        static IPAddress UDPListener()
        {
            const int listenPort = 11000;
            bool done = false;
            string ServerIP = null;

            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
            IPEndPoint bluegiga = new IPEndPoint(IPAddress.Broadcast, 23);
            listener.Send(new byte[] { 1, 2, 3, 4, 5 }, 5, bluegiga);
            try
            {
                while (!done)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP);

                    Console.WriteLine("Received broadcast from {0} :\n {1}\n",
                        groupEP.ToString(),
                        Encoding.ASCII.GetString(bytes, 0, bytes.Length));
                    ServerIP = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

                    if (ServerIP == "HELLO")
                    {
                        ServerIP = groupEP.ToString();
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
            return IPAddress.Parse(ServerIP.Split(':')[0]);
        }

        static void ConnectNetwork(string ssid, string password, Wifi wifi)
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
    }
}
