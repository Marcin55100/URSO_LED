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
using NativeWifi;
using System.IO;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace LED_Control
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ConfigWindow configWindow;
        LEDControl ledControl;
        TcpClient Client;
        ObservableCollection<LEDSegment> list;
         String AdrressIP = "10.2.2.236";
            String Port = "48569";
        public MainWindow()
        {
            InitializeComponent();
            //WlanClient wlan = new WlanClient();
            InitiateConnection();
            list = new ObservableCollection<LEDSegment>();
            //this.IPBox.Text = "192.168.1.5";
            //this.PortBox.Text = "48569";
            //this.LEDon_Button.Visibility = System.Windows.Visibility.Hidden;
            //this.LEDoff_Button.Visibility = System.Windows.Visibility.Hidden;
            //WlanClient wlan = new WlanClient();
            //string SSID = "";
            //foreach (var item in wlan.Interfaces)
            //{
            //    SSID += " " + item.CurrentConnection.profileName;
            //    //List<Wlan.WlanAvailableNetwork> networks = item.GetAvailableNetworkList(0).ToList();
            //    //foreach (var network in networks)
            //    //{
            //    //    Console.WriteLine("SSID {0}", Encoding.ASCII.GetString(network.dot11Ssid.SSID, 0, (int)network.dot11Ssid.SSIDLength));
            //    //}
            //}
            //this.Infolabel.Content = SSID;

        }
        private void InitiateConnection()
        {
            Client = new TcpClient();
            IPAddress IP;
            int port;
            if (!Client.Connected)
            {
                if (IPAddress.TryParse(AdrressIP, out IP) && int.TryParse(Port, out port))
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

    

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            configWindow = new ConfigWindow();
            this.Hide();
        }
        private void XmlFileToList(string filepath)
        {
            using (var sr = new StreamReader(filepath))
            {
                var deserializer = new XmlSerializer(typeof(ObservableCollection<LEDSegment>));
                ObservableCollection<LEDSegment> tmpList = (ObservableCollection<LEDSegment>)deserializer.Deserialize(sr);
                foreach (var item in tmpList)
                {
                    list.Add(item);
                }
            }
        }



        private void Load()
        {
           // Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
           // dlg.DefaultExt = ".xml";
          //  dlg.Filter = "XML documents (.xml)|*.xml";

         //   Nullable<bool> result = dlg.ShowDialog();
            string filepath = "C:\\Users\\Marcin\\Documents\\Visual Studio 2015\\Projects\\LED_Control\\LED_Control\\Save_configuration.xml";
          //  if (result == true)
           // {
            //    filepath = dlg.FileName;

            //}
            if (File.Exists(filepath))
            {
                XmlFileToList(filepath);
            }
            else
            {
                MessageBox.Show(@"chyba Ty'");
            }

        }

        private void LedButton_Click(object sender, RoutedEventArgs e)
        {
            Load();
            ledControl = new LEDControl(Client, list.Count);
            this.Close();
        }
    }
}
