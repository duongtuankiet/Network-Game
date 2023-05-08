using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Image = System.Drawing.Image;

namespace App
{
    public partial class Main : Form
    {
        string currentDir = Environment.CurrentDirectory;
        public Socket server;
        Point startPoint;
        Random rand = new Random();
        int countDice = 0;
        private Point draggingPoint;
        private Point imageOffset;
        // Để dừng khi đổ xúc xắc đủ 100 lần
        private PictureBox draggingImage;
        int[] baucua = new int[6];
        bool CorrectPoint = true;
        byte[] send = new byte[1024];
        byte[] receive = new byte[1024];
        bool Connected = false;
        int soTien = 0;
        string[] sotien2 = new string[3];
        bool DoubleButtonClick = false;
        List<PictureBox> pictureBoxList = new List<PictureBox>();
        public Main()
        {
            Connected = FormConnect.instance.Connected;
            server = FormConnect.instance.server;
            soTien = FormConnect.instance.soTien;

            InitializeComponent();
            /*textBox5.Text = $"{soTien}.000";*/
            /* timer3 = new Timer();
             timer3.Tick += timer3_Tick;
             timer3.Interval = 2000;
             timer3.Start();*/
            /*
                        timer4 = new Timer();
                        timer4.Tick += timer4_Tick;
                        timer4.Interval = 1000;
                        timer4.Start();*/

        }
        private void Send(string a)
        {
            try
            {
                send = Encoding.ASCII.GetBytes(a);
                server.Send(send, send.Length, SocketFlags.None);
            }
            catch
            {

            }
        }
        private string Receive()
        {
            try
            {
                int ive = server.Receive(receive);
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
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc ?", "Nếu đặt cược, hệ thống sẽ xóc xóc", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (!DoubleButtonClick)
                {
                    DoubleButtonClick = true;
                    //Gui tien cuoc len server
                    timer4 = new Timer();
                    timer4.Tick += timer4_Tick;
                    timer4.Interval = 1000;
                    timer4.Start();
                    //nhan ti so
                    timer3 = new Timer();
                    timer3.Tick += timer3_Tick;
                    timer3.Interval = 2000;
                    timer3.Start();
                }
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }
        private void Dice()
        {
            List<Bitmap> list = new List<Bitmap>();
            list.Add(new Bitmap(App.Properties.Resources.bau));
            list.Add(new Bitmap(App.Properties.Resources.cua));
            list.Add(new Bitmap(App.Properties.Resources.ca));
            list.Add(new Bitmap(App.Properties.Resources.ho));
            list.Add(new Bitmap(App.Properties.Resources.tom));
            list.Add(new Bitmap(App.Properties.Resources.ga));
            Random random = new Random();
            //pictureBox1.Image = list[random.Next(0, list.Count - 1)];
            //pictureBox2.Image = list[random.Next(0, list.Count - 1)];
            //pictureBox7.Image = list[random.Next(0, list.Count - 1)];
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
                timer2.Interval = 1000;
                timer2.Start();

            }
        }
        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                // Lưu vị trí chuột hiện tại
                startPoint = e.Location;
                // Tạo một đối tượng hình ảnh mới từ hình ảnh của PictureBox
                Image image = pictureBox3.Image;
                if (image != null)
                {
                    draggingImage = new PictureBox();
                    draggingImage.Image = image;
                    draggingImage.SizeMode = PictureBoxSizeMode.StretchImage;
                    draggingImage.Location = pictureBox3.Location;
                    draggingImage.BringToFront();
                    draggingImage.Visible = true;
                    draggingPoint = e.Location;
                    imageOffset = new Point(draggingImage.Location.X - draggingPoint.X, draggingImage.Location.Y - draggingPoint.Y);
                    // Thêm đối tượng hình ảnh vào form
                    this.Controls.Add(draggingImage);
                }
            }
        }
        private void pictureBox3_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggingImage != null)
            {
                Point newLocation = new Point(e.X + imageOffset.X, e.Y + imageOffset.Y);
                draggingImage.Location = newLocation;
            }
        }
        private void pictureBox3_MouseUp(object sender, MouseEventArgs e)
        {
            if (draggingImage != null)
            {
                // Xóa đối tượng hình ảnh mới
                this.Controls.Remove(draggingImage);
                draggingImage.Dispose();
                draggingImage = null;

                Image image = pictureBox3.Image;
                PictureBox draggingLastImage = new PictureBox();
                draggingLastImage.Image = image;
                Point LastLocation = new Point(e.X + imageOffset.X, e.Y + imageOffset.Y);
                draggingLastImage.Location = LastLocation;
                draggingLastImage.SizeMode = PictureBoxSizeMode.StretchImage;
                draggingLastImage.Click += DraggingLastImage_Click;
                //aaaaaa
                BauCua(LastLocation, 20);
                if (CorrectPoint)
                {
                    this.Controls.Add(draggingLastImage);
                    pictureBoxList.Add(draggingLastImage);
                }
                CorrectPoint = true;
            }
        }
        private void DraggingLastImage_Click(object sender, EventArgs e)
        {
            PictureBox temp = sender as PictureBox;
            this.Controls.Remove(temp);
            RutLaiTien(temp.Location, 20);

        }

        private void pictureBox4_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                // Lưu vị trí chuột hiện tại
                startPoint = e.Location;
                // Tạo một đối tượng hình ảnh mới từ hình ảnh của PictureBox
                Image image = pictureBox4.Image;
                if (image != null)
                {
                    draggingImage = new PictureBox();
                    draggingImage.Image = image;
                    draggingImage.SizeMode = PictureBoxSizeMode.StretchImage;
                    draggingImage.Location = pictureBox4.Location;
                    draggingImage.BringToFront();
                    draggingImage.Visible = true;
                    draggingPoint = e.Location;
                    imageOffset = new Point(draggingImage.Location.X - draggingPoint.X, draggingImage.Location.Y - draggingPoint.Y);
                    // Thêm đối tượng hình ảnh vào form
                    this.Controls.Add(draggingImage);
                }
            }
        }
        private void pictureBox4_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggingImage != null)
            {
                Point newLocation = new Point(e.X + imageOffset.X, e.Y + imageOffset.Y);
                draggingImage.Location = newLocation;
            }
        }
        private void pictureBox4_MouseUp(object sender, MouseEventArgs e)
        {
            if (draggingImage != null)
            {
                this.Controls.Remove(draggingImage);
                draggingImage.Dispose();
                draggingImage = null;

                Image image = pictureBox4.Image;
                PictureBox draggingLastImage = new PictureBox();
                draggingLastImage.Image = image;
                Point LastLocation = new Point(e.X + imageOffset.X, e.Y + imageOffset.Y);
                draggingLastImage.Location = LastLocation;
                draggingLastImage.SizeMode = PictureBoxSizeMode.StretchImage;
                draggingLastImage.Click += DraggingLastImage1_Click;
                //aaaaaa
                BauCua(LastLocation, 50);
                if (CorrectPoint)
                {
                    this.Controls.Add(draggingLastImage);
                    pictureBoxList.Add(draggingLastImage);
                }
                CorrectPoint = true;

            }
        }
        private void DraggingLastImage1_Click(object sender, EventArgs e)
        {
            PictureBox temp = sender as PictureBox;
            this.Controls.Remove(temp);
            RutLaiTien(temp.Location, 50);

        }
        private void pictureBox5_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                // Lưu vị trí chuột hiện tại
                startPoint = e.Location;
                // Tạo một đối tượng hình ảnh mới từ hình ảnh của PictureBox
                Image image = pictureBox5.Image;
                if (image != null)
                {
                    draggingImage = new PictureBox();
                    draggingImage.Image = image;
                    draggingImage.SizeMode = PictureBoxSizeMode.StretchImage;
                    draggingImage.Location = pictureBox5.Location;
                    draggingImage.BringToFront();
                    draggingImage.Visible = true;
                    draggingPoint = e.Location;
                    imageOffset = new Point(draggingImage.Location.X - draggingPoint.X, draggingImage.Location.Y - draggingPoint.Y);
                    // Thêm đối tượng hình ảnh vào form
                    this.Controls.Add(draggingImage);
                }
            }
        }
        private void pictureBox5_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggingImage != null)
            {
                Point newLocation = new Point(e.X + imageOffset.X, e.Y + imageOffset.Y);
                draggingImage.Location = newLocation;
            }
        }
        private void pictureBox5_MouseUp(object sender, MouseEventArgs e)
        {
            if (draggingImage != null)
            {
                this.Controls.Remove(draggingImage);
                draggingImage.Dispose();
                draggingImage = null;

                Image image = pictureBox5.Image;
                PictureBox draggingLastImage = new PictureBox();
                draggingLastImage.Image = image;
                Point LastLocation = new Point(e.X + imageOffset.X, e.Y + imageOffset.Y);
                draggingLastImage.Location = LastLocation;
                draggingLastImage.SizeMode = PictureBoxSizeMode.StretchImage;
                draggingLastImage.Click += DraggingLastImage2_Click;
                BauCua(LastLocation, 100);
                if (CorrectPoint)
                {
                    this.Controls.Add(draggingLastImage);
                    pictureBoxList.Add(draggingLastImage);
                }
                    CorrectPoint = true;
            }
        }
        private void DraggingLastImage2_Click(object sender, EventArgs e)
        {
            PictureBox temp = sender as PictureBox;
            this.Controls.Remove(temp);
            RutLaiTien(temp.Location, 100);

        }
        private void pictureBox6_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                // Lưu vị trí chuột hiện tại
                startPoint = e.Location;
                // Tạo một đối tượng hình ảnh mới từ hình ảnh của PictureBox
                Image image = pictureBox6.Image;
                if (image != null)
                {
                    draggingImage = new PictureBox();
                    draggingImage.Image = image;
                    draggingImage.SizeMode = PictureBoxSizeMode.StretchImage;
                    draggingImage.Location = pictureBox6.Location;
                    draggingImage.BringToFront();
                    draggingImage.Visible = true;
                    draggingPoint = e.Location;
                    imageOffset = new Point(draggingImage.Location.X - draggingPoint.X, draggingImage.Location.Y - draggingPoint.Y);
                    // Thêm đối tượng hình ảnh vào form
                    this.Controls.Add(draggingImage);
                }
            }
        }
        private void pictureBox6_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggingImage != null)
            {
                Point newLocation = new Point(e.X + imageOffset.X, e.Y + imageOffset.Y);
                draggingImage.Location = newLocation;
            }
        }
        private void pictureBox6_MouseUp(object sender, MouseEventArgs e)
        {
            if (draggingImage != null)
            {
                this.Controls.Remove(draggingImage);
                draggingImage.Dispose();
                draggingImage = null;

                Image image = pictureBox6.Image;
                PictureBox draggingLastImage = new PictureBox();
                draggingLastImage.Image = image;
                Point LastLocation = new Point(e.X + imageOffset.X, e.Y + imageOffset.Y);
                draggingLastImage.Location = LastLocation;
                draggingLastImage.SizeMode = PictureBoxSizeMode.StretchImage;
                draggingLastImage.Click += DraggingLastImage3_Click;
                BauCua(LastLocation, 200);
                if (CorrectPoint)
                {
                    this.Controls.Add(draggingLastImage);
                    pictureBoxList.Add(draggingLastImage);
                }
                CorrectPoint = true;
            }
        }
        private void DraggingLastImage3_Click(object sender, EventArgs e)
        {
            PictureBox temp = sender as PictureBox;
            this.Controls.Remove(temp);
            RutLaiTien(temp.Location, 200);

        }
        private void BauCua(Point location, int sotien)
        {
            if (soTien - sotien >= 0)
            {
                if (location.X > 9 && location.X < 127 && location.Y > 12 && location.Y < 128)
                {
                    baucua[0] += sotien;
                    string a = $"So tien {baucua[0]}";
                    textBox1.Text = a;
                    soTien -= sotien;
                }
                else if (location.X > 238 && location.X < 356 && location.Y > 12 && location.Y < 128)
                {
                    baucua[1] += sotien;
                    string a = $"So tien {baucua[1]}";
                    textBox2.Text = a;
                    soTien -= sotien;
                }
                else if (location.X > 467 && location.X < 585 && location.Y > 12 && location.Y < 128)
                {
                    baucua[2] += sotien;
                    string a = $"So tien {baucua[2]}";
                    textBox3.Text = a;
                    soTien -= sotien;
                }
                else if (location.X > 9 && location.X < 127 && location.Y > 223 && location.Y < 342)
                {
                    baucua[3] += sotien;
                    string a = $"So tien {baucua[3]}";
                    textBox4.Text = a;
                    soTien -= sotien;
                }
                else if (location.X > 238 && location.X < 356 && location.Y > 223 && location.Y < 342)
                {
                    baucua[4] += sotien;
                    string a = $"So tien {baucua[4]}";
                    textBox5.Text = a;
                    soTien -= sotien;
                }
                else if (location.X > 467 && location.X < 585 && location.Y > 223 && location.Y < 342)
                {
                    baucua[5] += sotien;
                    string a = $"So tien {baucua[5]}";
                    textBox6.Text = a;
                    soTien -= sotien;
                }
                else
                {
                    CorrectPoint = false;
                }
                textBox7.Text = $"{soTien}";
            }
            else
            {
                CorrectPoint = false;
            }
        }
        private void RutLaiTien(Point location, int sotien)
        {
            if (location.X > 9 && location.X < 127 && location.Y > 12 && location.Y < 128)
            {
                baucua[0] -= sotien;
                string a = $"So tien {baucua[0]}";
                textBox1.Text = a;
                soTien += sotien;
            }
            else if (location.X > 238 && location.X < 356 && location.Y > 12 && location.Y < 128)
            {
                baucua[1] -= sotien;
                string a = $"So tien {baucua[1]}";
                textBox2.Text = a;
                soTien += sotien;
            }
            else if (location.X > 467 && location.X < 585 && location.Y > 12 && location.Y < 128)
            {
                baucua[2] -= sotien;
                string a = $"So tien {baucua[2]}";
                textBox3.Text = a;
                soTien += sotien;
            }
            else if (location.X > 9 && location.X < 127 && location.Y > 223 && location.Y < 342)
            {
                baucua[3] -= sotien;
                string a = $"So tien {baucua[3]}";
                textBox4.Text = a;
                soTien += sotien;
            }
            else if (location.X > 238 && location.X < 356 && location.Y > 223 && location.Y < 342)
            {
                baucua[4] -= sotien;
                string a = $"So tien {baucua[4]}";
                textBox5.Text = a;
                soTien += sotien;
            }
            else if (location.X > 467 && location.X < 585 && location.Y > 223 && location.Y < 342)
            {
                baucua[5] -= sotien;
                string a = $"So tien {baucua[5]}";
                textBox6.Text = a;
                soTien += sotien;
            }
            textBox7.Text = $"{soTien}";
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

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }
        public void GuiTienDaDatDenServer()
        {
            string a = null;
            for (int i = 0; i < baucua.Length; i++)
            {
                a += baucua[i] + ",";
            }
            Send(a);
        }

        private void timer3_Tick(object sender, EventArgs e) // Nhan ti so
        {
            string[] n = Receive().Split(',');
            sotien2[0] = n[0];
            sotien2[1] = n[1];
            sotien2[2] = n[2];
            soTien = int.Parse(n[3]);
            textBox7.Text = soTien.ToString();
            timer3.Stop();
            timer5 = new Timer();
            timer5.Interval = 3000;
            timer6.Start();
            timer5.Tick += timer5_Tick;
            timer5.Start();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            GuiTienDaDatDenServer();
            /*string sotien = Receive();*/
            /*textBox7.Text = $"{int.Parse(sotien)}.000 vnd";*/
            DoubleButtonClick = false;
            timer4.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (PictureBox pic in pictureBoxList)
            {
                this.Controls.Remove(pic);
            }
        }

        private void timer5_Tick(object sender, EventArgs e) //timer cho xoa picturebox
        {
            timer2.Start();
            foreach (PictureBox pic in pictureBoxList)
            {
                this.Controls.Remove(pic);
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
            }
            pictureBoxList.Clear();
            for (int i = 0; i < baucua.Length; i++)
            {
                baucua[i] = 0;
            }
            timer5.Stop();
        }

        private void timer6_Tick(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            pictureBox2.Visible = true;
            pictureBox7.Visible = true;
            richTextBox1.Visible = true;
            string a = sotien2[0];
            string b = sotien2[2];
            string c = sotien2[1];
            if (a == "0")
            {
                pictureBox1.Image = new Bitmap(Properties.Resources.ho);
            }
            if (a == "1")
            {
                pictureBox1.Image = new Bitmap(Properties.Resources.bau);
            }
            if (a == "2")
            {
                pictureBox1.Image = new Bitmap(Properties.Resources.ga);
            }
            if (a == "3")
            {
                pictureBox1.Image = new Bitmap(Properties.Resources.ca);
            }
            if (a == "4")
            {
                pictureBox1.Image = new Bitmap(Properties.Resources.cua);
            }
            if (a == "5")
            {
                pictureBox1.Image = new Bitmap(Properties.Resources.tom);
            }
            if (b == " 0")
            {
                pictureBox2.Image = new Bitmap(Properties.Resources.ho);
            }
            if (b == " 1")
            {
                pictureBox2.Image = new Bitmap(Properties.Resources.bau);
            }
            if (b == " 2")
            {
                pictureBox2.Image = new Bitmap(Properties.Resources.ga);
            }
            if (b == " 3")
            {
                pictureBox2.Image = new Bitmap(Properties.Resources.ca);
            }
            if (b == " 4")
            {
                pictureBox2.Image = new Bitmap(Properties.Resources.cua);
            }
            if (b == " 5")
            {
                pictureBox2.Image = new Bitmap(Properties.Resources.tom);
            }
            if (c == " 0")
            {
                pictureBox7.Image = new Bitmap(Properties.Resources.ho);
            }
            if (c == " 1")
            {
                pictureBox7.Image = new Bitmap(Properties.Resources.bau);
            }
            if (c == " 2")
            {
                pictureBox7.Image = new Bitmap(Properties.Resources.ga);
            }
            if (c == " 3")
            {
                pictureBox7.Image = new Bitmap(Properties.Resources.ca);
            }
            if (c == " 4")
            {
                pictureBox7.Image = new Bitmap(Properties.Resources.cua);
            }
            if (c == " 5")
            {
                pictureBox7.Image = new Bitmap(Properties.Resources.tom);
            }
            Main mn = new Main();
            mn.SuspendLayout();
            timer6.Stop();
        }
    }
}
