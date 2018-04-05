using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ServerSocketForm
{
    public partial class Form1 : Form
    {
        TcpListener listener;
        Thread t;
        TcpClient client;
        public Form1()
        {
            InitializeComponent();
        } 

        private void StartListener()
        {
            t = new Thread(RunListener);
            t.Start();
        }

        private void RunListener()
        {
            listener = new TcpListener(IPAddress.Parse(textBox4.Text), Int32.Parse(textBox5.Text));
            listener.Start();
            Invoke(new Action(() => textBox1.Text="Server starterd"));
            while (true)
            {
                client = listener.AcceptTcpClient();
                this.Invoke(
                    new Action(() =>textBox1.Text = string.Format("New connection from {0}", client.Client.RemoteEndPoint)));
                ThreadPool.QueueUserWorkItem(ProcessClient, client);
            }
        }

        private void ProcessClient(object state)
        {
            TcpClient client = state as TcpClient;
            
            try
            {
                StreamReader reader = new StreamReader(client.GetStream());

                string s = String.Empty;

                s = reader.ReadLine();
                    Invoke(new Action(()=>textBox3.Text += s));
                    Console.WriteLine("From client -> " + s);

                reader.Close();

                Console.WriteLine("Closing client connection!");
            }
            catch (IOException)
            {
                Console.WriteLine("Problem with client communication. Exiting thread.");
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
        
    }



        private void button1_Click(object sender, EventArgs e)
        {
            StartListener();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendCommandToClient(textBox2.Text);
        }


        private void SendCommandToClient(string text)
        {
            try
            {
                TcpClient client = new TcpClient(textBox6.Text, Int32.Parse(textBox7.Text));

                StreamWriter writer = new StreamWriter(client.GetStream());
                
                writer.WriteLine(text);
                writer.Flush();


                writer.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void ProcessClientRequests(object argument)
        {
            TcpClient client = (TcpClient)argument;
            try
            {
                StreamReader reader = new StreamReader(client.GetStream());
                StreamWriter writer = new StreamWriter(client.GetStream());
                string s = String.Empty;
                while (!(s = reader.ReadLine()).Equals("Exit") || (s == null))
                {
                    //textBox3.Text = s;
                    Console.WriteLine("From client -> " + s);
                    writer.WriteLine("From server -> " + s);
                    writer.Flush();
                }
                reader.Close();
                writer.Close();
                client.Close();
                Console.WriteLine("Closing client connection!");
            }
            catch (IOException)
            {
                Console.WriteLine("Problem with client communication. Exiting thread.");
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SendCommandToClient("L");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SendCommandToClient("R");
        }
    }
}
