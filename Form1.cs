using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Multi_Media_Minecraft_Project_YM_MT
{
    public class BasicActor
    {
        public int X, Y, W, H;
        public List<Bitmap> imgs = new List<Bitmap>();
        public int iframe = 0;
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

    public class Block
    {
        public int X, Y, W, H;
        public Bitmap Img;
    }

    public class Hero
    {
        public int X, Y, W, H;
        public List<Bitmap> imgs;
        public int iframe = 0;
        public int isHeroStable = 0;
        public int dir = 1; //right -1 left
        public int speed = 10;
    }

    public partial class Form1 : Form
    {
        Bitmap off;
        Bitmap BackImg = new Bitmap("Images/Back.png");
        Bitmap SunImg = new Bitmap("Images/sun.png");
        Bitmap HeroImg = new Bitmap("Images/hero1.png");
        Rectangle rctSrc, rctDst;
        Hero hero;
        BasicActor Sun;
        List<BasicActor> SingleActors = new List<BasicActor>();
        int iframe = 0;
        int zoom = -10;
        int zoomRange = 10;
        List<Group> Groups = new List<Group>();

        int stX = 0, stY = 0;
        Timer t = new Timer();
        int ctTimer = 0;
        int ex = -1;
        int ey = -1;
        List<List<Block>> blocks2D = new List<List<Block>>(); // 2D list for blocks
        Random RR = new Random();

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
            // Sky
            rctSrc = new Rectangle(stX, stY, BackImg.Width / 2, BackImg.Height);
            rctDst = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);

            // Sun
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

            // Hero
            hero = new Hero();
            hero.W = HeroImg.Width / 2;
            hero.H = HeroImg.Height / 3;
            hero.X = ClientSize.Width / 2;
            hero.Y = ClientSize.Height - hero.H - 50;
            hero.imgs = new List<Bitmap>(Groups[0].Animations[0].imgs); // Choosed group

            // Create random biome blocks
            CreateRandomBiomeBlocks();
        }

        void CreateRandomBiomeBlocks()
        {
            int blockWidth = 60; // Set your block width
            int blockHeight = 60; // Set your block height
            int columns = ClientSize.Width / blockWidth;
            int rows = 10; // Number of rows of blocks
            int yPos = ClientSize.Height - blockHeight * rows +400; // Starting Y position of the bottom row

            for (int j = 0; j < rows; j++)
            {
                List<Block> rowBlocks = new List<Block>();
                for (int i = 0; i < columns; i++)
                {
                    Block blockPnn = new Block();
                    blockPnn.X = i * blockWidth;
                    blockPnn.Y = yPos + (j * blockHeight);
                    blockPnn.W = blockWidth;
                    blockPnn.H = blockHeight;

                    if (j < 5)
                    {
                        blockPnn.Img = Groups[1].Animations[0].imgs[0]; // Always grass for the first 5 rows from the bottom
                    }
                    else
                    {
                        int biomeType = RR.Next(3); // 0: Grass, 1: Stone, 2: Mixed
                        if (biomeType == 0)
                        {
                            blockPnn.Img = Groups[1].Animations[0].imgs[0]; // Grass biome
                        }
                        else if (biomeType == 1)
                        {
                            blockPnn.Img = Groups[1].Animations[0].imgs[1]; // Stone biome
                        }
                        else
                        {
                            int randomBlock = RR.Next(0,Groups[1].Animations[0].imgs.Count);
                            blockPnn.Img = Groups[1].Animations[0].imgs[randomBlock]; 
                        }
                    }

                    rowBlocks.Add(blockPnn);
                }
                blocks2D.Add(rowBlocks);
            }

            // Adjust hero position to be on top of the grass blocks
            hero.Y = yPos - hero.H + 10 ;
        }

        private void T_Tick(object sender, EventArgs e)
        {
            if (ctTimer % 1 == 0)
            {
                if (rctSrc.X + rctSrc.Width < BackImg.Width)
                    rctSrc.X++;
                else
                    rctSrc.X = 0;

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

            //always
            if (hero.isHeroStable == 0) //always
            {
                if (hero.dir == 1)
                {
                    hero.imgs = Groups[0].Animations[0].imgs;
                }

                if (hero.dir == -1)
                {
                    hero.imgs = Groups[0].Animations[1].imgs;
                }
            }
            else
            {
                hero.isHeroStable--;
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
                    hero.isHeroStable = 10;
                    hero.X += hero.speed;
                    hero.iframe++;
                    hero.imgs = Groups[0].Animations[2].imgs;
                    hero.dir = 1;
                    break;
                case Keys.A:
                    hero.X -= hero.speed;
                    hero.isHeroStable = 10;
                    hero.iframe++;
                    hero.imgs = Groups[0].Animations[3].imgs;
                    hero.dir = -1;
                    break;
                case Keys.W:
                    //hero.Y -=  hero.speed; //shouldn't work
                    break;
                case Keys.S:
                    //hero.Y +=  hero.speed;//shouldn't work
                    break;
                case Keys.Z:
                    Zoom(1);
                    break;
                case Keys.C:
                    Zoom(2);
                    break;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e) { }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    // Left mouse button logic
                    break;
                case MouseButtons.Right:
                    // Right mouse button logic
                    break;
                case MouseButtons.XButton1:  //The bonus button 1
                    Text = "Testo1";
                    break;
                case MouseButtons.XButton2:  //The bonus button 2
                    Text = "Testo2";
                    break;
                case MouseButtons.Middle:  //The bonus button 2
                    Text = "Testo3";
                    break;
            }
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

            g.DrawImage(hero.imgs[hero.iframe % hero.imgs.Count], hero.X, hero.Y, hero.W, hero.H);

            for (int j = 0; j < blocks2D.Count; j++)
            {
                List<Block> rowBlocks = blocks2D[j];
                for (int i = 0; i < rowBlocks.Count; i++)
                {
                    Block block = rowBlocks[i];
                    g.DrawImage(block.Img, block.X, block.Y, block.W, block.H);
                }
            }
        }

        void ImagesReady() //this function to add the photos in the memory
        {
            Group pnn = new Group();  // 1- hero Right 2- hero Left
            pnn.groupName = "Hero";
            Groups.Add(pnn);

            pnn = new Group();
            pnn.groupName = "Blocks";
            Groups.Add(pnn);

            pnn = new Group();
            pnn.groupName = "Mobs";
            Groups.Add(pnn);

            pnn = new Group();
            pnn.groupName = "Animals";
            Groups.Add(pnn);

            Animation heroStableRight = new Animation();
            heroStableRight.imgs.Add(new Bitmap("Images/SimpleSteve/0.png"));
            Groups[0].Animations.Add(heroStableRight);

            Animation heroStableLeft = new Animation();
            heroStableLeft.imgs.Add(new Bitmap("Images/SimpleSteve/5.png"));
            Groups[0].Animations.Add(heroStableLeft);

            Animation heroRight = new Animation();
            for (int i = 1; i < 5; i++)
            {
                heroRight.imgs.Add(new Bitmap("Images/SimpleSteve/" + i + ".png"));
            }
            Groups[0].Animations.Add(heroRight);

            Animation heroLeft = new Animation();
            for (int i = 6; i < 10; i++)
            {
                heroLeft.imgs.Add(new Bitmap("Images/SimpleSteve/" + i + ".png"));
            }
            Groups[0].Animations.Add(heroLeft);

            Animation blocks = new Animation();
            blocks.imgs.Add(new Bitmap("Images/Blocks/grass.png")); // Adding Grass image
            blocks.imgs.Add(new Bitmap("Images/Blocks/grass.png")); // Adding Grass image
            blocks.imgs.Add(new Bitmap("Images/Blocks/stone.png")); // Adding Stone image
            Groups[1].Animations.Add(blocks);
        }

        void Zoom(int type)
        {
            if (type == 1) // Zoom in
            {
                zoom += zoomRange;
                hero.W += zoomRange;
                hero.H += zoomRange;
                hero.Y -= zoomRange; // Adjust Y position to keep hero on top

                for (int j = 0; j < blocks2D.Count; j++)
                {
                    List<Block> rowBlocks = blocks2D[j];
                    for (int i = 0; i < rowBlocks.Count; i++)
                    {
                        Block block = rowBlocks[i];
                        block.W += zoomRange;
                        block.H += zoomRange;
                        block.Y -= zoomRange / 2; // Adjust Y position to account for zoom
                        block.X -= zoomRange / 2; // Adjust X position to account for zoom
                    }
                }
            }
            else if (type == 2) // Zoom out
            {
                if (zoom > 0) // Ensure zoom doesn't go negative
                {
                    zoom -= zoomRange;
                    hero.W -= zoomRange;
                    hero.H -= zoomRange;
                    hero.Y += zoomRange; // Adjust Y position to keep hero on top

                    for (int j = 0; j < blocks2D.Count; j++)
                    {
                        List<Block> rowBlocks = blocks2D[j];
                        for (int i = 0; i < rowBlocks.Count; i++)
                        {
                            Block block = rowBlocks[i];
                            block.W -= zoomRange;
                            block.H -= zoomRange;
                            block.Y += zoomRange / 2; // Adjust Y position to account for zoom
                            block.X += zoomRange / 2; // Adjust X position to account for zoom
                        }
                    }
                }
            }

            DrawDouble(CreateGraphics()); // Redraw the scene after zooming
        }

        void DrawDouble(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}
