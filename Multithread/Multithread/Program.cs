using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Multithread
{
    class Program
    {
        static Thread serverThread;
        static Thread clientThread;
        static TcpListener serverSocket;
        static TcpClient clientSocket;

        static void Main(string[] args)
        {
            serverThread = new Thread(Server);
            serverThread.Start();

        

            clientThread = new Thread(Client);
            clientThread.Start();
            serverThread.Join();
            clientThread.Join();

            Console.ReadLine();
        }


        static void Server()
        {
            serverSocket = new TcpListener(IPAddress.Parse("192.168.88.14"), 5100);
            clientSocket = default(TcpClient);


            serverSocket.Start();
            Console.WriteLine(" >> Server Started");
            
            clientSocket = serverSocket.AcceptTcpClient();
            Console.WriteLine(" >> Accept connection from client");

            while ((true))
            {
                try
                {
                    NetworkStream networkStream = clientSocket.GetStream();
                    byte[] bytesFrom = new byte[105];
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Console.WriteLine(" >> Data from client - " + dataFromClient);
                    string serverResponse = "Last Message from client" + dataFromClient;
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                    Console.WriteLine(" >> " + serverResponse);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine(" >> exit");
            Console.ReadLine();

        }

        static void Client()
        {
            for (int i = 500; i < 600; i++)
            {
                Console.WriteLine(i);
                Thread.Sleep(2000);
            }
        }
    }
}
