using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;

namespace ServerCheckers
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Thread serverThread = new Thread(new ThreadStart(StartServer));
            serverThread.Start();
            Console.WriteLine("Server started");
            Console.ReadLine();
            serverThread.Abort();

        }
        //comment
        static void StartServer()
        {
            const string ip = "127.0.0.1";
            const int port = 12345;

            var tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                tcpSocket.Bind(tcpEndPoint);
                tcpSocket.Listen(5);

                Console.WriteLine($"Server started on {ip}:{port}");

                while (true)
                {
                    var listener = tcpSocket.Accept();
                    Console.WriteLine($"Connected with {listener.RemoteEndPoint}");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Server error: " + ex.Message);
            }
            finally
            {
                tcpSocket.Close();
            }
        }
    }
}
