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
        private Thread serverThread;
        private TcpClient client;

        private Graphics drawingArea;
        Bitmap DrawArea;
        private readonly Pen blackPan = new Pen(Color.Black);
        private readonly List<Point> points = new List<Point>();

        private CoorinateSystemDirection mainDirection = CoorinateSystemDirection.Up;

        private readonly double multiplier = 100;

        Graphics g;
            

            Pen mypen = new Pen(Color.Black);

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
            points.Add(new Point(pictureBox1.Height/2, pictureBox1.Width/2));
            // Set the size of the PictureBox control.
            //  this.pictureBox1.Size = new System.Drawing.Size(954, 547);

            //Set the SizeMode to center the image.
            //  this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            panel1.AutoScroll = true;

            // Set the border style to a three-dimensional border.


            // Set the image property.
            pictureBox1.Image = Image.FromFile(@"D:\\Sanyessz\\allamvizsga\\Multithread\\cat.jpg");

            DrawArea = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = DrawArea;
            //new Bitmap(typeof(Button), "D://Sanyessz//allamvizsga//Multithread//cat.jpg");

            g = Graphics.FromImage(DrawArea);
            serverIPtextBox.Text = GetLocalIPAddress();
            Console.WriteLine(GetLocalIPAddress());
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private void StartListener()
        {
            serverThread = new Thread(RunListener);
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        private void RunListener()
        {
            listener = new TcpListener(IPAddress.Parse(serverIPtextBox.Text), Int32.Parse(serverPorttextBox.Text));
            listener.Start();
            Invoke(new Action(() => serverMessagetextBox.Text = "Server starterd"));
            while (true)
            {
                client = listener.AcceptTcpClient();
                Invoke(
                    new Action(
                        () =>
                            serverMessagetextBox.Text =
                                string.Format("New connection from {0}", client.Client.RemoteEndPoint)));
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
            
            try
            {
                var stepDirection = data.Split('*');
                if (stepDirection.Length > 1)
                {
                    Console.WriteLine("step {0} dir {1}", stepDirection[0], stepDirection[1]);
                    var step = Step(Math.Round(Double.Parse(stepDirection[0])/1000));
                    Console.WriteLine("Step: {0}", step);
                    clientOutputtextBox.AppendText(data + "->" + step + Environment.NewLine);
                    var direction = CommandToDirection(stepDirection[1]);

                    DrawFunction(Convert.ToInt32(step), direction);
                }
                else
                {
                    clientOutputtextBox.AppendText(data + Environment.NewLine);
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
            SendCommandToClient(inputtextBox.Text);
        }


        private void SendCommandToClient(string text)
        {
            try
            {
                var client = new TcpClient(clientIPtextBox.Text, Int32.Parse(clientPorttextBox.Text));

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
            clientOutputtextBox.Text = "";
            points.Clear();
            points.Add(new Point(pictureBox1.Height / 2, pictureBox1.Width / 2));
            mainDirection = CoorinateSystemDirection.Up;
            //DrawFunction(Step(2), CommandToDirection("R"));
            //DrawFunction(Step(2), CommandToDirection("R"));
            //DrawFunction(Step(2), CommandToDirection("R"));
            pictureBox1.Image = null;
            //g.FillRectangle(Brushes.Black, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
            g = Graphics.FromImage(DrawArea);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SendCommandToClient(String.Concat(inputtextBox.Text, "*L"));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SendCommandToClient(String.Concat(inputtextBox.Text, "*R"));
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
            var temp = (int) mainDirection + (int) direction;

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
            //drawingArea.DrawLines(blackPan, points.ToArray());

            g = Graphics.FromImage(DrawArea);
            g.DrawLines(blackPan, points.ToArray());

            pictureBox1.Image = DrawArea;

            g.Dispose();
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SendCommandToClient("init");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SendCommandToClient("start");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SendCommandToClient("stop");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SendCommandToClient("battery");
        }

        private void btnExitProgram_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //  serverThread.
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ctrlKeyDown = false;

            shiftKeyDown = false;
            Graphics g;
            g = Graphics.FromImage(DrawArea);

            Pen mypen = new Pen(Brushes.Black);
            g.DrawLine(mypen, 0, 0, 2000, 2000);
            g.Clear(Color.White);
            g.Dispose();
            WindowState = FormWindowState.Maximized;
           // panel1.Size = new Size(Form1.,Screen.FromControl(this).Bounds.Height);
            BackColor = Color.DarkGray;

        }

        private void Form4_MouseWheel(object sender, MouseEventArgs e)
        {
            var IsGoUp = e.Delta > 0 ? true : false;

            Console.WriteLine(IsGoUp.ToString());
            Console.WriteLine(panel1.HorizontalScroll.Value);
                
                if (IsGoUp)
                {
                    Console.WriteLine(pictureBox1.Size.Height + 50);
                    pictureBox1.Size = new Size(pictureBox1.Size.Height + 50, pictureBox1.Size.Width + 50);
                }

                if (!IsGoUp)
                {
                    Console.WriteLine(pictureBox1.Size.Height - 50);
                    pictureBox1.Size = new Size(pictureBox1.Size.Height - 50, pictureBox1.Size.Width - 50);
                }
            

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Graphics g;
            g = Graphics.FromImage(DrawArea);

            Pen mypen = new Pen(Color.Black);

            g.DrawLine(mypen, 0, 0, 200, 150);

            pictureBox1.Image = DrawArea;

            g.Dispose();
        }

        private bool Dragging;
        private int xPos;
        private int yPos;

        private bool ctrlKeyDown;

        private bool shiftKeyDown;

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            var c = sender as Control;
            if (Dragging && c != null)
            {
                c.Top = e.Y + c.Top - yPos;
                c.Left = e.X + c.Left - xPos;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Dragging = true;
                xPos = e.X;
                yPos = e.Y;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            ctrlKeyDown = e.Control;

            shiftKeyDown = e.Shift;
            Console.WriteLine("DOWN");
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            ctrlKeyDown = e.Control;

            shiftKeyDown = e.Shift;
            Console.WriteLine("UP");
        }
    }
}
