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

namespace App
{
    public partial class FormConnect : Form
    {
        public Socket[] client;
        byte[] send = new byte[1024];
        byte[] receive = new byte[1024];
        public bool Connected = false;
        public int count = 0;
        public int[] tienClient;
        public static FormConnect instance;
        public FormConnect()
        {
            instance = this;
            InitializeComponent();
        }
        private void Connect()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipep = new IPEndPoint(ip, 9999);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(ipep);
            server.Listen(10);
            textBox1.Text = "Server is listenning......";
            client = new Socket[10];
            tienClient = new int[10];
            while (count < 1)
            {
                client[count] = server.Accept();
                tienClient[count] = 500;
               //gửi cho client lần đầu khi kết nối
               // Send($"You are connected and have ID: {count}...",count);
                Send($"{tienClient[count]}", count);
                count++;
            }
            Connected = true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Connect();
            if (Connected)
            {
                this.Hide();
                Main f = new Main();
                f.ShowDialog();

                /*textBox1.Text = "connected";*/
            }
        }
        private void Send(string a,int count)
        {
            send = Encoding.ASCII.GetBytes(a);
            client[count].Send(send, send.Length, SocketFlags.None);
        }
        private string Receive(int count)
        {
            int ive = client[count].Receive(receive);
            string a = Encoding.ASCII.GetString(receive, 0, ive);
            return a;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
