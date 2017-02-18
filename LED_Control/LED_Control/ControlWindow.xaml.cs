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
using System.Windows.Shapes;
using System.Net.Sockets;

namespace LED_Control
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlWindow : Window
    {
        TcpClient Client;
        int buttonNumber = 0;
        string message = "";
        string buttonName = "";
        int LEDnumber = 0;
        public ControlWindow(TcpClient Client, int buttonNumber)
        {
            this.buttonNumber = buttonNumber;
            InitializeComponent();
            this.Client = Client;
            createButtons();
        }

        private void createButtons()
        {
            for (int i = 0; i < buttonNumber; i++)
            {
                System.Windows.Controls.Label newLabel = new Label();
                newLabel.Content = "LED" + i.ToString();
                newLabel.Name = "Label" + i.ToString();
                mainPanel.Children.Add(newLabel);
                System.Windows.Controls.Button newONBtn = new Button();
                    newONBtn.Content = "ON";
                    newONBtn.Name = "ButtonON" + i.ToString();
                newONBtn.Click += ONbutton_Click;
                System.Windows.Controls.Button newOFFBtn = new Button();
                newOFFBtn.Content = "OFF";
                newOFFBtn.Name = "ButtonOF" + i.ToString();
                newOFFBtn.Click += OFFbutton_Click;
                mainPanel.Children.Add(newONBtn);
                mainPanel.Children.Add(newOFFBtn);

            
            }

        }

        private void NewONBtn_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void newONBtn_Click(object sender, RoutedEventArgs e)
        {

        }


        private void ONbutton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            SendPacket("LON", button.Name);
        }

        private void OFFbutton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            SendPacket("LOF", button.Name);
        }
        private void SendPacket(String Command, String Number)
        {
            if (Client.Connected)
            {
                NetworkStream Stream = Client.GetStream();
                if (Stream.CanWrite)
                {
                    try
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append(Command).Append(0).Append(Number[8]);
                        message = builder.ToString();
                        byte[] Buffer = Encoding.ASCII.GetBytes(message);
                        Stream.Write(Buffer, 0, Buffer.Length);
                    }
                    catch(SystemException)
                    {
                        MessageBox.Show("Połączenie przerwane.");
                    }
                }
            }
           



        }



    }
}
