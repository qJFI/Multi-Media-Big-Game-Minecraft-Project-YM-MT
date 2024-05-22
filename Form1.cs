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

    public class Block
    {
        public int X;
        public int Y;
        public Bitmap img;

        public Block(int x, int y, Bitmap texture)
        {
            X = x;
            Y = y;
            img = texture;
        }
        private Bitmap BlockImages(BlockType type)
        {
            switch (type)
            {
                case BlockType.Grass:
                    return new Bitmap("Images/GrassBlock.jpg");
                case BlockType.Dirt:
                    return new Bitmap("Images/DirtBlock.jpg");
                case BlockType.Stone:
                    return new Bitmap("Images/StoneBlock.jpg");
                // ay block tanya hna
                default:
                    throw new Exception("Unknown block type!");
            }
        }
    }

    public enum BlockType
    {
        Grass,
        Dirt,
        Stone,
    }
    public class BasicActor
    {
        public int X, Y, W, H;
        public Bitmap img;
        public List<int> Vars = new List<int>();
    }

    public partial class Form1 : Form
    {
        Bitmap off;
        Bitmap BackImg = new Bitmap("Images/Back.png");
        Bitmap SunImg = new Bitmap("Images/sun.png");
        Rectangle rctSrc, rctDst;

       

        BasicActor Sun = new BasicActor();
        List<BasicActor> BasicActors = new List<BasicActor>();

        int stX=0,stY=0;
        Timer t= new Timer();
        int ctTimer = 0;
        //global ex, ey
        int ex = -1;
        int ey = -1;
        List<Block> blocks = new List<Block>();

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
    
               
                rctSrc = new Rectangle(stX, stY, BackImg.Width / 2, BackImg.Height);
                rctDst = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
                Sun.X = ClientSize.Width+100;
                Sun.Y = 300;
                Sun.W = ClientSize.Height/6;
                Sun.H = ClientSize.Height/6;
                Sun.img = SunImg;
                // ct ->[0] flag ->[1] speed ->[2]
                Sun.Vars.Add(0);
                Sun.Vars.Add(0);
                Sun.Vars.Add(3);



            BasicActor pnn = Sun;
            BasicActors.Add(pnn);



        }
        private void T_Tick(object sender, EventArgs e)
        {

            if (ctTimer % 2==0) { 
                rctSrc.X += 1;



                if (Sun.Vars[1] == 0)
                {

                    Sun.X -= 3 * Sun.Vars[2];
                    Sun.Y -= Sun.Vars[2];
                }
                if (Sun.Vars[1] == 1)
                {
                    Sun.X -= 3 * Sun.Vars[2];
                    Sun.Y += Sun.Vars[2];
                }


                /*
                 *   if we will do effects
                 *  if (Sun.Vars[1]==0)
                 * if (Sun.Vars[1] == 2)
                 {
                     Sun.X -= 3 * Sun.Vars[2];
                     Sun.Y -= Sun.Vars[2];
                 }
                 if (Sun.Vars[1] == 3)
                 {
                     Sun.X -= 3 * Sun.Vars[2];
                     Sun.Y += Sun.Vars[2];
                 }*/






            }

            if (Sun.Y <= 0 && Sun.Vars[1] == 0)
                Sun.Vars[1] = 1;
            if (Sun.X >= ClientSize.Width+100)
                Sun.Vars[1] = 2; //finish
       
         

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
            

            for (int i = 0; i < blocks.Count; i++)
            {
                Block ptrav = blocks[i];
                g.DrawImage(ptrav.img, ptrav.X, ptrav.Y, ptrav.img.Width, ptrav.img.Height);
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
