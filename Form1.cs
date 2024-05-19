using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Multi_Media_Minecraft_Project_YM_MT
{
    public partial class Form1 : Form
    {
        Bitmap off;
        Bitmap BackImg = new Bitmap("Images/Back.jpg");
        int ex = -1;
        int ey = -1;
        public Form1()
        {

            this.WindowState = FormWindowState.Maximized;
            Load += Form1_Load;
            Paint += Form1_Paint;
            MouseMove += Form1_MouseMove;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            ex = e.X;
            ey = e.Y;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDouble(e.Graphics);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(ClientSize.Width, ClientSize.Height);
        }

        void DrawScene(Graphics g)
        {
            g.Clear(Color.White);
            g.DrawImage(BackImg, 0, 0,ClientSize.Width,ClientSize.Height);
        }
        void DrawDouble(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}
