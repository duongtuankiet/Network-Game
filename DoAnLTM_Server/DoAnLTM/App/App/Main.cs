using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;

namespace App
{
    public partial class Main : Form
    {
        string currentDir = Environment.CurrentDirectory;

        Point startPoint;
        Random rand = new Random();
        int countDice = 0;
        private Point draggingPoint;
        private Point imageOffset;
        // Để dừng khi đổ xúc xắc đủ 100 lần
        private PictureBox draggingImage;
        Socket[] client;
        byte[] send = new byte[1024];
        byte[] receive = new byte[1024];
        bool Connected = false;
        int count = 0;
        int[] tiso = new int[3];
        int[] tienClient;
        int[] TongTienDaDat = new int[10];
        int[] TongTienDaAn = new int[10];
        string[] InRa = new string[6] { "Nai", "Bau", "Ga", "Ca", "Cua", "Tom" };
        List<List<int>> n = new List<List<int>>();
        List<PictureBox> pictureBoxList = new List<PictureBox>();
        bool DoubleClickButton = false;
        bool[] tisoBool = new bool[6];

        public Main()
        {
            Connected = FormConnect.instance.Connected;
            count = FormConnect.instance.count;
            client = FormConnect.instance.client;
            tienClient = FormConnect.instance.tienClient;

            //Nhan so tien da dat
            /*timer3 = new Timer();
            timer3.Tick += timer3_Tick;
            timer3.Interval = 1000;
            timer3.Start();*/

            //Gui ti so
            /*timer5 = new Timer();
            timer5.Tick += timer5_Tick;
            timer5.Interval = 2000;
            timer5.Start();*/


            InitializeComponent();

        }
        /*       private void Connect()
               {
                   IPAddress ip = IPAddress.Parse("127.0.0.1");
                   IPEndPoint ipep = new IPEndPoint(ip, 9999);
                   Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                   server.Bind(ipep);
                   server.Listen(10);
                   Console.WriteLine("Server listening...");
                   client = new Socket[10];
                   client[0] = server.Accept();
                   Connected = true;
                   Send("You are connected...");
               }*/
        private void Send(string a, int countClient)
        {
            try
            {
                send = Encoding.ASCII.GetBytes(a);
                client[countClient].Send(send, send.Length, SocketFlags.None);
            }
            catch
            {
            }
        }
        private string Receive(int countClient)
        {
            try
            {
                int ive = client[countClient].Receive(receive);
                string a = Encoding.ASCII.GetString(receive, 0, ive);
                return a;
            }
            catch
            {
                return null;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (Connected)
            {
                if (countDice == 0 && DoubleClickButton == false)
                {
                    DoubleClickButton = true;
                    richTextBox1.Visible = true;
                    pictureBox1.Visible = true;
                    pictureBox2.Visible = true;
                    pictureBox7.Visible = true;
                    timer1 = new Timer();
                    timer1.Tick += timer1_Tick;
                    timer1.Interval = 10;
                    timer1.Start();
                }
            }
        }
        private void Dice()
        {
            List<Bitmap> list = new List<Bitmap>();
            list.Add(new Bitmap(App.Properties.Resources.ho));
            list.Add(new Bitmap(App.Properties.Resources.bau));
            list.Add(new Bitmap(App.Properties.Resources.ga));
            list.Add(new Bitmap(App.Properties.Resources.ca));
            list.Add(new Bitmap(App.Properties.Resources.cua));
            list.Add(new Bitmap(App.Properties.Resources.tom));
            Random random = new Random();
            int a = random.Next(0, list.Count - 1);
            int b = random.Next(0, list.Count - 1);
            int c = random.Next(0, list.Count - 1);
            tiso[0] = a;
            tiso[1] = b;
            tiso[2] = c;
            pictureBox1.Image = list[a];
            pictureBox2.Image = list[b];
            pictureBox7.Image = list[c];

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Dice();
            countDice++;
            if (countDice > 100)
            {
                timer1.Stop();
                countDice = 0;
                timer2 = new Timer();
                timer2.Tick += timer2_Tick;
                timer2.Interval = 3000;
                timer2.Start();

                timer3 = new Timer(); //Nhan so tien da dat cua client
                timer3.Tick += timer3_Tick;
                timer3.Interval = 1000;
                timer3.Start();
                //gui ti so
                timer5 = new Timer();
                timer5.Tick += timer5_Tick;
                timer5.Interval = 2000;
                timer5.Start();

            }
        }


        private void timer2_Tick(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            pictureBox7.Visible = false;
            richTextBox1.Visible = false;
            timer2.Stop();
        }
        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void Main_Load(object sender, EventArgs e)
        {
            KhoiTao();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }
        public void KhoiTao()
        {
            for (int i = 0; i < count; i++)
            {
                /*string[] rev = Receive(i).Split(',');
                int[] a = Array.ConvertAll(rev, int.Parse);*/
                n.Add(new List<int>() { 0, 0, 0, 0, 0, 0 });
            }
        }
        public void NhanSoTienDaDat()
        {
            for (int i = 0; i < count; i++)
            {
                string[] rev = Receive(i).Split(',');

                for (int j = 0; j < 6; j++)
                {
                    n[i][j] = int.Parse(rev[j]);
                }
            }
            int dem = 0;
            string a = null;
            int demin = 0;
            foreach (List<int> innerList in n)
            {
                foreach (int element in innerList)
                {
                    a += InRa[demin] + " " + element + ", ";
                    TongTienDaDat[dem] += element;
                    demin++;
                }
                listBox1.Items.Add($"client {dem}: {a}");
                dem++;

                listBox1.Items.Add("");
                a = null;
                demin = 0;
            }
            /*
                        if (tienClient[demin] - TongTienDaDat[demin] >= 0)
                        {
                            TongTienDaDat[demin] = 0;
                            demin++;*/
            // do xuc xac de xu ly


            /*foreach (List<int> innerList in n)
            {
                if (tienClient[demin] - TongTienDaDat[demin] >= 0)
                {
                    TongTienDaDat[demin] = 0;
                    foreach (int element in innerList)
                    {
                        tienClient[demin] -= element;
                    }
                }
                demin++;
            }
            for (int i = 0; i < count; i++)
            {
                TongTienDaAn[i] += n[i][tiso[0]] * 2;
                TongTienDaAn[i] += n[i][tiso[1]] * 2;
                TongTienDaAn[i] += n[i][tiso[2]] * 2;
                tienClient[i] += TongTienDaAn[i];
                TongTienDaAn[i] = 0;
            }*/


            //-----------------------------------------------------------------------
            tisoBool[tiso[0]] = true;
            tisoBool[tiso[1]] = true;
            tisoBool[tiso[2]] = true;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < tisoBool.Length; j++)
                {
                    if (tisoBool[j] && n[i][j] > 0)
                    {
                        tienClient[i] += n[i][j];
                    }
                    else
                    {
                        tienClient[i] -= n[i][j];
                    }
                }
            }
            for (int i = 0; i < tisoBool.Length; i++)
            {
                tisoBool[i]= false;
            }
            /*  for (int i = 0; i < count; i++)
              {
                 *//* if (tienClient[i] <= 0)
                  {
                      Send($"{a}{0}", i);
                  }
                  else
                  {
                      Send($"{a}{tienClient[i]}", i);
                  }*//*
              }*/
            dem = 0;
            /*}
            else
            {
                *//* Send($"{a}{tienClient[j]}", j);*//*
            }*/

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            NhanSoTienDaDat();
            timer3.Stop();

        }
        public void GuiTiSo()
        {
            string a = null;
            for (int i = 0; i < tiso.Length; i++)
            {
                a += tiso[i] + ", ";
            }


            for (int i = 0; i < count; i++)
            {
                if (tienClient[i] <= 0)
                {
                    Send($"{a}{0}", i);
                }
                else
                {
                    Send($"{a}{tienClient[i]}", i);
                }
            }

        }
        private void timer5_Tick(object sender, EventArgs e)
        {
            GuiTiSo();
            DoubleClickButton = false;
            timer5.Stop();
        }
        private void timer4_Tick(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
