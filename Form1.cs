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
  /*  public class Block
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
*/
    public class BasicActor
    {
        public int X, Y, W, H;
        public List<Bitmap> imgs = new List<Bitmap>();
        public int iframe=0;
        public List<int> Vars = new List<int>();
    }
    class Animation
    {
        public List<Bitmap> imgs = new List<Bitmap>();

    }
    class Group // for example : Blocks 
    {
        public string groupName;
        public List<Animation> Animations = new List<Animation>(); 

    }


    public partial class Form1 : Form
    {
        Bitmap off;
        Bitmap BackImg = new Bitmap("Images/Back.png");
        
        Bitmap SunImg = new Bitmap("Images/sun.png");
        Bitmap HeroImg = new Bitmap("Images/hero1.png");
        Rectangle rctSrc, rctDst;
        BasicActor hero;
        BasicActor Sun;
        List<BasicActor> SingleActors = new List<BasicActor>();
        int iframe = 0;
        List<Group> Groups = new List<Group>();

        int stX = 0, stY = 0;
        Timer t = new Timer();
        int ctTimer = 0;
        int ex = -1;
        int ey = -1;
       /* List<Block> blocks = new List<Block>();*/

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
            //Sky
            rctSrc = new Rectangle(stX, stY, BackImg.Width / 2, BackImg.Height);
            rctDst = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);

            //Sun
            Sun = new BasicActor();
            Sun.X = ClientSize.Width + 100;
            Sun.Y = 300;
            Sun.W = ClientSize.Height / 6;
            Sun.H = ClientSize.Height / 6;
            Sun.imgs.Add(SunImg);
            Sun.Vars.Add(0);
            Sun.Vars.Add(0);
            Sun.Vars.Add(3);
            SingleActors.Add(Sun);

            //Hero
            hero = new BasicActor();
            hero.W = HeroImg.Width / 2;
            hero.H = HeroImg.Height / 3;
            hero.X = ClientSize.Width/2 ;
            hero.Y = ClientSize.Height- hero.H;
            string abo3le = Groups[0].groupName;
            hero.imgs = Groups[0].Animations[0].imgs; //choosed group

            SingleActors.Add(hero);
        }

        private void T_Tick(object sender, EventArgs e)
        {
            if (ctTimer % 2 == 0)
            {
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
               
            }

            if (Sun.Y <= 0 && Sun.Vars[1] == 0)
                Sun.Vars[1] = 1;
            if (Sun.X >= ClientSize.Width + 100)
                Sun.Vars[1] = 2; //finish

            ctTimer++;
            DrawDouble(CreateGraphics());
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {

                case Keys.D:
                    hero.X+=5;
                    hero.iframe++;
                    break;
                case Keys.A:
                    hero.X-=5;
                    break;
                case Keys.W:
                    hero.Y -= 5; //shouldn't work
                    break;
                case Keys.S:
                    hero.Y += 5;
                    break;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e) { }

        private void Form1_MouseDown(object sender, MouseEventArgs e) { }

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
            ImagesReady();
            CreateSome();
            
        }

        void DrawScene(Graphics g)
        {
            g.Clear(Color.White);
            g.DrawImage(BackImg, rctDst, rctSrc, GraphicsUnit.Pixel);

            for (int i = 0; i < SingleActors.Count; i++)
            {
                BasicActor BasicActorTrav = SingleActors[i];
                g.DrawImage(BasicActorTrav.imgs[BasicActorTrav.iframe % BasicActorTrav.imgs.Count], BasicActorTrav.X, BasicActorTrav.Y, BasicActorTrav.W, BasicActorTrav.H);
            }

           /* for (int i = 0; i < blocks.Count; i++)
            {
                Block ptrav = blocks[i];
                g.DrawImage(ptrav.img, ptrav.X, ptrav.Y, ptrav.img.Width, ptrav.img.Height);
            }*/
        }


        void ImagesReady() //this function to add the photos in the memory
        {

            
            Group pnn = new Group();
            pnn.groupName = "hero";
            Groups.Add(pnn);


            Animation heroRight = new Animation();
            for(int i = 0;i < 5;i++)
            {
                heroRight.imgs.Add(new Bitmap("Images/SimpleSteve/" + i + ".png"));
            }
            Groups[0].Animations.Add(heroRight);

            

        }
        void DrawDouble(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}
