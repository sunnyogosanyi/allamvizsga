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
        String server = "192.168.100.21";
        Int32 port = 80;
        TcpClient client;
        Byte[] data;

        NetworkStream stream;

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
            // Connect(message);
            Connect(server,message);
            
        }

        void Connect(String server, String message)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                Int32 port = 80;
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
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                
                DrawFunction(responseData);

                                textBox2.Text = responseData;
                // Close everything.
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

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }

        void DrawFunction(string message)
        {
            string messType;
            string messCommand;
            
            try
            {
                decode(message, out messType, out messCommand);
            Console.WriteLine("Received: {0} {1}", messType, messCommand);
                switch (messType)
                {
                    case "D":
                        y1 += Int32.Parse(messCommand);
                        //y2 = Int32.Parse(messCommand);
                        //x2 = 10;
                        break;
                    case "U":
                        y1 -= Int32.Parse(messCommand);
                        //y2 = Int32.Parse(messCommand);
                        //x2 = 10;
                        break;
                    case "L":
                        x1 -= Int32.Parse(messCommand);
                       // x2 = Int32.Parse(messCommand);
                       // y2 = 10;
                        break;
                    case "R":
                        x1 += Int32.Parse(messCommand);
                        //x2 = Int32.Parse(messCommand);
                        //y2 = 10;
                        break;
                }
                drawingArea.FillRectangle(Brushes.Black,new Rectangle( x1, y1, x2, y2));
                Console.WriteLine("x1 {0} y1 {1} x2 {2} y2 {3} ",x1,y1,x2,y2);
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

        void Connect(String message)
        {
            try
            {
                data = new Byte[256];
                data = System.Text.Encoding.ASCII.GetBytes(message);
                // Send the message to the connected TcpServer. 
                Console.WriteLine(data.Length.ToString());
                Console.WriteLine("Sent: {0}", message);
                stream = client.GetStream();
                stream.Write(data, 0, data.Length);
                
                

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                

                // Read the first batch of the TcpServer response bytes.
                bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);
                textBox2.Text = responseData;
                //int num = 1;
                //try
                //{
                //    num = Convert.ToInt32(responseData);
                //}
                //catch
                //{
                //    num = 1;
                //}
                //drawingArea.DrawLine(blackPan, num*10, 10, num*20, 20);

               
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (IOException e)
            {
                Console.WriteLine("IOException: {0}", e);
            }
            Console.WriteLine("\n Press Enter to continue...");
            
        }
        

    }
}
