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


    public partial class Form1 : Form
    {
        Bitmap off;
        Bitmap BackImg = new Bitmap("Images/Back.jpg");
        int ex = -1;
        int ey = -1;
        List<Block> blocks = new List<Block>();

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

            for (int i = 0; i < blocks.Count; i++)
            {
                Block ptrav = blocks[i];
                g.DrawImage(ptrav.img, ptrav.X, ptrav.Y, ptrav.img.Width, ptrav.img.Height);
            }
        }
        void DrawDouble(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}
