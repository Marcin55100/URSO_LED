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
        public ListBoxItem lastNetwork;
        Wifi wifi;
        public ConfigWindow(TcpClient Client)
        {
            Show();
            InitializeComponent();
            passwordBox.Visibility = Visibility.Hidden;
            ConnectButton.Visibility = Visibility.Hidden;
            this.Client = Client;

            wifi = new Wifi();
            WifiSearch(wifi);
            if (Client.Connected) Infolabel.Content = "Połączono";
        }

        private void WifiSearch(Wifi wifi)
        {
            listBox.Items.Clear();
            if (!wifi.NoWifiAvailable)
            {
                foreach (var accessPoint in wifi.GetAccessPoints())
                {
                    ListBoxItem network = new ListBoxItem();
                    network.Content = accessPoint.Name;
                    if (accessPoint.IsConnected)
                    {
                        network.FontWeight = FontWeights.Bold;
                        lastNetwork = network;
                    }

                    listBox.Items.Add(network);
                }
            }
        }
        
        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            LEDControl LED = new LEDControl(Client);
            this.Close();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            //int index=listBox.Items.IndexOf(lastNetwork);
            lastNetwork.FontWeight = FontWeights.Regular;
            // listBox.Items[index]
            if (listBox.SelectedItem != null)
            {
                ListBoxItem network = listBox.SelectedItem as ListBoxItem;
                string password = "";
                if (!wifi.GetAccessPoints().Find(item => item.Name == network.Content.ToString()).HasProfile)
                {
                    password = passwordBox.Text;
                }
                ConnectionControl.ConnectNetwork(wifi, network.Content.ToString(), password);
                if (ConnectionControl.ConnectBluegiga(Client) == true)
                {
                    MessageBoxResult result = MessageBox.Show("Połączono. Czy chcesz skonfigurować porty?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if(result== MessageBoxResult.Yes)
                    {
                        LEDControl LED = new LEDControl(Client);
                        this.Close();
                    }
                    Infolabel.Content = "Połączono";
                }
                WifiSearch(wifi);
            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem selectedNetwork = new ListBoxItem();
            try {
                selectedNetwork = (ListBoxItem)listBox.SelectedItem;
                if (selectedNetwork.FontWeight == FontWeights.Bold)
                {
                    passwordBox.Visibility = System.Windows.Visibility.Hidden;
                    ConnectButton.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    passwordBox.Visibility = System.Windows.Visibility.Visible;
                    ConnectButton.Visibility = System.Windows.Visibility.Visible;
                }
            }
            catch(NullReferenceException)
            { }
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            WifiSearch(wifi);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
