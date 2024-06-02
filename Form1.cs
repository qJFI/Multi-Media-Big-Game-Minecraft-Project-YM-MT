using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Multi_Media_Minecraft_Project_YM_MT
{
    /*
        public class OneImageBasicActor
        {
            public int X, Y, W, H;
            public Bitmap img;
            public List<int> Vars = new List<int>();
        }*/

    public class Effect
    {
        public int X, Y, W, H;
        public List<Bitmap> imgs = new List<Bitmap>();
        public int iframe = 0;
        public int stTime, endTime;
       
    }


    public class ladder
    {
        public int X, Y, W, H;
        public Bitmap img;
       
    }
    public class cAdvImg {

        public Rectangle rctSrc;
        public Rectangle rctDst;
        public Bitmap img;
    }

    public class Bullet
    {
        public int X, Y, W, H;
        public Bitmap img;
        public int tpye = 0;
        public int speed;
        public int dir;
    }
    public class BasicActor //actors that isn't responsive no Zoom or Cam !
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
        public int X, Y, W, H,Z=0;
        public Bitmap Img;
        public int ID; 
        public int ItemType;
    }

    public class AnimatedBlock
    {
        public int X, Y, W, H;
        public List<Bitmap> imgs = new List<Bitmap>();
        public int ID;
        public int iframe = 0;
    }


  

    public class InventoryItem
    {
        public int X, Y, W, H;
        public Bitmap Img;
        public int itemID;
        public int ItemType;
        public int quantity=0;

        public InventoryItem(int x, int y, int w, int h, Bitmap img,  int id)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
            Img = img;
            itemID = id;
            
        }
    }

    public class Inventory
    {
        public List<InventoryItem> items;

        public Inventory()
        {
            items = new List<InventoryItem>();
        }

        public void AddItem(InventoryItem item)
        {
            bool itemExists = false;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].itemID == item.itemID && items[i].quantity <64)
                {
                    itemExists = true;
                    items[i].quantity++;
                    break;
                }
            }

            if (!itemExists)
            {
                item.quantity =1;
                items.Add(item);
            }
        }

        public void RemoveItem(int itemID, int quantity)
        {
            // find item index
            int index = -1;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].itemID == itemID)
                {
                    index = i;
                    break;
                }
            }

            // if the item is in inventory decrease quantity
            if (index != -1)
            {
                items.RemoveAt(index);
            }
        }

      
        public void Clear()
        {
            items.Clear();
        }
    }

    public class Hero
    {
        public int X, Y, W, H;
        public int HeroHitBox = 200;
        public List<Bitmap> imgs;
        public int iframe = 0;
        public int isHeroStable = 0;
        public int dir = 1; //right -1 left
        public int speed = 20;
        public bool isJumping = false;
        public int jumpCt = 25;
        public int health = 100;
        
       
        public Inventory Inventory;

        public Hero()
        {
            Inventory = new Inventory();
        }
    }
    public class Enemy
    {
        public int X, Y, W, H;
        public List<Bitmap> imgs;
        public int iframe = 0;
        public int dir;
        public int speed = 10;
        public int moveCt = 30;
        public int type = 0; //0->zombie 1->Skeleton 2->Creeper
        public int health, fullHealth;
    }

    public class Camera
    {
        public int X, Y, Width, Height;
        public float ZoomFactor;

        public Camera(int width, int height)
        {
            Width = width;
            Height = height;
            X = 0;
            Y = 0;
            ZoomFactor = 1.0f;
        }

        public void Update(Hero hero)
        {
            X = (int)(hero.X - (Width / (2 * ZoomFactor)) - 200);
            Y = (int)(hero.Y - (Height / (2 * ZoomFactor)) - 300);

            // Ensure the camera doesn't go out of bounds
            if (X < 0) X = 0;
            if (Y < 0) Y = 0;
        }

        public Rectangle GetViewRect()
        {
            return new Rectangle(X, Y, (int)(Width / ZoomFactor), (int)(Height / ZoomFactor));
        }
    }

    public partial class Form1 : Form
    {
        Bitmap off;
        Bitmap BackImg = new Bitmap("Images/Back.png");
        Bitmap BlueBackImg = new Bitmap("Images/blue.png");
        Bitmap BackBiomeImg = new Bitmap("Images/biomeBackgrounds/birchForest.png");
        Bitmap BorderImg = new Bitmap("Images/Border.png");
        Bitmap SunImg = new Bitmap("Images/sun.png");
        List<Block> ladderBlocks;
        Bitmap HeroImg = new Bitmap("Images/hero1.png");
        Rectangle rctSrc, rctDst;
        Hero hero;
        Hero Zombie;
        AnimatedBlock breaking = null;
        Bitmap breakedImg = null;
        BasicActor Sun; //in single actor
        BasicActor HotBarItemsBorder = new BasicActor();
        BasicActor Inventory = new BasicActor();
        cAdvImg Health1 = new cAdvImg();
        BasicActor Health2 = new BasicActor();
        BasicActor Hunger = new BasicActor();
   

        int iframe = 0;
        int breakingI = -1, breakingJ = -1;
        int zoom = -10;
        int isBreaking = 0;
        int isLeftClick = 0;
        int zoomRange = 10;
        int healthValue = 100;
        int hungerValue = 10;
        int minuteCounter = 0;
        int yPos;
        int laser = -1;
        Point location1 = new Point(-1,-1);
        Point location2 = new Point(-1,-1);
        int stX = 0, stY = 0;
        Timer t = new Timer();
        int ctTimer = 0;
        int ex = -1;
        int ey = -1;
        List<List<Block>> blocks2D = new List<List<Block>>(); // 2D list for blocks :)
        List<InventoryItem> droppedItems = new List<InventoryItem>();
        List<BasicActor> SingleActors = new List<BasicActor>();
        List<Group> Groups = new List<Group>();
        List<Enemy> Enemies = new List<Enemy>();
        List<Bullet> Bullets = new List<Bullet>();
        List<Effect> Effects = new List<Effect>();
        //List<InventoryItem> heroTools = new List<InventoryItem>();
        bool isBroken = false;

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
            rctSrc = new Rectangle(stX, stY, BackImg.Width / 10, BackImg.Height);
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

            // Hero
            /*
            Zombie = new Hero();
            Zombie.W = HeroImg.Width / 2;
            Zombie.H = HeroImg.Height / 3;
            Zombie.X = ClientSize.Width / 2;
            Zombie.Y = ClientSize.Height - Zombie.H - 50;
            Zombie.imgs = new List<Bitmap>(Groups[0].Animations[0].imgs);
            */
            //create the hotbar borders

            HotBarItemsBorder.X = ClientSize.Width / 2 - 450;
            HotBarItemsBorder.Y = ClientSize.Height - 100;
            HotBarItemsBorder.W = 100;
            HotBarItemsBorder.H = 100;
            HotBarItemsBorder.Vars.Add(0);
            HotBarItemsBorder.imgs.Add(new Bitmap("Images/hotbar/big.png"));
            for (int i = 0; i < 8; i++)
            {
                HotBarItemsBorder.imgs.Add(new Bitmap("Images/hotbar/small.png"));
            }

            //create inventory
            Inventory.W = 934;
            Inventory.H = 867;
            Inventory.X = ClientSize.Width / 2 - Inventory.W/2;
            Inventory.Y = ClientSize.Height /2-500;
           
            Inventory.Vars.Add(-1); //is inventory active ?
            Inventory.Vars.Add(0); //which is hovered will be used later
            Inventory.imgs.Add(new Bitmap("Images/inventory/inventory.png"));



            Bitmap HealthImg = new Bitmap("Images/hotbar/health1.png");
            Health1.rctDst.X = ClientSize.Width / 2 - 455;
            Health1.rctDst.Y = ClientSize.Height - 150;
            Health1.rctDst.Width = 300;
            Health1.rctDst.Height = 37;
            Health1.img = HealthImg;
            Health1.rctSrc=new Rectangle(0,0, HealthImg.Width, HealthImg.Height);


            Health2.X = ClientSize.Width / 2 - 470;
            Health2.Y = ClientSize.Height - 150;
            Health2.W = 300;
            Health2.H = 37;
            Health2.Vars.Add(0);
            Health2.imgs.Add(new Bitmap("Images/hotbar/health2.png"));

            

            Hunger.X = ClientSize.Width / 2 + 150;
            Hunger.Y = ClientSize.Height - 150;
            Hunger.W = 300;
            Hunger.H = 40;
            Hunger.Vars.Add(0);
            Hunger.imgs.Add(new Bitmap("Images/hotbar/hunger0.png"));

            //Gun
            Bitmap gunImage = new Bitmap("Images/heroTools/gun.png");
       
            InventoryItem gun = new InventoryItem(HotBarItemsBorder.X + 30, HotBarItemsBorder.Y,60,60,gunImage,20);
            hero.Inventory.AddItem (gun);
       

            // Create random biome blocks
            CreateRandomBiomeBlocks();

            // Tree
            CreateTrees();

            /* //Zombies
             CreateZombie();*/
           

            CreateUpper();
        }

        void UpdateHealth(int newValue)  // we will use this function when we add fall damage(fel a8lb msh hy7sl xD) or zombies
        {
            healthValue = newValue;
        }

       
        void CreateRandomBiomeBlocks()
        {
            int blockWidth = 60; // Set your block width
            int blockHeight = 60; // Set your block height
            int columns = ClientSize.Width / blockWidth;
            int rows = 20; // Number of rows of blocks
            yPos = ClientSize.Height - blockHeight * rows + 1000; // Starting Y position of the bottom row


            makeladder(960, yPos, Groups[1].Animations[1].imgs[0]);

            for (int i = 0; i < rows; i++)
            {
                List<Block> rowBlocks = new List<Block>();
                for (int j = 0; j < columns +30; j++)
                {
                    Block blockPnn = new Block();
                    blockPnn.X = j * blockWidth;
                    blockPnn.Y = yPos + (i * blockHeight);
                    blockPnn.W = blockWidth;
                    blockPnn.H = blockHeight;
                   
                    if (i < 1)
                    {
                        blockPnn.Img = Groups[1].Animations[0].imgs[0]; // Always grass for the first 5 rows from the bottom
                        blockPnn.ID = 0;
                    }
                    else if (i < 2)
                    {
                        blockPnn.Img = Groups[1].Animations[0].imgs[1];
                        blockPnn.ID = 1;
                    }
                    else
                    {
                        int isStone = RR.Next(0, 3);

                        if (isStone == 0)
                        {
                            
                            int randomBlock = RR.Next(4, 12);
                            blockPnn.ID = randomBlock;
                            blockPnn.Img = Groups[1].Animations[0].imgs[randomBlock];
                        }
                        else
                        {
                            blockPnn.ID = 4;
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


        void CreateUpper()
        {
            int blockWidth = 60; // Set your block width
            int blockHeight = 60; // Set your block height
            int columns = ClientSize.Width / blockWidth;
            //int rows = 2; // Number of rows of blocks
            int UpperYPos = ClientSize.Height - blockHeight  + 1000 - 30*60; 

           
                List<Block> rowBlocks = new List<Block>();
                for (int j = 0; j < columns + 30; j++)
                {
                    Block blockPnn = new Block();
                    blockPnn.X = j * blockWidth;
                    blockPnn.Y = UpperYPos + (  blockHeight);
                    blockPnn.W = blockWidth;
                    blockPnn.H = blockHeight;

                   
                        blockPnn.Img = Groups[1].Animations[0].imgs[0]; // Always grass for the first 5 rows from the bottom
                        blockPnn.ID = 0;
                    

                    rowBlocks.Add(blockPnn);
                }
                blocks2D.Add(rowBlocks);
            

        }




        void CreateZombie()
        {
            int zombieCount = 3; // Number of trees to create
            int blockWidth = 60; // Ensure this matches your block width
            
            Bitmap woodImage = Groups[1].Animations[0].imgs[2]; // Wood image from staticBlock
            List<int> existingTreePositions = new List<int>();
            Rectangle viewRect = camera.GetViewRect();
            for (int i = 0; i < zombieCount; i++)
            {
                int x;
                do
                {
                    x = RR.Next(0, ClientSize.Width - 60);
                    x = (int)((x / camera.ZoomFactor) + viewRect.X);


                    // Determine the column and row based on the mouse click position
                    int column = x / blockWidth;

                    x = column * blockWidth;


                } while (CheckOverlap(existingTreePositions, x));

                existingTreePositions.Add(x);

          

                int imgdir = RR.Next(0, 2);
                int Dir = 1;
                if(imgdir==1)
                {
                    Dir = -1;
                }
                Enemy Zombie = new Enemy
                {
                    X = x,
                    Y = yPos - hero.H,
                    W = hero.W,
                    H = hero.H,
                   

                    dir = Dir,
                    imgs = Groups[2].Animations[imgdir].imgs,
                    health = 200,
                    fullHealth = 200,


                };
                Enemies.Add(Zombie);
            }
        }


        void CreateTrees()
        {
            int yCurr = ClientSize.Height - 60 * 10 + 400;
            int treeCount = 3; // Number of trees to create
            int blockWidth = 60; // Ensure this matches your block width
           
            Bitmap woodImage = Groups[1].Animations[0].imgs[2]; // Wood image from staticBlocks
            Bitmap treeGrassImage = Groups[1].Animations[0].imgs[3]; // Tree grass image from staticBlocks  
            List<int> existingTreePositions = new List<int>();
            Rectangle viewRect = camera.GetViewRect();
            for (int i = 0; i < treeCount; i++)
            {
                int x;
                do
                {
                    x = RR.Next(0, ClientSize.Width - 60);
                    x = (int)((x / camera.ZoomFactor) + viewRect.X);


                    // Determine the column and row based on the mouse click position
                    int column = x / blockWidth;

                    x = column * blockWidth;

                   
                } while (CheckOverlap(existingTreePositions, x));

                existingTreePositions.Add(x);

                int y = yCurr - woodImage.Height + 100;

                makeRandomTrees(x, y, woodImage, treeGrassImage);
            }
        }

        void makeladder(int baseX, int baseY, Bitmap ladderImage)
        {
           ladderBlocks = new List<Block>();
            blocks2D.Add(ladderBlocks); //->20
          
            // tree of height 5 blocks
            for (int i = 1; i < 11; i++)
            {
                Block woodBlock = new Block
                {
                    X = baseX,
                    Y = baseY - (i * 60),
                    W = 60,
                    H = 60,
                    Img = ladderImage,
                    ID = 14,
                    Z = 1
                };
                ladderBlocks.Add(woodBlock);
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
                    Img = woodImage,
                    ID = 2,
                    Z = 1
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
                        Img = treeGrassImage,
                        ID = 3,
                        Z = 1
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

        bool IsDroppedOnGround(InventoryItem droppedItemsTrav)
        {
            for (int i = 0; i < blocks2D.Count; i++)
            {
                List<Block> row = blocks2D[i];
                for (int j = 0; j < row.Count; j++)
                {
                    Block block = row[j];

                    if (droppedItemsTrav.X < block.X + block.W &&
                        droppedItemsTrav.X + droppedItemsTrav.W > block.X &&
                        droppedItemsTrav.Y + droppedItemsTrav.H <= block.Y + block.H + 4 &&
                        droppedItemsTrav.Y + droppedItemsTrav.H >= block.Y) // Adjust 10 as per the gravity
                    {
                        droppedItemsTrav.Y = block.Y - droppedItemsTrav.H; // Adjust hero's position to stand on the block
                        return true;
                    }
                }
            }
            return false;
        }
        private void UpdateHungerStatus()
        {
            int totalFrames = 11;
            int elapsedMinutes = minuteCounter / 60;
            hungerValue = elapsedMinutes % totalFrames;

            Hunger.imgs[0] = new Bitmap("Images/hotbar/hunger" + hungerValue + ".png");
        }

        private void T_Tick(object sender, EventArgs e)
        {
            if (hero.health <= 0)
                return;

            for (int i = 0; i < Effects.Count; i++)
            {
                Effect EffectsTrav = Effects[i];
                EffectsTrav.iframe++;
                if (ctTimer > EffectsTrav.endTime)
                    Effects.RemoveAt(i);
            }

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

                //bullets move 

                for (int i = 0; i < Bullets.Count; i++)
                {
                    Bullet BulletsTrav = Bullets[i];
                    BulletsTrav.X += BulletsTrav.dir * BulletsTrav.speed;

                    // Check if bullet hits any enemy
                    for (int j = 0; j < Enemies.Count; j++)
                    {
                        Enemy enemy = Enemies[j];
                        if (BulletsTrav.X < enemy.X + enemy.W &&
                            BulletsTrav.X + BulletsTrav.W > enemy.X &&
                            BulletsTrav.Y < enemy.Y + enemy.H &&
                            BulletsTrav.Y + BulletsTrav.H > enemy.Y)
                        {
                            // Bullet hits the enemy
                            Text = "" + BulletsTrav.X + "  " + enemy.X;
                            enemy.health -= 30;
                            Effect pnnEffect = new Effect();
                            pnnEffect.X = enemy.X;
                            pnnEffect.Y = enemy.Y;
                            pnnEffect.W = 100;
                            pnnEffect.H = 100;
                            pnnEffect.imgs = Groups[3].Animations[1].imgs; // Blood
                            pnnEffect.stTime = ctTimer;
                            pnnEffect.endTime = ctTimer + 7;
                            Effects.Add(pnnEffect);
                            if (enemy.health <= 0)
                            {
                                Enemies.RemoveAt(j);
                            }
                            // Remove the bullet after it hits the enemy
                            Bullets.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                }

             
                if (laser > 0)
                {
                   
                        
                        laser--;
                    

                }




            }

            if((ctTimer)%100==0)
            {
                CreateZombie();
            }

            if(ctTimer%5==0)
            {
                //enemie move
                for (int i = 0; i < Enemies.Count; i++)
                {
                    Enemy EnemyTrav = Enemies[i];
                    EnemyTrav.X += EnemyTrav.dir * EnemyTrav.speed;
                    EnemyTrav.iframe++;
                    if (hero.X < EnemyTrav.X + EnemyTrav.W &&
                        hero.X + hero.W > EnemyTrav.X &&
                        hero.Y < EnemyTrav.Y + EnemyTrav.H &&
                        hero.Y + hero.H > EnemyTrav.Y)
                    {
                        EnemyTrav.dir = 0;
                        // Enemy touches the hero
                        hero.health -= 4;
                        Effect pnnEffect = new Effect();
                        pnnEffect.X = hero.X;
                        pnnEffect.Y = hero.Y;
                        pnnEffect.W = 100;
                        pnnEffect.H = 100;
                        pnnEffect.imgs = Groups[3].Animations[0].imgs; // Blood
                        pnnEffect.stTime = ctTimer;
                        pnnEffect.endTime = ctTimer + 7;
                        Effects.Add(pnnEffect);
                        Health1.rctSrc.Width -= 11;
                        Health1.rctDst.Width -= 12;
                    }
                    else if (EnemyTrav.dir == 0)
                    {
                        EnemyTrav.dir = 1;
                        EnemyTrav.imgs = Groups[2].Animations[0].imgs;
                    }
                }
            }

            for (int i = 0; i < droppedItems.Count; i++)
            {
                InventoryItem droppedItemsTrav = droppedItems[i];
                if (!IsDroppedOnGround(droppedItemsTrav))
                {
                    droppedItemsTrav.Y += 5;
                }

                if (droppedItemsTrav.X < hero.X + hero.W &&
                    droppedItemsTrav.X + droppedItemsTrav.W > hero.X &&
                    droppedItemsTrav.Y + droppedItemsTrav.H <= hero.Y + hero.H &&
                    droppedItemsTrav.Y + droppedItemsTrav.H >= hero.Y) // Adjust 10 as per the gravity
                {
                    hero.Inventory.AddItem(droppedItemsTrav);
                    droppedItems.RemoveAt(i);
                }
            }

            // Gravity and jump logic
            if (hero.isJumping)
            {
                hero.Y -= hero.jumpCt;
                hero.jumpCt -= 6;
                if (hero.jumpCt < 0)
                {
                    hero.isJumping = false;
                    hero.jumpCt = 25;
                }
            }
            else
            {
                if (!IsOnGround())
                {
                    hero.Y += 15; // gravity
                }
            }

            if (breaking != null && isBreaking == 1)
            {
                if (ctTimer % 3 == 0 && breaking.iframe < 5)
                {
                    breaking.iframe++;
                }
                else if (breaking.iframe >= 5)
                {
                    // Create a dropped item
                    InventoryItem droppedItem = new InventoryItem(breaking.X + 15, breaking.Y + 30, 22, 22, breakedImg, breaking.ID);
                    droppedItems.Add(droppedItem);
                    isBroken = false;

                    blocks2D[breakingI].RemoveAt(breakingJ);
                    breaking = null;
                    isBreaking = 0;
                }
            }

            minuteCounter++;

            
            if (minuteCounter % 60 == 0)
            {
                UpdateHungerStatus();
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
                        hero.Y + hero.H <= block.Y + block.H  &&
                        hero.Y + hero.H + 10 >= block.Y) // Adjust 10 as per the gravity
                    {
                        hero.Y = block.Y - hero.H+10; // Adjust hero's position to stand on the block
                        return true;
                    }
                }
            }
            return false;
        }
        private bool IsBlocked(int newX, int newY, int heroWidth, int heroHeight)
        {
            for (int i = 0; i < blocks2D.Count; i++)
            {
                List<Block> rowBlocks = blocks2D[i];
                for (int j = 0; j < rowBlocks.Count; j++)
                {
                    Block block = rowBlocks[j];
                   
                    if (newX+50 < block.X + block.W &&
                        newX + heroWidth-50 > block.X &&
                        newY +50 < block.Y + block.H &&
                        newY + heroHeight-50 > block.Y && 
                        block.Z == 0)
                    {
                        return true;
                    }
                }
               
            }
         
            return false;
        }


        bool IsOnLadder()
        {
            for(int i=0;i<ladderBlocks.Count;i++)
            {
                Block block = ladderBlocks[i];
                if (hero.X < block.X + block.W  &&
            hero.X +hero.W  > block.X &&
            hero.Y <= block.Y + block.H &&
            hero.Y  +hero.H >= block.Y ) // Adjust 10 as per the gravity
                {
                    return true;
                }
            }
          return false;
        }
            
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int newHeroX;
            switch (e.KeyCode)
            {
                case Keys.D:
                case Keys.Right:
                    newHeroX = hero.X + hero.speed;
                    if (!IsBlocked(newHeroX, hero.Y, hero.W, hero.H))
                    {
                        hero.isHeroStable = 10;
                        hero.X += hero.speed;
                        hero.iframe++;
                        hero.imgs = Groups[0].Animations[2].imgs;
                        hero.dir = 1;
                    }
                    break;
                case Keys.A:
                case Keys.Left:
                     newHeroX = hero.X - hero.speed;
                    if (!IsBlocked(newHeroX, hero.Y, hero.W, hero.H))
                    {
                        hero.X -= hero.speed;
                        hero.isHeroStable = 10;
                        hero.iframe++;
                        hero.imgs = Groups[0].Animations[3].imgs;
                        hero.dir = -1;
                    }
                   
                    break;
                //case Keys.W:
                case Keys.Up:               //Jumping
                case Keys.Space:
                    if (IsOnGround() && !hero.isJumping)
                    {
                        hero.isJumping = true;
                    }
                    

                    break;
                case Keys.W:
                    if (IsOnLadder())
                    {
                        hero.Y -= 50;
                    }
                    break;
                case Keys.S:
                case Keys.Down:
                    if (IsOnLadder())
                    {
                        hero.Y += 50;
                    }
                    // hero crouch logic if needed
                    break;
                case Keys.Z:      ///Zoom-In
                    Zoom(1);
                    break;
                case Keys.C:     //Zoom-Up
                    Zoom(2);
                    break;

                    //Hotbar ->
                case Keys.D1:
                    HotBarItemsBorder.Vars[0] = 0;
                    break;
                case Keys.D2:
                    HotBarItemsBorder.Vars[0] = 1;
                    break;
                case Keys.D3:
                    HotBarItemsBorder.Vars[0] = 2;
                    break;
                case Keys.D4:
                    HotBarItemsBorder.Vars[0] = 3;
                    break;
                case Keys.D5:
                    HotBarItemsBorder.Vars[0] = 4;
                    break;
                case Keys.D6:
                    HotBarItemsBorder.Vars[0] = 5;
                    break;
                case Keys.D7:
                    HotBarItemsBorder.Vars[0] = 6;
                    break;
                case Keys.D8:
                    HotBarItemsBorder.Vars[0] = 7;
                    break;
                case Keys.D9:
                    HotBarItemsBorder.Vars[0] = 8;
                    break;
                case Keys.D0:
                    HotBarItemsBorder.Vars[0] = 9;
                    break;
                case Keys.E:
                    Inventory.Vars[0] *= -1;
                    break;
                case Keys.F:
                    if( HotBarItemsBorder.Vars[0] == 0)
                    {
                       Bullet bulletPnn = new Bullet();
                        bulletPnn.X = hero.X + hero.W -50;
                        bulletPnn.Y = hero.Y+30 ;
                        bulletPnn.W = 40;
                        bulletPnn.H = 40;
                        bulletPnn.tpye = 0; //normal bullet  1-> double 2-> for arrow if made 
                        if(hero.dir ==1 )
                        {
                            bulletPnn.dir= 1;
                            bulletPnn.img = new Bitmap("Images/bullet1.png");
                        }
                        else
                        {
                            bulletPnn.dir = -1;
                            bulletPnn.X -= hero.W - 60;
                            bulletPnn.img = new Bitmap("Images/bullet2.png");
                        }
                        bulletPnn.speed = 20;
                        
                        Bullets.Add (bulletPnn);
                    }
                    break;
                case Keys.G:
                    if (HotBarItemsBorder.Vars[0] == 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Bullet bulletPnn = new Bullet();
                            bulletPnn.X = hero.X + hero.W - 50 + i*40;
                            bulletPnn.Y = hero.Y + 30;
                            bulletPnn.W = 40;
                            bulletPnn.H = 40;
                            bulletPnn.tpye = 0; //normal bullet  1-> double 2-> for arrow if made 
                            if (hero.dir == 1)
                            {
                                bulletPnn.dir = 1;
                                bulletPnn.img = new Bitmap("Images/bullet1.png");
                            }
                            else
                            {
                                bulletPnn.dir = -1;
                                bulletPnn.X -= hero.W - 20;
                                bulletPnn.img = new Bitmap("Images/bullet2.png");
                            }
                            bulletPnn.speed = 20;

                            Bullets.Add(bulletPnn);
                        }
                    }

                    break;
                case Keys.L:
                    location1.X = (hero.X + hero.W);
                    laser=5;
                    if (ex<hero.X-camera.GetViewRect().X)
                    {
                        location1.X -= hero.W - 20; 
                    }
                    location1.Y = hero.Y + 30;
                    location2.X = ex;
                    location2.Y = hero.Y + 30;
                    // Calculate the hitbox for the laser
                    int a  = (hero.X + hero.W);
                    int b = camera.GetViewRect().X + ex;
                    
                    int laserStartX, laserEndX, laserStartY,laserEndY;
                    if (a<b)
                    {
                         laserStartX = a;
                        laserEndX = b;
                      
                    }
                    else
                    {

                        laserStartX = b;
                        laserEndX = a;
                     


                    }
                    int laserY = location1.Y;
                   
                   
                   
                    for (int i = 0; i < Enemies.Count; i++)
                    {
                        Enemy enemy = Enemies[i];

                        
                        // Calculate the hitbox for the enemy
                 
                       
                        
                        // Check if the laser intersects with the enemy's hitbox
                        if ((enemy.X >= laserStartX && enemy.X + enemy.W <= laserEndX) &&
                               (laserY >= enemy.Y && laserY <= enemy.Y + enemy.H )

                         )
                        {
                            enemy.health -= 50; // Laser does more damage
                            if (enemy.health <= 0)
                            {
                                Enemies.RemoveAt(i);
                                i--;
                            }
                            Effect pnnEffect = new Effect();
                            pnnEffect.X = enemy.X;
                            pnnEffect.Y = enemy.Y;
                            pnnEffect.W = 100;
                            pnnEffect.H = 100;
                            pnnEffect.imgs = Groups[3].Animations[1].imgs; // Blood
                            pnnEffect.stTime = ctTimer;
                            pnnEffect.endTime = ctTimer + 7;
                            Effects.Add(pnnEffect);
                        }
                   
                    }
                    break;

            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            isBreaking = 0;
            breaking = null;
        }
        private void PlaceBlock(int x, int y, int blockID)
        {
            if(blockID>=20)
            { return; }
            int blockWidth = 60; // Ensure this matches your block width
            int blockHeight = 60; // Ensure this matches your block height

            // Determine the column and row based on the mouse click position
            int column = x / blockWidth;
            int row = y / blockHeight;

            // Create the new block
            Block newBlock = new Block
            {
                X = column * blockWidth ,
                Y = row * blockHeight+17,
                W = blockWidth,
                H = blockHeight,
                ID = blockID,
                Img = Groups[1].Animations[0].imgs[blockID] // Assuming blockID corresponds to the index in the images list
            };

            bool blockExists = false;
            for (int i = 0; i < blocks2D.Count; i++)
            {
                List<Block> rowBlocks = blocks2D[i];
                for (int j = 0; j < rowBlocks.Count; j++)
                {
                    Block block = rowBlocks[j];
                    if (block.X == newBlock.X && block.Y == newBlock.Y)
                    {
                        // There's already a block in this position, so we can't place a new one
                        blockExists = true;
                        break;
                    }
                }
                if (blockExists)
                    break;
            }

            if (blockExists)
                return;

            // Add the new block to the blocks2D list
            int itemIndex = HotBarItemsBorder.Vars[0];
           
            if (hero.Inventory.items[itemIndex].quantity> 0){

            
                if (row < blocks2D.Count)
                {
                    blocks2D[row].Add(newBlock);
                }
                else
                {
                    List<Block> newRow = new List<Block> { newBlock };
                    blocks2D.Add(newRow);
                }
                hero.Inventory.items[itemIndex].quantity--;
                if(hero.Inventory.items[itemIndex].quantity==0)
                {
                    hero.Inventory.items.RemoveAt(itemIndex);
                }
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    Rectangle viewRect = camera.GetViewRect();
                    int clickX = (int)((e.X / camera.ZoomFactor) + viewRect.X);
                    int clickY = (int)((e.Y / camera.ZoomFactor) + viewRect.Y);

                    for (int i = 0; i < blocks2D.Count; i++)
                    {
                        List<Block> row = blocks2D[i];
                        for (int j = 0; j < row.Count; j++)
                        {
                            Block block = row[j];

                            //check Range ->
                            if (block.X < hero.X + hero.HeroHitBox &&
                                block.X >= hero.X - hero.HeroHitBox &&
                                block.Y < hero.Y + hero.HeroHitBox &&
                                 block.Y >= hero.Y - hero.HeroHitBox
                                )
                            {

                                //check click ->
                                if (clickX < block.X + block.W &&
                                    clickX > block.X &&
                                    clickY <= block.Y + block.H &&
                                    clickY >= block.Y)
                                {
                                    
                                    isBreaking = 1;
                                    breakedImg = block.Img;
                                    breakingI = i; //for removing the block
                                    breakingJ = j; //for removing the block 
                                    breaking = new AnimatedBlock();
                                    breaking.X = block.X;
                                    breaking.Y = block.Y;
                                    breaking.W = block.W;
                                    breaking.H = block.H;
                                    breaking.ID = block.ID;
                                    breaking.imgs = Groups[1].Animations[2].imgs;
                                    breaking.iframe = 0; // Start breaking animation from the first frame
                                    break;
                                }
                            }
                        }
                    }
                    break;

                case MouseButtons.Right:
                    Rectangle viewRect1 = camera.GetViewRect();
                    int clickX1 = (int)((e.X / camera.ZoomFactor) + viewRect1.X);
                    int clickY1 = (int)((e.Y / camera.ZoomFactor) + viewRect1.Y);

                    // Get the blockID from the hotbar selection
                    int hotbarIndex = HotBarItemsBorder.Vars[0];
                    if (hotbarIndex < hero.Inventory.items.Count)
                    {
                        int blockID = hero.Inventory.items[hotbarIndex].itemID;
                        PlaceBlock(clickX1, clickY1, blockID);
                    }
                    
                    break;
                    // Right mouse button logic (Build logic)
                case MouseButtons.XButton1:  //The bonus button 1
                    HotBarItemsBorder.Vars[0] = 2;
                    break;
                case MouseButtons.XButton2:  //The bonus button 2
                    HotBarItemsBorder.Vars[0] = 4;
                    break;
                case MouseButtons.Middle:  //The bonus button 2
                    HotBarItemsBorder.Vars[0] = 0;
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
                        if ((e.X / camera.ZoomFactor) + viewRect.X < block.X + block.W &&
                            (e.X / camera.ZoomFactor) + viewRect.X > block.X &&
                            (e.Y / camera.ZoomFactor) + viewRect.Y <= block.Y + block.H &&
                            (e.Y / camera.ZoomFactor) + viewRect.Y >= block.Y)
                        {
                            isBreaking = 0;
                           
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
            g.DrawImage(BlueBackImg, 0, 0, ClientSize.Width, ClientSize.Height);

            g.DrawImage(BackBiomeImg, 0, 0, ClientSize.Width, ClientSize.Height);
            g.DrawImage(BackImg, rctDst, rctSrc, GraphicsUnit.Pixel);
            Rectangle viewRect = camera.GetViewRect();

            for (int i = 0; i < SingleActors.Count; i++)
            {
                BasicActor BasicActorTrav = SingleActors[i];
                g.DrawImage(BasicActorTrav.imgs[BasicActorTrav.iframe % BasicActorTrav.imgs.Count],
                            (BasicActorTrav.X - viewRect.X) * camera.ZoomFactor,
                            (BasicActorTrav.Y - viewRect.Y) * camera.ZoomFactor,
                            BasicActorTrav.W * camera.ZoomFactor,
                            BasicActorTrav.H * camera.ZoomFactor);
            }

            g.DrawImage(hero.imgs[hero.iframe / 5 % hero.imgs.Count],
                        (hero.X - viewRect.X) * camera.ZoomFactor,
                        (hero.Y - viewRect.Y) * camera.ZoomFactor,
                        hero.W * camera.ZoomFactor,
                        hero.H * camera.ZoomFactor);

            for (int j = 0; j < blocks2D.Count; j++)
            {
                List<Block> rowBlocks = blocks2D[j];
                for (int i = 0; i < rowBlocks.Count; i++)
                {
                    Block block = rowBlocks[i];
                    g.DrawImage(block.Img,
                                (block.X - viewRect.X) * camera.ZoomFactor,
                                (block.Y - viewRect.Y) * camera.ZoomFactor,
                                block.W * camera.ZoomFactor,
                                block.H * camera.ZoomFactor);
                }
            }

            if (isBreaking == 1)
            {
                g.DrawImage(breaking.imgs[breaking.iframe % breaking.imgs.Count],
                            (breaking.X - viewRect.X) * camera.ZoomFactor,
                            (breaking.Y - viewRect.Y) * camera.ZoomFactor,
                            breaking.W * camera.ZoomFactor,
                            breaking.H * camera.ZoomFactor);
                g.DrawImage(BorderImg,
                            (breaking.X - viewRect.X) * camera.ZoomFactor,
                            (breaking.Y - viewRect.Y) * camera.ZoomFactor,
                            breaking.W * camera.ZoomFactor,
                            breaking.H * camera.ZoomFactor);
            }
            else if (breaking != null)
            {
                g.DrawImage(BorderImg,
                            (breaking.X - viewRect.X) * camera.ZoomFactor,
                            (breaking.Y - viewRect.Y) * camera.ZoomFactor,
                            breaking.W * camera.ZoomFactor,
                            breaking.H * camera.ZoomFactor);
            }

            // Draw dropped items using a for loop without var



            for (int i = 0; i < Enemies.Count; i++)
            {
                Enemy enemyTrav = Enemies[i];
                g.DrawImage(enemyTrav.imgs[enemyTrav.iframe% enemyTrav.imgs.Count], (enemyTrav.X - viewRect.X) * camera.ZoomFactor, (enemyTrav.Y - viewRect.Y) * camera.ZoomFactor, enemyTrav.W * camera.ZoomFactor, enemyTrav.H * camera.ZoomFactor);

            }


            //bullets

            for(int i=0; i < Bullets.Count;i++)
            {
                Bullet BulletsTrav = Bullets[i];
                g.DrawImage(BulletsTrav.img,
                            (BulletsTrav.X - viewRect.X) * camera.ZoomFactor,
                            (BulletsTrav.Y - viewRect.Y) * camera.ZoomFactor,
                            BulletsTrav.W * camera.ZoomFactor,
                            BulletsTrav.H * camera.ZoomFactor);
            }

            for (int i = 0; i < Effects.Count; i++)
            {
                Effect EffectsTrav = Effects[i];
                g.DrawImage(EffectsTrav.imgs[EffectsTrav.iframe% EffectsTrav.imgs.Count],
                            (EffectsTrav.X - viewRect.X) * camera.ZoomFactor,
                            (EffectsTrav.Y - viewRect.Y) * camera.ZoomFactor,
                            EffectsTrav.W * camera.ZoomFactor,
                            EffectsTrav.H * camera.ZoomFactor);
            }
            if (laser >0)
            {
                Pen P = new Pen(Color.Red, 5);
                g.DrawLine(P, (location1.X - viewRect.X) * camera.ZoomFactor, (location1.Y - viewRect.Y) * camera.ZoomFactor, location2.X, location2.Y);
            }

            //dont draw thing after this block just before
            for (int i = 0; i < droppedItems.Count; i++)
            {
                InventoryItem droppedItem = droppedItems[i];
                g.DrawImage(droppedItem.Img, (droppedItem.X - viewRect.X) * camera.ZoomFactor, (droppedItem.Y - viewRect.Y) * camera.ZoomFactor, droppedItem.W, droppedItem.H);
            }
            for (int i = 0; i < HotBarItemsBorder.imgs.Count; i++)
            {
                if (HotBarItemsBorder.Vars[0] != i)
                    g.DrawImage(HotBarItemsBorder.imgs[1], HotBarItemsBorder.X + HotBarItemsBorder.W * i + 15, HotBarItemsBorder.Y, HotBarItemsBorder.W, HotBarItemsBorder.H);
                else
                    g.DrawImage(HotBarItemsBorder.imgs[0], HotBarItemsBorder.X + HotBarItemsBorder.W * i, HotBarItemsBorder.Y - 20, HotBarItemsBorder.W + 20, HotBarItemsBorder.H + 20);
            }
            
            g.DrawImage(Health2.imgs[0], Health2.X+ 15, Health2.Y, Health2.W, Health2.H);
            g.DrawImage(Health1.img, Health1.rctDst, Health1.rctSrc, GraphicsUnit.Pixel);
            
            g.DrawImage(Hunger.imgs[0], Hunger.X+ 15, Hunger.Y, Hunger.W, Hunger.H);
            

            for (int i = 0; i < hero.Inventory.items.Count && i < 9; i++)
            {
                g.DrawImage(hero.Inventory.items[i].Img, HotBarItemsBorder.X + 30 + HotBarItemsBorder.W * i, HotBarItemsBorder.Y + 15, 60, 60);
                g.DrawString(hero.Inventory.items[i].quantity.ToString(), new Font("Arial", 12, FontStyle.Bold), Brushes.White, HotBarItemsBorder.X + 30 + HotBarItemsBorder.W * i, HotBarItemsBorder.Y + 15);
            }

            if (Inventory.Vars[0]==1)
            {
                g.DrawImage(Inventory.imgs[0], Inventory.X , Inventory.Y, Inventory.W, Inventory.H);
                for (int i = 0; i < hero.Inventory.items.Count; i++)
                {
                    int cX= Inventory.X + 55 + (Inventory.W / 10) * (i%9) , cY = Inventory.Y + Inventory.H - (Inventory.H / 8) * (i / 9) - 115, cW= 60, cH= 60;
                    g.DrawImage(hero.Inventory.items[i].Img, cX, cY, cW, cH);
                    g.DrawString(hero.Inventory.items[i].quantity.ToString(), new Font("Arial", 12, FontStyle.Bold), Brushes.White, cX, cY);
                }
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
            pnn.groupName = "Monsters";// [2] 1-ZombieRight 2-ZombieRight 3-Skeleton...
            Groups.Add(pnn);

            pnn = new Group();
            pnn.groupName = "Effects";// [3] 1-blood 2-...
            Groups.Add(pnn);

            pnn = new Group();
            pnn.groupName = "Animals";
            Groups.Add(pnn);



            //hero
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



            //blocks
            Animation staticBlocks = new Animation();
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/grass.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Dirt.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Oak.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/grassleaves.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/stone.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Coal.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Diamond.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Emerald.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Gold.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Ruby.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Sapphire.png"));
            staticBlocks.imgs.Add(new Bitmap("Images/Blocks/Iron.png")); // Last Block image  (11)
          
            Animation otherBlocks = new Animation();
            otherBlocks.imgs.Add(new Bitmap("Images/Blocks/ladder.png")); // Last Block image  (1) 




            Groups[1].Animations.Add(staticBlocks);
            Groups[1].Animations.Add(otherBlocks);

            Animation blockBorders = new Animation();
            blockBorders.imgs.Add(new Bitmap("Images/Border.png"));
           

            Animation blockBreaking = new Animation();
            for (int i = 1; i < 6; i++)
            {
                blockBreaking.imgs.Add(new Bitmap("Images/breaking/breaking" + i + ".png"));
            }

            Groups[1].Animations.Add(blockBreaking);


            //Monsters 

            Animation ZombieRight = new Animation();
            for (int i = 0; i < 2; i++)
            {
                ZombieRight.imgs.Add(new Bitmap("Images/Monsters/Zombie/ZombieRight/zombie" + (i+1) +".png"));
            }
            Groups[2].Animations.Add(ZombieRight);
            Animation ZombieLeft = new Animation();
            for (int i = 0; i < 2; i++)
            {
                ZombieLeft.imgs.Add(new Bitmap("Images/Monsters/Zombie/ZombieLeft/zombie" + (i + 1) + ".png"));
            }
            Groups[2].Animations.Add(ZombieLeft);

            //effects 
            Animation blood = new Animation();
            for (int i = 0; i < 14; i++)
            {
                blood.imgs.Add(new Bitmap("Images/blood/blood (" + (i + 1) + ").png"));
            }
            Groups[3].Animations.Add(blood);
            Animation zombieblood = new Animation();
            for (int i = 0; i < 14; i++)
            {
                zombieblood.imgs.Add(new Bitmap("Images/zombieblood/blood (" + (i + 1) + ").png"));
            }
            Groups[3].Animations.Add(zombieblood);


        }

        void Zoom(int type)
        {
            breaking = null;
            isBreaking = 0;
            if (type == 1) // Zoom in
            {
                camera.ZoomFactor += 0.1f;
                hero.HeroHitBox++;
            }
            else if (type == 2 && camera.ZoomFactor > 1) // Zoom out
            {
                camera.ZoomFactor -= 0.1f;
                hero.HeroHitBox--;
            }
            camera.Update(hero);
        }

        void DrawDouble(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}
