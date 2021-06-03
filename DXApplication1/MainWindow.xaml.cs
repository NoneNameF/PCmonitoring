using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO.Ports;
using System.Threading;

namespace DXApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ThemedWindow
    {
        DATA MyData = new DATA();
        SerialPort SerialPort = new SerialPort();
        public MainWindow()
        {
            InitializeComponent();
            SerialPortLink();
            DispatcherTimer timer = new DispatcherTimer();
            TimeSpan t1 = new TimeSpan(0, 0, 1);
            timer.Interval = t1;
            timer.Tick += TimeEvent;
            timer.Start();
        }
        private void SerialPortLink()
        {
            SerialPort.BaudRate = 115200;
            SerialPort.StopBits = StopBits.One;
            SerialPort.DataBits = 8;
            string[] str1 = SerialPort.GetPortNames();
            string str2;
            foreach (string s in str1)
            {
                SerialPort.PortName = s;
                try
                {
                    SerialPort.Open();
                    SerialPort.Write("Linked");
                    Thread.Sleep(500);
                    str2 = SerialPort.ReadExisting();
                    if (str2.Contains("LinkOk")) break;
                    else SerialPort.Close();
                }
                catch
                {
                    continue;
                }
            }
            if (SerialPort.IsOpen == false) { MessageBox.Show("连接主机失败 软件即将关闭","警告！！！"); Environment.Exit(-1); }
            else { MessageBox.Show("连接成功"); SerialPort.DataReceived += SerialPortData_DealWith; }
        }
        private void SerialPortData_DealWith(object sender, EventArgs e)
        {
            string str = SerialPort.ReadExisting();
            str = str.Substring(str.LastIndexOf("S"), str.LastIndexOf("ed"));
            MyData.TEMP = Convert.ToSingle(str.Substring(1, 5));
            MyData.PH= Convert.ToSingle(str.Substring(6, 4));
            MyData.NTU=Convert.ToInt32(str.Substring(10, 4));
            MyData.Conductivity = Convert.ToInt32(str.Substring(14, 4));
            if (Convert.ToInt32(str.Substring(18, 4)) < 0) MyData.oxidability = "氧化性";
            else MyData.oxidability = "还原性";
        }
        private void TimeEvent(object sender, EventArgs e)
        {
            Time.Text = DateTime.Now.ToString();
            this.Dispatcher.Invoke(new Action(() =>
            {
                PTEMP_Value.Text = MyData.TEMP.ToString() + "℃";
                PH_Value.Text = MyData.PH.ToString();
                NTU_Value.Text = MyData.NTU.ToString()+"NTU";
                Conductivity_Value.Text = MyData.Conductivity.ToString()+"S/M";
                oxidability.Text = MyData.oxidability;
                if (RadioButton21.IsChecked == true) series1.Points.Add(new SeriesPoint(DateTime.Now.ToString("hh:mm:ss"), MyData.TEMP));        
                if (RadioButton31.IsChecked == true) series2.Points.Add(new SeriesPoint(DateTime.Now.ToString("hh:mm:ss"), MyData.PH));          
                if (RadioButton41.IsChecked == true) series3.Points.Add(new SeriesPoint(DateTime.Now.ToString("hh:mm:ss"), MyData.NTU));         
                if (RadioButton51.IsChecked == true) series4.Points.Add(new SeriesPoint(DateTime.Now.ToString("hh:mm:ss"), MyData.Conductivity));
            }));
        }

        private void RadioButton1_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            switch (radioButton.Uid)
            {
                case "01"://实时监控页面
                    RadioButton01.Background = Brushes.YellowGreen;
                    RadioButton02.Background = Brushes.Transparent;
                    RadioButton03.Background = Brushes.Transparent;
                    BOX1.Visibility = Visibility.Visible;
                    BOX2.Visibility = Visibility.Collapsed;
                    BOX3.Visibility = Visibility.Collapsed;
                    break;
                case "02"://历史数据页面
                    RadioButton01.Background = Brushes.Transparent;
                    RadioButton02.Background = Brushes.YellowGreen;
                    RadioButton03.Background = Brushes.Transparent;
                    BOX1.Visibility = Visibility.Collapsed;
                    BOX2.Visibility = Visibility.Visible;
                    BOX3.Visibility = Visibility.Collapsed;
                    break;
                case "03"://设备控制页面
                    RadioButton01.Background = Brushes.Transparent;
                    RadioButton02.Background = Brushes.Transparent;
                    RadioButton03.Background = Brushes.YellowGreen;
                    BOX1.Visibility = Visibility.Collapsed;
                    BOX2.Visibility = Visibility.Collapsed;
                    BOX3.Visibility = Visibility.Visible;
                    break;
                case "11"://历史数据下的温度值
                    RadioButton11.Background = Brushes.YellowGreen;
                    RadioButton12.Background = Brushes.Transparent;
                    RadioButton13.Background = Brushes.Transparent;
                    RadioButton14.Background = Brushes.Transparent;
                    Chart1.Visibility = Visibility.Visible;
                    Chart2.Visibility = Visibility.Collapsed;
                    Chart3.Visibility = Visibility.Collapsed;
                    Chart4.Visibility = Visibility.Collapsed;
                    break;
                case "12"://历史数据下的PH值
                    RadioButton11.Background = Brushes.Transparent;
                    RadioButton12.Background = Brushes.YellowGreen;
                    RadioButton13.Background = Brushes.Transparent;
                    RadioButton14.Background = Brushes.Transparent;
                    Chart1.Visibility = Visibility.Collapsed;
                    Chart2.Visibility = Visibility.Visible;
                    Chart3.Visibility = Visibility.Collapsed;
                    Chart4.Visibility = Visibility.Collapsed;
                    break;
                case "13"://历史数据下的浑浊度
                    RadioButton11.Background = Brushes.Transparent;
                    RadioButton12.Background = Brushes.Transparent;
                    RadioButton13.Background = Brushes.YellowGreen;
                    RadioButton14.Background = Brushes.Transparent;
                    Chart1.Visibility = Visibility.Collapsed;
                    Chart2.Visibility = Visibility.Collapsed;
                    Chart3.Visibility = Visibility.Visible;
                    Chart4.Visibility = Visibility.Collapsed;
                    break;
                case "14"://历史数据下的电导率
                    RadioButton11.Background = Brushes.Transparent;
                    RadioButton12.Background = Brushes.Transparent;
                    RadioButton13.Background = Brushes.Transparent;
                    RadioButton14.Background = Brushes.YellowGreen;
                    Chart1.Visibility = Visibility.Collapsed;
                    Chart2.Visibility = Visibility.Collapsed;
                    Chart3.Visibility = Visibility.Collapsed;
                    Chart4.Visibility = Visibility.Visible;
                    break;
                case "21"://温度值监测
                    try
                    {
                        SerialPort.Write("WNed");
                        RadioButton21.Background = Brushes.YellowGreen;
                        RadioButton22.Background = Brushes.Transparent;
                    }
                    catch { MessageBox.Show("指令发送失败", "警告"); }
                    break;
                case "22":
                    try
                    {
                        SerialPort.Write("WFed");
                        RadioButton21.Background = Brushes.Transparent;
                        RadioButton22.Background = Brushes.YellowGreen;
                        PTEMP_Value.Text = "已关闭";
                    }
                    catch { MessageBox.Show("指令发送失败", "警告"); }
                    break;
                case "31"://PH值监测
                    try
                    {
                        SerialPort.Write("PNed");
                        RadioButton31.Background = Brushes.YellowGreen;
                        RadioButton32.Background = Brushes.Transparent;
                    }
                    catch { MessageBox.Show("指令发送失败", "警告"); }
                    break;
                case "32":
                    try
                    {
                        SerialPort.Write("PFed");
                        RadioButton32.Background = Brushes.YellowGreen;
                        RadioButton31.Background = Brushes.Transparent;
                        PH_Value.Text = "已关闭";
                    }
                    catch { MessageBox.Show("指令发送失败", "警告"); }
                    break;
                case "41"://浑浊度监测
                    try
                    {
                        SerialPort.Write("DFed");
                        RadioButton41.Background = Brushes.YellowGreen;
                        RadioButton42.Background = Brushes.Transparent;
                    }
                    catch { MessageBox.Show("指令发送失败", "警告"); }
                    break;
                case "42":
                    try
                    {
                        SerialPort.Write("DFed");
                        RadioButton42.Background = Brushes.YellowGreen;
                        RadioButton41.Background = Brushes.Transparent;
                        NTU_Value.Text = "已关闭";
                    }
                    catch { MessageBox.Show("指令发送失败", "警告"); }
                    break;
                case "51"://电导率监测
                    try
                    {
                        SerialPort.Write("TFed");
                        RadioButton51.Background = Brushes.YellowGreen;
                        RadioButton52.Background = Brushes.Transparent;
                    }
                    catch { MessageBox.Show("指令发送失败", "警告"); }
                    break;
                case "52":
                    try
                    {
                        SerialPort.Write("TFed");
                        RadioButton52.Background = Brushes.YellowGreen;
                        RadioButton51.Background = Brushes.Transparent;
                        Conductivity_Value.Text = "已关闭";
                    }
                    catch { MessageBox.Show("指令发送失败", "警告"); }
                    break;
                case "61"://含氧量监测
                    try
                    {
                        SerialPort.Write("YFed");
                        RadioButton51.Background = Brushes.YellowGreen;
                        RadioButton52.Background = Brushes.Transparent;
                    }
                    catch { MessageBox.Show("指令发送失败", "警告"); }
                    break;
                case "62":
                    try
                    {
                        SerialPort.Write("YFed");
                        RadioButton52.Background = Brushes.YellowGreen;
                        RadioButton51.Background = Brushes.Transparent;
                        oxidability.Text = "已关闭";
                    }
                    catch { MessageBox.Show("指令发送失败","警告"); }
                    break;

            }
        }
    }

    public class DATA
    {
        public float PH { get; set; }                     //酸碱度
        public float TEMP { get; set; }                   //温度值
        public int NTU { get; set; }                    //浑浊度
        public int Conductivity { get; set; }           //电导率
        public string oxidability { get; set; }          //氧化性
        public DATA()
        {
            PH = 0; NTU = 0; TEMP = 0; Conductivity = 0; 
        }
    }
}