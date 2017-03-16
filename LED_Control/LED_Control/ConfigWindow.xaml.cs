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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using SimpleWifi;
using NativeWifi;
using System.IO;
using System.Threading;

namespace LED_Control
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public TcpClient Client;
        public ListBoxItem lastNetwork;
        Wifi wifi;

        public ConfigWindow(TcpClient Client)
        {
            Show();
            InitializeComponent();
            this.Client = Client;

            wifi = new Wifi();
            wifi.ConnectionStatusChanged += wifi_ConnectionStatusChanged;
            WifiSearch(wifi);
            if (Client.Connected) infoLabel.Content = "Połączono ze sterownikiem";
            else infoLabel.Content = "Sterownik niedostępny";
        }

        private void wifi_ConnectionStatusChanged(object sender, WifiStatusEventArgs e)
        {
            //WifiSearch(wifi);
            //automatyczne odświeżanie listy sieci
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
                    var test = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault();
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
            ConnectListNetwork();
        }

        private void ConnectListNetwork()
        {
            lastNetwork.FontWeight = FontWeights.Regular;
            if (listBox.SelectedItem != null)
            {
                ListBoxItem network = listBox.SelectedItem as ListBoxItem;
                string password = "";
                if (wifi.GetAccessPoints().Find(item => item.Name == network.Content.ToString()).IsSecure)
                {
                    password = passwordBox.Password;
                }
                ConnectionControl.ConnectNetwork(wifi, network.Content.ToString(), password);
                if (wifi.ConnectionStatus == WifiStatus.Connected)
                {
                    if (wifi.GetAccessPoints().Find(item => item.IsConnected).Name == network.Content.ToString())
                    {
                        ConnectionControl.DeleteMemory();
                        if (ConnectionControl.ConnectBluegiga(Client))
                        {
                            MessageBoxResult result = MessageBox.Show("Połączono. Czy chcesz skonfigurować porty?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                LEDControl LED = new LEDControl(Client);
                                this.Close();
                            }
                            infoLabel.Content = "Połączono";
                        }
                        else infoLabel.Content = "Brak połączenia";
                    }
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
                    networkPanel.Visibility = Visibility.Hidden;
                }
                else
                {
                    networkPanel.Visibility = Visibility.Visible;
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

        private void changeButton_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                ListBoxItem network = listBox.SelectedItem as ListBoxItem;
                NetworkStream stream = Client.GetStream();
                if (stream.CanWrite)
                {
                    //stream.WriteTimeout = 1000;
                    byte[] message = Encoding.ASCII.GetBytes("BSSID" + network.Content.ToString());
                    stream.Write(message, 0, message.Length);
                    message = Encoding.ASCII.GetBytes("NETPW" + passwordBox.Password);
                    stream.Write(message, 0, message.Length);
                    //Thread.Sleep(5000);
                    var delay = Task.Run(async delegate
                    {
                        await Task.Delay(3000);
                    });
                    delay.Wait();
                    //stream.Close();
                }
                ConnectListNetwork();
            }
        }
    }
}
