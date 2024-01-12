using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows.Controls;
using Checkers.Game;
using System.Windows;

namespace Checkers.Controls;
public partial class MainMenu : UserControl
{
    public MainMenu()
    {
        InitializeComponent();
        this.DataContext = this; 
        startButton.Click += StartGame;
        ConnectButton.Click += ConnectFriend;
        //InviteServer();
    }
    private void StartGame(object o, EventArgs e)
    {
        (this.Parent as MainWindow)!.Content = new Board((this.Parent as MainWindow), new RussianCheckers(), computer.IsChecked is true, white.IsChecked is true);
    }

    private void ConnectFriend(object o, EventArgs e)
    {
        IPWindow ipWindow = new IPWindow();
        ipWindow.Show();
    }


    public void InviteServer()
    {
        const string ip = "127.0.0.1";
        const int port = 12345;

        var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port); // получаем адреса для запуска сокета

        var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // создаем сокет

        try
        {
            tcpSocket.Connect(tcpEndPoint); // подключаемся к удаленному хосту
        }
        catch (SocketException ex)
        {
            MessageBox.Show(ex.Message);
            throw;
        }

        MessageBox.Show("Connected");
    }
}
