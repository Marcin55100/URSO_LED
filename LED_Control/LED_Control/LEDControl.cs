using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using NativeWifi;
using System.Timers;

namespace LED_Control
{
    public class LEDControl
    {
        SelectionWindow Window;
        ControlWindow controlWindow;
        TcpClient Client;
        //static Timer _timer;
        public LEDControl(TcpClient Client)
        {
            Window = new SelectionWindow(Client);
            Window.Visibility = System.Windows.Visibility.Visible;
            this.Client = Client;
        }
        public LEDControl(TcpClient Client,int i)
        {
            controlWindow = new ControlWindow(Client, i);
            controlWindow.Visibility = System.Windows.Visibility.Visible;
            this.Client = Client;
        }
        //private void Start()
        //{
        //    _timer = new Timer(500);
        //    _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
        //    _timer.Enabled = true;
        //}
        //private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    if(Window.endLabel.Content.ToString().Equals("koniec"))
        //    {
        //        Window.Close();
        //        controlWindow = new ControlWindow(Client);
        //    }
        //}



        }
}
