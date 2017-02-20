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
        //string AddressIP;
        //const string Port = "23";//"48569";

        public MainWindow()
        {
            InitializeComponent();
            InitiateConnection();
            list = new ObservableCollection<LEDSegment>();
        }
        public void InitiateConnection()
        {
            IPAddress IP;
            const int port = 23;

            if (ConnectionControl.ReadMemoryIP(out IP))
            {
                Client = ConnectionControl.CreateTCPConnection(IP, port);
                if (Client.Connected) Infolabel.Content = "Połączono";
                else Infolabel.Content = "Serwer niedostępny";
            }
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

        private void GroupButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
