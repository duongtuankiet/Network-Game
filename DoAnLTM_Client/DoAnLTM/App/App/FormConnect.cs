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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace App
{
    public partial class FormConnect : Form
    {
        public FormConnect()
        {
            instance = this;
            InitializeComponent();
        }
        public Socket server;
        byte[] send = new byte[1024];
        byte[] receive = new byte[1024];
        public bool Connected = false;
        public int soTien = 0;
        public static FormConnect instance;

        private void Connect()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipep = new IPEndPoint(ip, 9999);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Connect(ipep);
            Connected = true;

        }
        private void Send(string a)
        {
            send = Encoding.ASCII.GetBytes(a);
            server.Send(send, send.Length, SocketFlags.None);
        }
        private string Receive()
        {
            int ive = server.Receive(receive);
            string a = Encoding.ASCII.GetString(receive, 0, ive);
            return a;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Connect();
            if (Connected)
            {
                string a = Receive();
                soTien = int.Parse(a);
                this.Hide();
                Main f = new Main();
                f.ShowDialog();
            }
        }
    }
}
