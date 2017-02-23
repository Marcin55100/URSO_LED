using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using SimpleWifi;
using System.IO;

namespace LED_Control
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        private const int listenPort = 11000;
        public TcpClient Client;
        public static String ServerIP;
        public static String Message;
        public String [] IPInfo;
        public ConfigWindow()
        {
            this.Show();
            InitializeComponent();

            Wifi wifi = new Wifi();
            if (!wifi.NoWifiAvailable)
            {
                foreach (var accessPoint in wifi.GetAccessPoints())
                {
                    ListBoxItem network = new ListBoxItem();
                    network.Content = accessPoint.Name;
                    listBox.Items.Add(network);
                }
            }

            //StartListener();
            //Connect_();
        }

   

        private void StartListener()
        {
            bool done = false;

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
                    Message = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

                    if (Message=="HELLO")
                    {
                        ServerIP = groupEP.ToString();
                        IPInfo = ServerIP.Split(':');
                        WriteToFile(IPInfo);
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
        }

        private void WriteToFile(String [] IP)
        {
            string[] lines = {IP[0], IP[1]};
            
               var systemPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            using (StreamWriter outputFile = new StreamWriter(systemPath + @"\ConnectionInfo.txt"))
            {
                foreach (string line in lines)
                    outputFile.WriteLine(line);
            }
        }

        private void Connect_()
        {
            Client = new TcpClient();
            IPAddress IP;
            int port;
            if (!Client.Connected)
            {
                if (IPAddress.TryParse(IPInfo[0], out IP) && int.TryParse(IPInfo[1], out port))
                {
                    try
                    {
                        Client.Connect(IP, port);
                        Infolabel.Content = "Połączono";
                    }
                    catch (SocketException)
                    {
                        Infolabel.Content = "Serwer niedostępny";
                    }
                }
                else Infolabel.Content = "Błędny format adresu IP lub portu";
            }
            else Infolabel.Content = "Połącznie wciąż aktywne";
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            LEDControl LED = new LEDControl(Client);
            this.Close();
        }
    }
}
