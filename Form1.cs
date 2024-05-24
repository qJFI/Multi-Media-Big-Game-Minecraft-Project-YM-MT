using System;
using System.Collections.Generic;
using System.Drawing;
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
        public int ID; //made for the Zoom 
        public int ItemType;
    }

    public class AnimatedBlock
    {
        public int X, Y, W, H;
        public List<Bitmap> imgs = new List<Bitmap>();
        public int iframe = 0;
    }

    public class Hero
    {
        public int X, Y, W, H;
        public List<Bitmap> imgs;
        public int iframe = 0;
        public int isHeroStable = 0;
        public int dir = 1; //right -1 left
        public int speed = 10;
        public bool isJumping = false;
        public int jumpCt = 20;
        public int force = 0;

        public List<InventoryItem> Inventory;

        public Hero()
        {
            Inventory = new List<InventoryItem>();
        }
    }

    public class InventoryItem
    {
        public int ItemID;
        public int Quantity;

        public InventoryItem(int itemID, int quantity)
        {
            ItemID = itemID;
            Quantity = quantity;
        }
    }

    public class Inventory
    {
        private List<InventoryItem> items;

        public Inventory()
        {
            items = new List<InventoryItem>();
        }

        public void AddItem(int itemID, int quantity)
        {
            // if the item is already in the inventory
            bool found = false;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ItemID == itemID)
                {
                    items[i].Quantity += quantity;
                    found = true;
                    break;
                }
            }

            // if the item doesnt exist add it to the inventory
            if (!found)
            {
                items.Add(new InventoryItem(itemID, quantity));
            }
        }

        public void RemoveItem(int itemID, int quantity)
        {
            // find item index
            int index = -1;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ItemID == itemID)
                {
                    index = i;
                    break;
                }
            }

            // if the item is in inventory decrease quantity
            if (index != -1)
            {
                items[index].Quantity -= quantity;

                // temove if it is not there
                if (items[index].Quantity == 0)
                {
                    items.RemoveAt(index);
                }
            }
        }

        public List<InventoryItem> GetItems()
        {
            return items;
        }

        public void Clear()
        {
            items.Clear();
        }
    }


    public class Camera
    {
        public int X, Y, Width, Height;

        public Camera(int width, int height)
        {
            Width = width;
            Height = height;
            X = 0;
            Y = 0;
        }

        public void Update(Hero hero)
        {
            X = hero.X - Width / 2-800;
            Y = hero.Y - Height / 2-400;

            // Ensure the camera doesn't go out of bounds
            if (X < 0) X = 0;
            if (Y < 0) Y = 0;
        }

        public Rectangle GetViewRect()
        {
            return new Rectangle(X, Y, Width, Height);
        }
    }

    public partial class Form1 : Form
    {
        Bitmap off;
        Bitmap BackImg = new Bitmap("Images/Back.png");
        Bitmap BorderImg = new Bitmap("Images/Border.png");
        Bitmap SunImg = new Bitmap("Images/sun.png");
        Bitmap HeroImg = new Bitmap("Images/hero1.png");
        Rectangle rctSrc, rctDst;
        Hero hero;
        AnimatedBlock breaking = null;
        BasicActor Sun;
        List<BasicActor> SingleActors = new List<BasicActor>();
 
        int iframe = 0;
        int breakingI = -1, breakingJ = -1;
        int zoom = -10;
        int isBreaking = 0;
        int isLeftClick = 0;
        int zoomRange = 10;
        List<Group> Groups = new List<Group>();

        int stX = 0, stY = 0;
        Timer t = new Timer();
        int ctTimer = 0;
        int ex = -1;
        int ey = -1;
        List<List<Block>> blocks2D = new List<List<Block>>(); // 2D list for blocks
   
        Random RR = new Random();
        Camera camera;

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
            camera = new Camera(ClientSize.Width, ClientSize.Height);
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

            // Tree
            CreateTrees();
        }

        void CreateRandomBiomeBlocks()
        {
            int blockWidth = 60; // Set your block width
            int blockHeight = 60; // Set your block height
            int columns = ClientSize.Width / blockWidth;
            int rows = 20; // Number of rows of blocks
            int yPos = ClientSize.Height - blockHeight * rows + 1000; // Starting Y position of the bottom row

            for (int i = 0; i < rows; i++)
            {
                List<Block> rowBlocks = new List<Block>();
                for (int j = 0; j < columns; j++)
                {
                    Block blockPnn = new Block();
                    blockPnn.X = j * blockWidth;
                    blockPnn.Y = yPos + (i * blockHeight);
                    blockPnn.W = blockWidth;
                    blockPnn.H = blockHeight;
                    blockPnn.ID = j;
                    if (i < 1)
                    {
                        blockPnn.Img = Groups[1].Animations[0].imgs[0]; // Always grass for the first 5 rows from the bottom
                    }
                    else if (i < 2)
                    {
                        blockPnn.Img = Groups[1].Animations[0].imgs[1];
                    }
                    else
                    {
                        int isStone = RR.Next(0, 2);

                        if (isStone == 0)
                        {
                            int randomBlock = RR.Next(4, Groups[1].Animations[0].imgs.Count);
                            blockPnn.Img = Groups[1].Animations[0].imgs[randomBlock];
                        }
                        else
                        {
                            blockPnn.Img = Groups[1].Animations[0].imgs[4];
                        }
                    }

                    rowBlocks.Add(blockPnn);
                }
                blocks2D.Add(rowBlocks);
            }

            // Adjust hero position to be on top of the grass blocks
            hero.Y = yPos - hero.H + 10;
        }

        void CreateTrees()
        {
            int yCurr = ClientSize.Height - 60 * 10 + 400;
            int treeCount = 3; // Number of trees to create
            Bitmap woodImage = Groups[1].Animations[0].imgs[2]; // Wood image from staticBlocks
            Bitmap treeGrassImage = Groups[1].Animations[0].imgs[3]; // Tree grass image from staticBlocks  
            List<int> existingTreePositions = new List<int>();

            for (int i = 0; i < treeCount; i++)
            {
                int x;
                do
                {
                    x = RR.Next(0, ClientSize.Width - 60);
                } while (CheckOverlap(existingTreePositions, x));

                existingTreePositions.Add(x);

                int y = yCurr - woodImage.Height + 100;

                makeRandomTrees(x, y, woodImage, treeGrassImage);

                
            }
        }

        void makeRandomTrees(int baseX, int baseY, Bitmap woodImage, Bitmap treeGrassImage)
        {
         List<Block> TreesBlocks;
        

        
            TreesBlocks = new List<Block>();
          
            blocks2D.Add(TreesBlocks); //->20
         
            // tree of height 5 blocks
            for (int i = 0; i < 5; i++)
            {
                Block woodBlock = new Block
                {
                    X = baseX,
                    Y = baseY - (i * 60),
                    W = 60,
                    H = 60,
                    Img = woodImage
                };
                blocks2D[20].Add(woodBlock);
            }


            int treeGrassTopY = baseY - (5 * 60) - 3 * 60;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Block treeGrassBlock = new Block
                    {
                        X = baseX - 60 + j * 60,
                        Y = treeGrassTopY + i * 60 + 60,
                        W = 60,
                        H = 60,
                        Img = treeGrassImage
                    };
                    blocks2D[20].Add(treeGrassBlock);
                }
            }
        
    }

        bool CheckOverlap(List<int> existingPositions, int newX)
        {
            for (int i = 0; i < existingPositions.Count; i++)
            {
                int position = existingPositions[i];


                if (newX >= position - 100 && newX <= position + 150)
                {
                    return true;
                }
            }
            return false;
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

            // Gravity and jump logic
            if (hero.isJumping)
            {
                hero.Y -= hero.jumpCt;
                hero.jumpCt -= 4;
                if (hero.jumpCt < 0)
                {
                    hero.isJumping = false;
                    hero.jumpCt = 20;
                }
            }
            else
            {
                if (!IsOnGround())
                {
                    hero.Y += 10; // gravity
                }
            }

            if (breaking != null)
            {
                if (ctTimer % 3 == 0 && breaking.iframe < 5)
                {
                    breaking.iframe++;
                }
                else if (breaking.iframe >= 5)
                {
                    blocks2D[breakingI].RemoveAt(breakingJ);
                    breaking = null;
                    isBreaking = 0;
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
            if (Sun.X + Sun.W <= 0)
            {
                Sun.Vars[1] = 0; //finish
                Sun.X = ClientSize.Width + 100;
                Sun.Y = 300;
          
            }

            ctTimer++;
            camera.Update(hero); // Update camera position based on hero's position
            DrawDouble(CreateGraphics());
        }

        private bool IsOnGround()
        {
            for (int i = 0; i < blocks2D.Count; i++)
            {
                List<Block> row = blocks2D[i];
                for (int j = 0; j < row.Count; j++)
                {
                    Block block = row[j];
                    if (hero.X < block.X + block.W - 40 &&
                        hero.X + hero.W - 40 > block.X &&
                        hero.Y + hero.H <= block.Y + block.H &&
                        hero.Y + hero.H + 10 >= block.Y) // Adjust 10 as per the gravity
                    {
                        hero.Y = block.Y - hero.H; // Adjust hero's position to stand on the block
                        return true;
                    }
                }
            }
            return false;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D:
                case Keys.Right:
                    hero.isHeroStable = 10;
                    hero.X += hero.speed;
                    hero.iframe++;
                    hero.imgs = Groups[0].Animations[2].imgs;
                    hero.dir = 1;
                    break;
                case Keys.A:
                case Keys.Left:
                    hero.X -= hero.speed;
                    hero.isHeroStable = 10;
                    hero.iframe++;
                    hero.imgs = Groups[0].Animations[3].imgs;
                    hero.dir = -1;
                    break;
                case Keys.W:
                case Keys.Up:               //Jumping
                case Keys.Space:
                    if (IsOnGround() && !hero.isJumping)
                    {
                        hero.isJumping = true;
                    }
                    break;
                case Keys.S:
                case Keys.Down:
                    // hero crouch logic if needed
                    break;
                case Keys.Z:      ///Zoom-In
                    Zoom(1);
                    break;
                case Keys.C:     //Zoom-Up
                    Zoom(2);
                    break;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            isBreaking = 0;
            breaking = null;
          
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    Rectangle viewRect = camera.GetViewRect();
                    int clickX = e.X + viewRect.X;
                    int clickY = e.Y + viewRect.Y;


                    for (int i = 0; i < blocks2D.Count; i++)
                    {
                        List<Block> row = blocks2D[i];
                        for (int j = 0; j < row.Count; j++)
                        {
                            Block block = row[j];
                            if (e.X + viewRect.X < block.X + block.W &&
                                e.X + viewRect.X > block.X &&
                                e.Y + viewRect.Y <= block.Y + block.H &&
                                e.Y + viewRect.Y >= block.Y) // Adjust 10 as per the gravity
                            {
                                Text = "works";
                                isBreaking = 1;
                                breakingI = i; //for removing the block
                                breakingJ = j; //for removing the block 
                                breaking = new AnimatedBlock();
                                breaking.X = block.X;
                                breaking.Y = block.Y;
                                breaking.W = block.W;
                                breaking.H = block.H;
                                breaking.imgs = Groups[1].Animations[2].imgs;
                                breaking.iframe = 0; // Start breaking animation from the first frame


                                // Handle item collection based on block type
                                if (block.Img == Groups[1].Animations[0].imgs[0]) // Grass block
                                {
                                    hero.Inventory.Add(new InventoryItem(1, 1));
                                }
                                else if (block.Img == Groups[1].Animations[0].imgs[4]) // Stone block
                                {
                                    hero.Inventory.Add(new InventoryItem(2, 1));
                                }
                                else if (block.Img == Groups[1].Animations[0].imgs[5]) // Coal block
                                {
                                    // Add stone block item to hero's inventory
                                    hero.Inventory.Add(new InventoryItem(3, 1));
                                }
                                break;

                            }
                        }
                    }


                 

                    break;
                case MouseButtons.Right:
                    // Right mouse button logic
                    break;
                case MouseButtons.XButton1:  //The bonus button 1 ملهمش فايدة بس بكتشف
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
            ex = e.X - 20;
            ey = e.Y - 30;
            if (isBreaking == 0)
            {
                Rectangle viewRect = camera.GetViewRect();
                for (int i = 0; i < blocks2D.Count; i++)
                {
                    List<Block> row = blocks2D[i];
                    for (int j = 0; j < row.Count; j++)
                    {
                        Block block = row[j];
                        if (e.X + viewRect.X < block.X + block.W &&
                            e.X + viewRect.X > block.X &&
                            e.Y + viewRect.Y <= block.Y + block.H &&
                            e.Y + viewRect.Y >= block.Y) // Adjust 10 as per the gravity
                        {
                            isBreaking = 0;
                            Text = "works";
                            breakingI = i; //for removing the block
                            breakingJ = j; //for removing the block 
                            breaking = new AnimatedBlock();
                            breaking.X = block.X;
                            breaking.Y = block.Y;
                            breaking.W = block.W;
                            breaking.H = block.H;
                            breaking.imgs = Groups[1].Animations[2].imgs;
                            breaking.iframe = 0; // Start breaking animation from the first frame
                            break;
                        }
                    }
                }
            }
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

            Rectangle viewRect = camera.GetViewRect();

            for (int i = 0; i < SingleActors.Count; i++)
            {
                BasicActor BasicActorTrav = SingleActors[i];
                g.DrawImage(BasicActorTrav.imgs[BasicActorTrav.iframe % BasicActorTrav.imgs.Count], BasicActorTrav.X , BasicActorTrav.Y , BasicActorTrav.W, BasicActorTrav.H);
            }

            g.DrawImage(hero.imgs[hero.iframe / 5 % hero.imgs.Count], hero.X - viewRect.X, hero.Y - viewRect.Y, hero.W, hero.H);

            for (int j = 0; j < blocks2D.Count; j++)
            {
                List<Block> rowBlocks = blocks2D[j];
                for (int i = 0; i < rowBlocks.Count; i++)
                {
                    Block block = rowBlocks[i];
                    g.DrawImage(block.Img, block.X - viewRect.X, block.Y - viewRect.Y, block.W, block.H);
                }
            }

            if (isBreaking == 1)
            {
                g.DrawImage(breaking.imgs[breaking.iframe % breaking.imgs.Count], breaking.X - viewRect.X, breaking.Y - viewRect.Y, breaking.W, breaking.H);
                g.DrawImage(BorderImg, breaking.X - viewRect.X, breaking.Y - viewRect.Y, breaking.W, breaking.H);
            }
            else if (breaking != null)
            {
                g.DrawImage(BorderImg, breaking.X - viewRect.X, breaking.Y - viewRect.Y, breaking.W, breaking.H);

            }
        }

        void ImagesReady() //this function to add the photos in the memory
        {
            Group pnn = new Group();  // [0]     1- hero Right 2- hero Left
            pnn.groupName = "Hero";
            Groups.Add(pnn);

            pnn = new Group();
            pnn.groupName = "Blocks";  // [1] 1-staticBlocks 2-BorderBoxes 3-BreakBlock
            Groups.Add(pnn);

            pnn = new Group();
            pnn.groupName = "Mobs";      
            Groups.Add(pnn);

            pnn = new Group();
            pnn.groupName = "Animals";
            Groups.Add(pnn);

            pnn = new Group();
            pnn.groupName = "Trees";
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
            for (int i = 6; i < 10 ; i++)
            {
                heroLeft.imgs.Add(new Bitmap("Images/SimpleSteve/" + i + ".png"));
            }
            Groups[0].Animations.Add(heroLeft);

            Animation staticBlocks = new Animation();
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/grass.png")); // Adding Grass image
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Dirt.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Oak.png")); // Adding Grass image
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/grassleaves.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/stone.png")); // Adding Stone image
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Coal.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Diamond.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Emerald.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Gold.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Ruby.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Sapphire.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Silver.png"));// Last Block image  (11)
            Groups[1].Animations.Add(staticBlocks);

            Animation blockBorders = new Animation();
            blockBorders.imgs.Add(new Bitmap("Images/Border.png"));
            Groups[1].Animations.Add(staticBlocks);

            Animation blockBreaking = new Animation();
            for (int i = 1; i < 6; i++)
            {
                blockBreaking.imgs.Add(new Bitmap("Images/breaking/breaking" + i + ".png"));
            }

            Groups[1].Animations.Add(blockBreaking);
        }

        void Zoom(int type)
        {
            breaking = null;
            isBreaking = 0;
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
                        block.Y += j * zoomRange; // Adjust Y position to account for zoom
                        block.X = (block.ID * block.W) - (zoomRange * block.ID) - zoom * 17 - zoom / 17; //hardcoded last 2
                    }
                }
            }
            else if (type == 2)
            {
                if (zoom > 0) 
                {
                    zoom -= zoomRange;
                    hero.W -= zoomRange;
                    hero.H -= zoomRange;
                    hero.Y += zoomRange;

                    for (int j = 0; j < blocks2D.Count; j++)
                    {
                        List<Block> rowBlocks = blocks2D[j];
                        for (int i = 0; i < rowBlocks.Count; i++)
                        {
                            Block block = rowBlocks[i];
                            block.W -= zoomRange;
                            block.H -= zoomRange;
                            block.Y -= j * zoomRange;
                            block.X = (block.ID * block.W) - (zoomRange * i) - zoom * 17 - zoom / 17;
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
