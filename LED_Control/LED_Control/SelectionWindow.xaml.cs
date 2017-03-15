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
using System.Collections.ObjectModel;
using System.Timers;
using System.IO;
using System.Xml.Serialization;
using System.Collections;

namespace LED_Control
{
    /// <summary>
    /// Interaction logic for SelectionWindow.xaml
    /// </summary>
    public partial class SelectionWindow : Window
    {
        private ObservableCollection<LEDSegment> list;
        private List<string> typeList;
        int id = 0;
        string message = "";
        LEDSegment Segment;
        static Timer _timer;
        TcpClient Client;
        ControlWindow Window;
        LEDControl ledControl;
        public SelectionWindow(TcpClient Client)
        {
            InitializeComponent();
            Start();
            this.Client = Client;
            // this.listView.DataContext=
            list = new ObservableCollection<LEDSegment>();
            typeList = new List<string>();
            typeList.Add("ON/OFF");
            typeList.Add("PWM");
            this.listView.ItemsSource = list;
            this.addStack.DataContext = this;
            this.typeBox.ItemsSource = typeList;
            this.typeBox.SelectedIndex = 0;
            hideElements();
            Load();
        }
        private void hideElements()
        {
            this.mainStack.Visibility = System.Windows.Visibility.Hidden;
            this.NameLabel.Visibility = System.Windows.Visibility.Hidden;
            this.NameBox.Visibility = System.Windows.Visibility.Hidden;    
        }
        private void Start() // Licznik jest po to, aby można było wstawić migającą diodkę do widoku. Jeszcze nie rozkminione.
        {
            _timer = new Timer(1000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
             // Enable it    
        }
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Segment.Blink == "White")
                Segment.Blink = "Black";
            else
                Segment.Blink = "White";
        }
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Enabled = false;
            Segment.Name = NameBox.Text;
            //Segment.Type = ((ListBoxItem)this.typeBox.SelectedValue).Content.ToString();
            // int index = this.typeBox.SelectedIndex;
            // Segment.Type = typeList.ElementAt();
            //var current = this.typeBox.SelectedItem;
            // ListBoxItem lbi = this.typeBox.ItemContainerGenerator.ContainerFromItem(current) as ListBoxItem;
            Segment.Type = (typeBox.SelectedItem as String);
            Segment.Id = id;
            id++;
            list.Add(Segment);
            hideElements();
        }


        private void ListToXmlFile(string filePath)
        {
            using (var sw = new StreamWriter(filePath))
            {
                var serializer = new XmlSerializer(typeof(ObservableCollection<LEDSegment>));
                serializer.Serialize(sw, list);
            }
        }

        private void XmlFileToList(string filepath)
        {
            list.Clear();
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
            var systemPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            // string[] filePaths = Directory.GetFiles(systemPath);
            fileBox.Items.Clear();
            foreach (var file in Directory.EnumerateFiles(systemPath, "*.xml"))
            {
                string fileName_ = file.ToString().Replace(systemPath.ToString()+"\\","");
                fileName_=fileName_.Replace(".xml", "");
                ListBoxItem fileName = new ListBoxItem();
                fileName.Content = fileName_;
                fileBox.Items.Add(fileName);
            }
           // filePaths =Array.FindAll(filePaths,s =>s.Contains("xml"));

            try
            {
                XmlFileToList(systemPath + @"\Segments.xml");
            }
            catch(FileNotFoundException)
            {

            }

        }


        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            var systemPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);     
            ListToXmlFile(systemPath + @"\Segments.xml");
            _timer.Enabled = false;
            ledControl = new LEDControl(Client, list.Count);
            this.Close();
         
        }

        private void deleteButton(object sender, RoutedEventArgs e)
        {
            if (this.listView.SelectedIndex > -1)
                list.RemoveAt(listView.SelectedIndex);
            else
            {
                list.Remove(list.Last());
                id--;
            }
        }

        private void NewSegment_Click(object sender, RoutedEventArgs e)
        {
                    NetworkStream Stream = Client.GetStream();
                    if (Stream.CanWrite)
                    {
                StringBuilder builder = new StringBuilder();
                builder.Append("BLK0").Append(id);
                message = builder.ToString();
                byte[] Buffer = Encoding.ASCII.GetBytes(message);
                        Stream.Write(Buffer, 0, Buffer.Length);
                    }
            Segment = new LEDSegment();
            this.mainStack.Visibility = System.Windows.Visibility.Visible;
            this.NameLabel.Visibility = System.Windows.Visibility.Visible;
            this.NameBox.Visibility = System.Windows.Visibility.Visible;
            _timer.Enabled = true;
        }

        private void fileBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                var systemPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                ListBoxItem selectedNetwork = new ListBoxItem();
                selectedNetwork = (ListBoxItem)fileBox.SelectedItem;
            try
            {
                XmlFileToList(systemPath + "\\" + selectedNetwork.Content + ".xml");
            }
            catch(NullReferenceException)
                {


                }
        }

        private void saveConfigButton_Click(object sender, RoutedEventArgs e)
        {
             var systemPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);     
             ListToXmlFile(systemPath +"\\"+ConfigNameBox.Text+".xml");
             Load();
        }

        private void deleteConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var systemPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            ListBoxItem selectedNetwork = new ListBoxItem();
            selectedNetwork = (ListBoxItem)fileBox.SelectedItem;

            System.IO.DirectoryInfo di = new DirectoryInfo(systemPath);
            foreach (FileInfo file in di.GetFiles())
            {
                if(file.Name == (selectedNetwork.Content.ToString()+".xml"))
                {
                    file.Delete();
                    fileBox.SelectedIndex = 0;
                    fileBox.Items.Remove(selectedNetwork);
                    
                }
            }



        }
    }
}
