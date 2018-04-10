using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ServerSocketForm
{
    public partial class Form1 : Form
    {
        private TcpListener listener;
        private Thread t;
        private TcpClient client;

        private Graphics drawingArea;
        private readonly Pen blackPan = new Pen(Color.Green);
        private readonly List<Point> points = new List<Point>();

        private CoorinateSystemDirection mainDirection = CoorinateSystemDirection.Up;

        private readonly double multiplier = 1;

        public enum Direction
        {
            Left = -1,
            Right = 1,
            Forward = 0
        }

        public enum CoorinateSystemDirection
        {
            Up = 0,
            Right = 1,
            Down = 2,
            Left = 3
        }

        public Form1()
        {
            InitializeComponent();
            points.Add(new Point(300, 300));
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
            Invoke(new Action(() => textBox1.Text = "Server starterd"));
            while (true)
            {
                client = listener.AcceptTcpClient();
                Invoke(
                    new Action(
                        () => textBox1.Text = string.Format("New connection from {0}", client.Client.RemoteEndPoint)));
                ThreadPool.QueueUserWorkItem(ProcessClient, client);
            }
        }

        private void ProcessClient(object state)
        {
            var client = state as TcpClient;

            try
            {
                var reader = new StreamReader(client.GetStream());

                var s = String.Empty;

                s = reader.ReadLine();
                Invoke(new Action(() => ProcessData(s)));
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

        private void ProcessData(string data)
        {
            textBox3.Text += data+System.Environment.NewLine;
            try
            {
                var stepDirection = data.Split('*');
                if (stepDirection.Length > 1)
                {
                    Console.WriteLine("step {0} dir {1}", stepDirection[0], stepDirection[1]);
                    Double step = Step(Double.Parse(stepDirection[0]))/100;
  
                    var direction = CommandToDirection(stepDirection[1]);

                    DrawFunction(Convert.ToInt32(step), direction);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
                var client = new TcpClient(textBox6.Text, Int32.Parse(textBox7.Text));

                var writer = new StreamWriter(client.GetStream());

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

        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            points.Clear();
            points.Add(new Point(300,300));
            mainDirection = CoorinateSystemDirection.Up;
            //DrawFunction(Step(2), CommandToDirection("R"));
            //DrawFunction(Step(2), CommandToDirection("R"));
            //DrawFunction(Step(2), CommandToDirection("R"));
            pictureBox1.CreateGraphics().Clear(Color.Aqua);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SendCommandToClient("L");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SendCommandToClient("R");
        }

        private double Step(double time)
        {
            return multiplier*time;
        }

        public Direction CommandToDirection(string direction)
        {
            Direction returnValue;

            switch (direction)
            {
                case "R":
                    returnValue = Direction.Right;
                    break;
                case "L":
                    returnValue = Direction.Left;
                    break;
                default:
                    returnValue = Direction.Forward;
                    break;
            }

            return returnValue;
        }

        private Point CalculatePoint(Point prevPoint, int length, Direction direction)
        {
            var newPoint = new Point();
            switch (mainDirection)
            {
                case CoorinateSystemDirection.Up:
                    if (direction == Direction.Right)
                    {
                        newPoint.X = prevPoint.X + length;
                        newPoint.Y = prevPoint.Y;
                    }
                    else if (direction == Direction.Left)
                    {
                        newPoint.X = prevPoint.X - length;
                        newPoint.Y = prevPoint.Y;
                    }
                    break;
                case CoorinateSystemDirection.Right:
                    if (direction == Direction.Right)
                    {
                        newPoint.X = prevPoint.X;
                        newPoint.Y = prevPoint.Y + length;
                    }
                    else if (direction == Direction.Left)
                    {
                        newPoint.X = prevPoint.X;
                        newPoint.Y = prevPoint.Y - length;
                    }
                    break;
                case CoorinateSystemDirection.Down:
                    if (direction == Direction.Right)
                    {
                        newPoint.X = prevPoint.X - length;
                        newPoint.Y = prevPoint.Y;
                    }
                    else if (direction == Direction.Left)
                    {
                        newPoint.X = prevPoint.X + length;
                        newPoint.Y = prevPoint.Y;
                    }
                    break;
                case CoorinateSystemDirection.Left:
                    if (direction == Direction.Right)
                    {
                        newPoint.X = prevPoint.X;
                        newPoint.Y = prevPoint.Y - length;
                    }
                    else if (direction == Direction.Left)
                    {
                        newPoint.X = prevPoint.X;
                        newPoint.Y = prevPoint.Y + length;
                    }
                    break;
                default:
                    break;
            }


            return newPoint;
        }


        private void DrawFunction(int length, Direction direction)
        {
            drawingArea = pictureBox1.CreateGraphics();

            var newPoint = CalculatePoint(points[points.Count - 1], length, direction);
            
            points.Add(newPoint);
            Console.WriteLine("DrawFunction {0} {1} {2} {3}", (int) mainDirection, (int) direction,
            ((int) mainDirection + (int) direction),
            ((int) mainDirection + (int) direction)%4);
            int temp =(int)mainDirection + (int)direction;

            switch (temp)
            {
                case -1:
                    temp = 3;
                    break;
                case 4:
                    temp = 0;
                    break;
                default:
                    break;
            }
            mainDirection = (CoorinateSystemDirection) temp;
            Console.WriteLine("Main {0} {1} - {2}", mainDirection, newPoint.X, newPoint.Y);

            blackPan.Width = 5;
            drawingArea.DrawLines(blackPan, points.ToArray());
        }
    }
}
