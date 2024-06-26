﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Multi_Media_Minecraft_Project_YM_MT
{
    
        public class Actor
        {
            public int X, Y, W, H;
            public Bitmap img;
            public List<int> Vars = new List<int>();
        }


    public class Effect
    {
        public int X, Y, W, H;
        public List<Bitmap> imgs = new List<Bitmap>();
        public int iframe = 0;
        public int stTime, endTime;
       
    }
    public class Chest
    {
        public int X, Y, W, H;
        public Bitmap img = new Bitmap("Images/Blocks/chest.png");
        public bool looted = false;
        public List<InventoryItem> Items = new List<InventoryItem>();
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
        public int healthIncrease = 0; 
        public int hungerIncrease = 0;  

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
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].itemID == itemID)
                {
                    if (items[i].quantity > quantity)
                    {
                        items[i].quantity -= quantity;
                    }
                    else
                    {
                        items.RemoveAt(i);
                    }
                    break;
                }
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
        public int hunger = 10; 


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
        List<Block> ElevatorBlocks;
        Bitmap HeroImg = new Bitmap("Images/hero1.png");
        Rectangle rctSrc, rctDst;
        Block Crafting = new Block();
        Hero hero;
        List<Block> jailBlocks;
        AnimatedBlock breaking = null;
        Bitmap breakedImg = null;
        BasicActor Sun; //in single actor
        
        BasicActor HotBarItemsBorder = new BasicActor();
        BasicActor Inventory = new BasicActor();
        cAdvImg Health1 = new cAdvImg();
        BasicActor Health2 = new BasicActor();
        BasicActor Hunger = new BasicActor();
        Actor alex = new Actor();
        int ZombieSpwanQuantity = 3;
        int iframe = 0;
        int breakingI = -1, breakingJ = -1;
        int zoom = -10;
        int isBreaking = 0;
        int isLeftClick = 0;
        int zoomRange = 10;
        bool isWin = false;
        int healthValue = 100;
       
     
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
        List<Chest> Chests = new List<Chest>();
        //List<InventoryItem> heroTools = new List<InventoryItem>();
        bool isBroken = false;
        bool onElev = false;
        bool elevatorMovingUp = false;
        bool elevatorMovingDown = false;
        Random RR = new Random();
        Camera camera;

        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;
            Text = " Hint 5 Diamond";
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

            // Create random biome blocks
            CreateRandomBiomeBlocks();

            // Tree
            CreateTrees();

            
           

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
            int columns = 60;
            int rows = 20; // Number of rows of blocks
            yPos = ClientSize.Height - blockHeight * rows + 1000; // Starting Y position of the bottom row

          

            makeladder(960, yPos, Groups[1].Animations[1].imgs[0]);
            makeElevator(180, yPos, Groups[1].Animations[1].imgs[1]);

            for (int i = 0; i < rows; i++)
            {
                List<Block> rowBlocks = new List<Block>();
                for (int j = 0; j < columns ; j++)
                {
                    int x = j * blockWidth;
                    int y = yPos + (i * blockHeight);
                    if (x < 180|| x > 300 && (x < 180 || y<25*60|| y> 30*60)) { 
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
                }
                blocks2D.Add(rowBlocks);
            }
            
           jailBlocks = new List<Block>();
            alex.W = hero.W;
            alex.H = hero.H;
            alex.X = 3000;
            alex.Y = 30 * 60 - alex.H;
            alex.img = new Bitmap("Images/alex.png");
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j <5; j++)
                {
                    Block blockPnn = new Block();
                    blockPnn.X = alex.X -100+ j * blockWidth;
                    blockPnn.Y = alex.Y-80+ (i * blockHeight);
                    blockPnn.W = blockWidth;
                    blockPnn.H = blockHeight;
                    blockPnn.ItemType = 15;
                    blockPnn.ID = 15;
                    blockPnn.Img = Groups[1].Animations[1].imgs[2];
                    jailBlocks.Add(blockPnn);
                }
            }
            Crafting.X = alex.X - 400;
            Crafting.Y = alex.Y + alex.H-50;
            Crafting.W = 60;
            Crafting.H = 60;
            Crafting.Img = Groups[1].Animations[1].imgs[3];
            jailBlocks.Add(Crafting);
            blocks2D.Add(jailBlocks);
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


            Chest chest1 = new Chest
            {
                X = 60 * 23,
                Y = UpperYPos,
                W = 60,
                H = 60
            };
            Bitmap gunImage = new Bitmap("Images/heroTools/gun.png");
            InventoryItem gun = new InventoryItem(HotBarItemsBorder.X + 30, HotBarItemsBorder.Y, 60, 60, gunImage, 20);
          
            chest1.Items.Add(gun);
         
            Bitmap steakImage = new Bitmap("Images/steak.png");
            InventoryItem steak = new InventoryItem(HotBarItemsBorder.X + 30, HotBarItemsBorder.Y, 60, 60, steakImage, 30)
            {
                healthIncrease = 12,
                hungerIncrease = 12,
                quantity = 64
            };
            chest1.Items.Add(steak);

            Chests.Add(chest1);


            List<Block> rowBlocks = new List<Block>();
            for (int j = 0; j < columns + 30; j++)
            {
                if (j * blockWidth <180|| j * blockWidth>300 && j * blockWidth!=960) { 
                Block blockPnn = new Block();
                blockPnn.X = j * blockWidth;
                blockPnn.Y = UpperYPos + (blockHeight);
                blockPnn.W = blockWidth;
                blockPnn.H = blockHeight;

                blockPnn.Img = Groups[1].Animations[0].imgs[0]; // Always grass for the first 5 rows from the bottom
                blockPnn.ID = 0;

                rowBlocks.Add(blockPnn);
                }
            }
            blocks2D.Add(rowBlocks);
        }
        void CreateCreeper()
        {
            int zombieCount = ZombieSpwanQuantity-2; // Number of zombies to create
            int blockWidth = 60;

           
            Rectangle viewRect = camera.GetViewRect();
            for (int i = 0; i < zombieCount; i++)
            {
                int x;

                x = RR.Next(0, 3400);
               


                // Determine the column and row based on the mouse click position
                int column = x / blockWidth;

                x = column * blockWidth;




                int imgdir = RR.Next(0, 2);
                int Dir = 1;
                if (imgdir == 1)
                {
                    Dir = -1;
                }
                imgdir += 2;
                Enemy Creeper = new Enemy
                {
                    X = x,
                    Y = hero.Y,
                    W = hero.W+50,
                    H = hero.H,

                    type = 1,
                    dir = Dir,
                    imgs = Groups[2].Animations[imgdir].imgs,
                    health = 200,
                    fullHealth = 200,
                };
                Enemies.Add(Creeper);
            }
        }
        void CreateZombie()
        {
            int zombieCount = ZombieSpwanQuantity; // Number of zombies to create
            int blockWidth = 60;
            
       
            Rectangle viewRect = camera.GetViewRect();
            for (int i = 0; i < zombieCount; i++)
            {
                int x;
             
                    x = RR.Next(0, 3400);
                    


                    // Determine the column and row based on the mouse click position
                    int column = x / blockWidth;

                    x = column * blockWidth;


             

                int imgdir = RR.Next(0, 2);
                int Dir = 1;
                if(imgdir==1)
                {
                    Dir = -1;
                }
                Enemy Zombie = new Enemy
                {
                    X = x,
                    Y = hero.Y,
                    W = hero.W,
                    H = hero.H,
                   
                    type =0,
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
            int blockWidth = 60;
           
            Bitmap woodImage = Groups[1].Animations[0].imgs[2]; // Wood image from staticBlocks
            Bitmap treeGrassImage = Groups[1].Animations[0].imgs[3]; // Tree grass image from staticBlocks  
            List<int> existingTreePositions = new List<int>();
            Rectangle viewRect = camera.GetViewRect();
            for (int i = 0; i < treeCount; i++)
            {
                int x;
                do
                {
                    x = RR.Next(0, 3400);
                   


                    // Determine the column and row based on the mouse click position
                    int column = x / blockWidth;

                    x = column * blockWidth;

                   
                } while (CheckOverlap(existingTreePositions, x) && (x>=880 && x<=1020) &&( x > 120 && x< 400) );

                existingTreePositions.Add(x);

                int y = yCurr - woodImage.Height + 100;

                makeRandomTrees(x, y, woodImage, treeGrassImage);
            }
        }

        void makeladder(int baseX, int baseY, Bitmap ladderImage)
        {
           ladderBlocks = new List<Block>();
            blocks2D.Add(ladderBlocks); 
          
           
            for (int i = 1; i < 11; i++)
            {
                Block ladderBlock = new Block
                {
                    X = baseX,
                    Y = baseY - (i * 60),
                    W = 60,
                    H = 60,
                    Img = ladderImage,
                    ID = 14,
                    Z = 2
                };
                ladderBlocks.Add(ladderBlock);
            }

        }
        void makeElevator(int baseX, int baseY, Bitmap ElevatorImage)
        {
            ElevatorBlocks = new List<Block>();
            blocks2D.Add(ElevatorBlocks); //->20

            
            for (int i = 0; i < 3; i++)
            {
                Block Elevator = new Block
                {
                    X = baseX + (i * 60),
                    Y = baseY - 50,
                    W = 60,
                    H = 60,
                    Img = ElevatorImage,
                    ID = 14,
                    Z = 2
                };
                ElevatorBlocks.Add(Elevator);

               

                
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
                TreesBlocks.Add(woodBlock);
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
                    TreesBlocks.Add(treeGrassBlock);
                }
            }
        }

        bool CheckOverlap(List<int> existingPositions, int newX)
        {
            for (int i = 0; i < existingPositions.Count; i++)
            {
                int position = existingPositions[i];

                if (newX >= position - 500 && newX <= position + 500)
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
            int elapsedMinutes = ctTimer / 60;
            hero.hunger-- ;

            Hunger.imgs[0] = new Bitmap("Images/hotbar/hunger" +(10- hero.hunger) + ".png");
        }

        private void MoveElevator(int direction)
        {
            for (int i = 0; i < ElevatorBlocks.Count; i++)
            {
                ElevatorBlocks[i].Y += direction * 15;
              
               


            }
        }

        private void T_Tick(object sender, EventArgs e)
        {
            if (hero.health <= 0)
                return;
            if (isWin) {
                DrawDouble(CreateGraphics());
                hero.health = 0;
                MessageBox.Show("Congrats You Won");
                    t.Stop();
                return;
                    }

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

            if((ctTimer+1)%80==0)
            {
                CreateZombie();
                CreateCreeper();
               
            }
            
            if (ctTimer%5==0)
            {
                //enemie move
                for (int i = 0; i < Enemies.Count; i++)
                {
                    Enemy EnemyTrav = Enemies[i];
                    int type = EnemyTrav.type*2;
                    if (!IsEnemyOnGround(EnemyTrav))
                        EnemyTrav.Y += 15;

                    EnemyTrav.X += EnemyTrav.dir * EnemyTrav.speed;
                    EnemyTrav.moveCt--;
                    if(EnemyTrav.moveCt <=0)
                    {
                        EnemyTrav.moveCt = 30;
                        if(EnemyTrav.dir==1)
                        {
                            EnemyTrav.imgs = Groups[2].Animations[1+type].imgs;
                        }
                        else
                        {
                            EnemyTrav.imgs = Groups[2].Animations[0 + type].imgs;
                        }
                        EnemyTrav.dir *= -1;
                    }

                    //follow hero
                   


                        EnemyTrav.iframe++;
                    if (hero.X < EnemyTrav.X + EnemyTrav.W &&
                        hero.X + hero.W > EnemyTrav.X &&
                        hero.Y < EnemyTrav.Y + EnemyTrav.H &&
                        hero.Y + hero.H > EnemyTrav.Y)
                    {
                        if (EnemyTrav.type == 0)
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
                        else if (EnemyTrav.type == 1)
                        {
                            hero.health -= 4*3;
                            Effect pnnEffect = new Effect();
                            pnnEffect.X = EnemyTrav.X;
                            pnnEffect.Y = EnemyTrav.Y;
                            pnnEffect.W = EnemyTrav.W;
                            pnnEffect.H = EnemyTrav.H;
                            pnnEffect.imgs = Groups[3].Animations[2].imgs; // explosion
                            pnnEffect.stTime = ctTimer;
                            pnnEffect.endTime = ctTimer + 7;
                            Effects.Add(pnnEffect);
                            Health1.rctSrc.Width -= 14 *3;
                            Health1.rctDst.Width -= 15 *3;
                            Enemies.RemoveAt(i);
                          
                         
                            i--;
                        }
                    }
                    else if (EnemyTrav.dir == 0)
                    {
                        EnemyTrav.dir = 1;
                        EnemyTrav.imgs = Groups[2].Animations[0 + type].imgs;
                    }
                    else if (EnemyTrav.X < hero.X + 500 &&
                        EnemyTrav.X > hero.X - 500 &&
                        EnemyTrav.Y < hero.Y + hero.H &&
                        EnemyTrav.Y + EnemyTrav.H > hero.Y)
                    {

                        if (hero.X >= EnemyTrav.X)
                            EnemyTrav.dir = 1;
                        else
                            EnemyTrav.dir = -1;


                        if (EnemyTrav.dir == -1)
                        {
                            EnemyTrav.imgs = Groups[2].Animations[1 + type].imgs;
                        }
                        else
                        {
                            EnemyTrav.imgs = Groups[2].Animations[0 + type].imgs;
                        }

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
            int ElevLimit = ClientSize.Height - 60 + 1000 - 30 * 60;
            if (elevatorMovingUp)
            {
                
                MoveElevator(-1);
                if (ElevatorBlocks[0].Y <= ElevLimit)
                {
                    elevatorMovingUp = false;
                    if (IsOnElev())
                        hero.X += 120;
                   
                }
            }

            if (IsOnElev() && !elevatorMovingUp && !elevatorMovingDown)
            {
                if (ElevatorBlocks[0].Y >= 60*15)
                {
                    hero.Y -=15;
                    elevatorMovingUp = true;
                    

                }
                else
                {
                    hero.Y -= 15;
                    elevatorMovingDown = true;
                }

            }


            if (elevatorMovingDown)
            {
                MoveElevator(1);
                if (ElevatorBlocks[0].Y >= yPos + 60*15)
                {
                    elevatorMovingDown = false;
                    if (IsOnElev())
                    hero.X += 120;
                }
            }

            if (breaking != null && isBreaking == 1 )
            {
                if (ctTimer % 2 == 0 && breaking.iframe < 5)
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

          

            
            if (ctTimer % 60 == 0)
            {
                UpdateHungerStatus();
               
            }
            if (ctTimer+1 % 400 == 0)
            {  
                ZombieSpwanQuantity++;
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

        private bool IsEnemyOnGround(Enemy EnemyTrav)
        {
            for (int i = 0; i < blocks2D.Count; i++)
            {
                List<Block> row = blocks2D[i];
                for (int j = 0; j < row.Count; j++)
                {
                    Block block = row[j];
                    if (EnemyTrav.X < block.X + block.W - 40 &&
                        EnemyTrav.X + EnemyTrav.W - 40 > block.X &&
                        EnemyTrav.Y  <= block.Y + block.H &&
                        EnemyTrav.Y + EnemyTrav.H + 10 >= block.Y && (block.Z == 0)) // Adjust 10 as per the gravity
                    {
                        EnemyTrav.Y = block.Y - EnemyTrav.H + 10; 
                        return true;
                    }
                }
            }
            return false;
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
                        hero.Y + hero.H + 10 >= block.Y && (block.Z ==0 || block.Z == 2)) // Adjust 10 as per the gravity
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
                        newY +20 < block.Y + block.H &&
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
            hero.Y  +hero.H >= block.Y )
                {
                    return true;
                }
            }
          return false;
        }

        bool IsOnElev()
        {
            for (int i = 0; i < ElevatorBlocks.Count; i++)
            {
                Block block = ElevatorBlocks[i];
                if (hero.X + 50 < block.X + block.W &&
            hero.X + 50 + hero.W > block.X &&
            hero.Y <= block.Y + block.H &&
            hero.Y + hero.H+50 >= block.Y)
                {
                    return true;
                }
            }
            return false;
        }

        void checkWin()
        {
            if(hero.X<=alex.X+20&& hero.X >= alex.X - 20 && hero.Y <= alex.Y+20 && hero.Y >= alex.Y  -20)
            {
                isWin =true; 
            }

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
                    checkWin();
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
                    checkWin();
                    break;
                //case Keys.W:
                case Keys.Up:               //Jumping
                case Keys.Space:

                    int newHeroY = hero.Y - 50;
                    if (IsOnGround() && !hero.isJumping )
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
                case Keys.X:     //Zoom-Up
                    Zoom(2);
                    break;
                case Keys.C:     //Zoom-Up
                  
                    // Get the blockID from the hotbar selection
                    int itemIndex = HotBarItemsBorder.Vars[0];
                    Text += itemIndex;
                    if (itemIndex < hero.Inventory.items.Count)
                    {
                        if (hero.Inventory.items[itemIndex].itemID == 6 && hero.Inventory.items[itemIndex].quantity >= 5)
                        {
                            hero.Inventory.items.RemoveAt(itemIndex);
                            //make pickaxe
                            Bitmap picAxe = new Bitmap("Images/pickaxe.png");
                            InventoryItem item = new InventoryItem(HotBarItemsBorder.X + 30, HotBarItemsBorder.Y, 60, 60, picAxe, 21);
                            hero.Inventory.AddItem(item);
                        }
                    }
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
                case Keys.R:
                    Inventory.Vars[0] *= -1;
                    break;
                case Keys.E:
                      int foodIndex = HotBarItemsBorder.Vars[0];
                    if (foodIndex < hero.Inventory.items.Count)
                    {
                        InventoryItem foodItem = hero.Inventory.items[foodIndex];
                        if (foodItem.healthIncrease > 0 || foodItem.hungerIncrease > 0)
                        {

                            hero.health += foodItem.healthIncrease;
                            hero.hunger += foodItem.hungerIncrease;
                            
                            if (hero.health > 100)
                                hero.health = 100;
                            
                            if(hero.hunger > 10)
                                hero.hunger = 10;
                            Hunger.imgs[0] = new Bitmap("Images/hotbar/hunger" + (10-hero.hunger) + ".png");
                            

                            Health1.rctSrc.Width += 14 * 3;
                            Health1.rctDst.Width += 15 * 3;
                            
                            if(hero.health==100)
                            {
                                Health1.rctSrc.Width -= 14 * 2;
                                Health1.rctDst.Width -= 15 * 2;
                            }


                            hero.Inventory.RemoveItem(foodItem.itemID, 1);
                        }
                    }
                    break;
                case Keys.F:
                    int hotbarIndex = HotBarItemsBorder.Vars[0];
                  
                    if (hotbarIndex < hero.Inventory.items.Count && hero.Inventory.items[hotbarIndex].itemID == 20)
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

                    int laserStartX, laserEndX;
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
                        if ((enemy.X >= laserStartX-100 && enemy.X + enemy.W <= laserEndX) &&
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
            int blockWidth = 60; 
            int blockHeight = 60;

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
                                    // Get the blockID from the hotbar selection
                                    int itemIndex = HotBarItemsBorder.Vars[0];
                                
                                    if(itemIndex< hero.Inventory.items.Count) { 
                                        if (block.ID == 15 && hero.Inventory.items[itemIndex].itemID !=21)
                                            break;
                                       
                                    }
                                    else if (block.ID == 15 && itemIndex >= hero.Inventory.items.Count)
                                    {
                                        break;
                                    }
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

                    for (int i = 0; i < Chests.Count; i++)
                    {
                        
                        Chest block = Chests[i];
                        if (block.X < hero.X + hero.HeroHitBox &&
                               block.X >= hero.X - hero.HeroHitBox &&
                               block.Y < hero.Y + hero.HeroHitBox &&    //check range
                                block.Y >= hero.Y - hero.HeroHitBox
                               )
                        {

                            if (!block.looted) 
                            {
                                if (clickX1 <= block.X + block.W && clickX1 >= block.X &&
                                     clickY1 >= block.Y && clickY1 <= block.Y + block.H)
                                {
                                    for (int j = 0; j < block.Items.Count; j++)
                                    {
                                        hero.Inventory.AddItem(block.Items[j]);
                                        block.looted = true;
                                    }
                                    block.Items[1].quantity = 64;
                                }
                            }
                        }

                    }

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
            //alex
            g.DrawImage(alex.img, (alex.X - viewRect.X) * camera.ZoomFactor, (alex.Y - viewRect.Y) * camera.ZoomFactor, alex.W * camera.ZoomFactor, alex.H * camera.ZoomFactor);

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
         
            for (int i = 0; i < Chests.Count; i++)
            {
                Chest block = Chests[i];
                g.DrawImage(block.img,
                            (block.X - viewRect.X) * camera.ZoomFactor,
                            (block.Y - viewRect.Y) * camera.ZoomFactor,
                            block.W * camera.ZoomFactor,
                            block.H * camera.ZoomFactor);
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
                g.DrawLine(P, (location1.X - viewRect.X) * camera.ZoomFactor, (location1.Y - viewRect.Y) * camera.ZoomFactor, location2.X,( location2.Y - viewRect.Y) * camera.ZoomFactor);
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
            if(isWin)
            {
                g.DrawImage(new Bitmap("Images/win.png"), 0, 0, ClientSize.Width, ClientSize.Height);
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
            Animation oreBlocks = new Animation();
            oreBlocks.imgs.Add(new Bitmap("Images/Blocks/grass.png"));
            oreBlocks.imgs.Add(new Bitmap("Images/Blocks/Dirt.png"));
            oreBlocks.imgs.Add(new Bitmap("Images/Blocks/Oak.png"));
            oreBlocks.imgs.Add(new Bitmap("Images/Blocks/grassleaves.png"));
            oreBlocks.imgs.Add(new Bitmap("Images/Blocks/stone.png"));
            oreBlocks.imgs.Add(new Bitmap("Images/Blocks/Coal.png"));
            oreBlocks.imgs.Add(new Bitmap("Images/Blocks/Diamond.png"));
            oreBlocks.imgs.Add(new Bitmap("Images/Blocks/Emerald.png"));
            oreBlocks.imgs.Add(new Bitmap("Images/Blocks/Gold.png"));
            oreBlocks.imgs.Add(new Bitmap("Images/Blocks/Ruby.png"));
            oreBlocks.imgs.Add(new Bitmap("Images/Blocks/Sapphire.png"));
            oreBlocks.imgs.Add(new Bitmap("Images/Blocks/Iron.png")); // Last Block image  (11)
          
            Animation otherBlocks = new Animation();
            otherBlocks.imgs.Add(new Bitmap("Images/Blocks/ladder.png")); 
            otherBlocks.imgs.Add(new Bitmap("Images/Blocks/elevator.png")); 
            otherBlocks.imgs.Add(new Bitmap("Images/Blocks/jail.png"));  
            otherBlocks.imgs.Add(new Bitmap("Images/Blocks/crafting.png"));  // Last Block image  (3) 

            Groups[1].Animations.Add(oreBlocks);
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


            Animation CreeperRight = new Animation();
            for (int i = 0; i < 11; i++)
            {
                CreeperRight.imgs.Add(new Bitmap("Images/Monsters/creeper/creeperRight/creeper (" + (i + 1) + ").gif"));
            }
            Groups[2].Animations.Add(CreeperRight);
            Animation CreeperLeft = new Animation();
            for (int i = 0; i < 11; i++)
            {
                CreeperLeft.imgs.Add(new Bitmap("Images/Monsters/creeper/creeperLeft/creeper (" + (i + 1) + ").gif"));
            }
            Groups[2].Animations.Add(CreeperLeft);

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

            Animation creeperParticles = new Animation();
            for (int i = 0; i < 14; i++)
            {
                creeperParticles.imgs.Add(new Bitmap("Images/creeperParticles/creeper (" + (i + 1) + ").gif"));
            }
            Groups[3].Animations.Add(creeperParticles);

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
