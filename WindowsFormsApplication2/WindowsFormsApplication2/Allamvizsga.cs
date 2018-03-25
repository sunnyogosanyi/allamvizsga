using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        Int32 bytes;

        Graphics drawingArea;
        Pen blackPan = new Pen(Color.Green);
        String responseData = String.Empty;
        String server = "192.168.88.210";
        Int32 port = 5200;
        TcpClient client;
        Byte[] data;
        bool var = true;
        NetworkStream stream;
        int globalNumber = 1;
        String message;
        int x1 = 200;
        int y1 = 200;
        int x2= 10;
        int y2= 10;

        public Form1()
        {
            InitializeComponent();
            drawingArea = pictureBox1.CreateGraphics();
        }
        
        private void sendBtn_Click(object sender, EventArgs e)
        {

            String message = textBox1.Text;
           
            //message = "Csecs";
           
                // Connect(message);
                Connect(server, message);
                Console.WriteLine("Wait");
                //System.Threading.Thread.Sleep(1000);
            
        }

        void Connect(String server, String message)
        {
            try
            {
             


                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                
                TcpClient client = new TcpClient(server, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                

                // String to store the response ASCII representation.

                stream.Flush();
                stream.Close();
                client.Close();

            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

        }

        void test(string mess)
        {
            if (mess.Equals("R"))
            {
                globalNumber++;
            }
            else if (mess.Equals("L"))
            {
                globalNumber--;
            }
            if (globalNumber>4)
            {
                globalNumber = 1;
            }
             if(globalNumber < 1)
            {
                globalNumber = 4;
            }
            textBox2.Text = globalNumber.ToString();
            DrawFunction(globalNumber);
        }

        void DrawFunction(int message)
        {
                 
            try
            {
                switch (message)
                {
                    case 1:
                        y1 += 30;
                        //y2 = Int32.Parse(messCommand);
                        //x2 = 10;
                        break;
                    case 3:
                        y1 -= 30;
                        //y2 = Int32.Parse(messCommand);
                        //x2 = 10;
                        break;
                    case 2:
                        x1 -= 30;
                       // x2 = Int32.Parse(messCommand);
                       // y2 = 10;
                        break;
                    case 4:
                        x1 += 30;
                        //x2 = Int32.Parse(messCommand);
                        //y2 = 10;
                        break;
                }
                drawingArea.FillRectangle(Brushes.Black,new Rectangle( x1, y1, x2, y2));
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Error");
            }


        }


        void decode(string input, out string messType, out string messCommand)
        {
            string temp = input.Trim(new Char[] { '{', '}' });
            string[] words = temp.Split(':');
            messType = words[0];
            messCommand = words[1];
           
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            TcpListener serverSocket = new TcpListener(5100);
            int requestCount = 0;
            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine(" >> Server Started");
            clientSocket = serverSocket.AcceptTcpClient();
            Console.WriteLine(" >> Accept connection from client");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
