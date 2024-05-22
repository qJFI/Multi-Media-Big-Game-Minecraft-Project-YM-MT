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
        Bitmap SunImg = new Bitmap("Images/sun.png");
        Rectangle rctSrc, rctDst;

        public class BasicActor
        {
            public int X, Y, W, H;
            public Bitmap img;
            public int helperVar1, helperVar2;
        }

        BasicActor Sun = new BasicActor();
        List<BasicActor> BasicActors = new List<BasicActor>();

        int stX=0,stY=0;
        Timer t= new Timer();
        int ctTimer = 0;
        //global ex, ey
        int ex = -1;
        int ey = -1;
        public Form1()
        {

            this.WindowState = FormWindowState.Maximized;
            Load += Form1_Load;
            Paint += Form1_Paint;
            MouseMove += Form1_MouseMove;
            MouseDown += Form1_MouseDown;
            MouseUp += Form1_MouseUp;
            KeyDown += Form1_KeyDown;
            t.Tick += T_Tick;
            t.Interval = 100;
            t.Start();


            
        }
        void CreateSome()
        {
            rctSrc = new Rectangle(stX, stY, BackImg.Width / 4, BackImg.Height);
            rctDst = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            Sun.X = 100;
            Sun.Y = 300;
            Sun.W = ClientSize.Height/6;
            Sun.H = ClientSize.Height/6;
            Sun.img = SunImg;
            BasicActor pnn = Sun;
            BasicActors.Add(pnn);



        }
        private void T_Tick(object sender, EventArgs e)
        {

            if (ctTimer % 10==0) { 
                rctSrc.X += 1;
            }


            ctTimer++;
            DrawDouble(CreateGraphics());
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D:
                    //ex Hero.X++
                    break;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {

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
            CreateSome();
        }
      
        void DrawScene(Graphics g)
        {
            g.Clear(Color.White);
            //g.DrawImage(BackImg, 0, 0,ClientSize.Width,ClientSize.Height);
            g.DrawImage(BackImg,rctDst, rctSrc,GraphicsUnit.Pixel);

            for(int i = 0;i< BasicActors.Count; i++)
            {
                BasicActor BasicActorTrav = BasicActors[i];
                g.DrawImage(BasicActorTrav.img, BasicActorTrav.X, BasicActorTrav.Y, BasicActorTrav.W, BasicActorTrav.H);
            }
        }

        //Draw Double Buffer to solve flickering
        void DrawDouble(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}
