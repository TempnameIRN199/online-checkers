using System;
using System.Net;
using System.Net.Sockets;
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

namespace Checkers.Controls
{
    /// <summary>
    /// Логика взаимодействия для IPWindow.xaml
    /// </summary>
    public partial class IPWindow : Window
    {
        public IPWindow()
        {
            InitializeComponent();
            ConnectButton.Click += ConnectButton_Click;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (IPBox.Text == null || PortBox.Text == null)
            {
                MessageBox.Show("Введите IP");
            }
            else
            {
                var ip = IPBox.Text;
                int port = int.Parse(PortBox.Text);

                var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port); // получаем адреса для запуска сокета
                var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // создаем сокет

                tcpSocket.Connect(tcpEndPoint); // подключаемся к удаленному хосту

                //var buffer = new byte[256];
                //var size = 0;
                //var answer = new StringBuilder();
                //
                //do
                //{
                //    size = tcpSocket.Receive(buffer); // получаем количество реально принятых байт
                //    answer.Append(Encoding.UTF8.GetString(buffer, 0, size)); // добавляем принятую строку из буфера в StringBuilder
                //}
                //while (tcpSocket.Available > 0);

                //MessageBox.Show(answer.ToString());

                MessageBox.Show($"Connect with {tcpEndPoint}");

                //tcpSocket.Shutdown(SocketShutdown.Both); // отключаемся
                //tcpSocket.Close(); // закрываем сокет

                this.Close();
            }
        }
    }
}
