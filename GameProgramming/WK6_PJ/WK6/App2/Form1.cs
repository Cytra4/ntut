using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App2
{
    public partial class Form1 : Form
    {
        Rect[] rects;
        Circle[] circles;
        Color[] colors;
        Brush[] brushes;
        long interval = 500000;
        Pen pen1 = new Pen(Color.Red);

        public Form1()
        {
            InitializeComponent();
            Initialize();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random rand = new Random();

            Graphics g = panel1.CreateGraphics();

            Bitmap image = new Bitmap(panel1.Width, panel1.Height);
            Graphics canvas = Graphics.FromImage(image);

            int times = 100;
            int k = 0;
            DateTime now = DateTime.Now;
            long st, et;

            while (k < times)
            {
                st = DateTime.Now.Ticks;

                update(canvas);

                et = DateTime.Now.Ticks;

                while (et - st < interval)
                {
                    et = DateTime.Now.Ticks;
                }

                g.DrawImage(image, 0, 0);
                k++;
            }
        }

        public void update(Graphics G)
        {
            G.Clear(Color.White);
            for (int i = 0; i < rects.Length; i++)
            {
                rects[i].x += rects[i].velo.x;
                rects[i].y += rects[i].velo.y;
                pen1.Color = colors[i];

                G.DrawRectangle(pen1, rects[i].x - rects[i].w / 2, rects[i].y - rects[i].h / 2, rects[i].w, rects[i].h);
            }

            for (int i = 0; i < circles.Length; i++)
            {
                circles[i].x += circles[i].velo.x;
                circles[i].y += circles[i].velo.y;
                pen1.Color = colors[i+6];

                G.DrawEllipse(pen1, circles[i].x - circles[i].r, circles[i].y - circles[i].r, 2 * circles[i].r, 2 * circles[i].r);
            }

            CheckCollision(panel1.Width, panel1.Height);
        }

        public void CheckCollision(int w, int h)
        {
            for (int i = 0; i < rects.Length; i++)
            {
                if (rects[i].x - rects[i].w/2 <= 0 || rects[i].x + rects[i].w/2 >= w)
                {
                    rects[i].velo.x *= -1;
                }
                if (rects[i].y - rects[i].h / 2 <= 0 || rects[i].y + rects[i].h / 2 >= h)
                {
                    rects[i].velo.y *= -1;
                }
            }

            for (int i = 0; i < circles.Length; i++)
            {
                if (circles[i].x <= circles[i].r - 1 || w <= circles[i].x + circles[i].r - 1)
                {
                    circles[i].velo.x *= -1;
                }
                if (circles[i].y <= circles[i].r - 1 || h <= circles[i].y + circles[i].r - 1)
                {
                    circles[i].velo.y *= -1;
                }
            }
        }

        void Initialize()
        {
            rects = new Rect[6];
            circles = new Circle[6];
            colors = new Color[12];
            brushes = new Brush[12];

            Random rand = new Random();

            for (int k = 0; k < 12; k++)
            {
                colors[k] = Color.FromArgb(rand.Next(0, 256),
                                           rand.Next(0, 256),
                                           rand.Next(0, 256));
            }

            for (int k = 0; k < 12; k++)
            {
                brushes[k] = new SolidBrush(Color.FromArgb(rand.Next(0, 256),
                                                           rand.Next(0, 256),
                                                           rand.Next(0, 256)));
            }

            for (int i = 0; i < rects.Length; i++)
            {
                rects[i] = new Rect();
                rects[i].w = rand.Next(30, 60);
                rects[i].h = rand.Next(30, 60);

                rects[i].x = rand.Next((int)rects[i].w, (int)(panel1.Width - rects[i].w));
                rects[i].y = rand.Next((int)rects[i].h, (int)(panel1.Height - rects[i].h));

                rects[i].velo.x = rand.Next(1, 10);
                rects[i].velo.y = rand.Next(1, 10);
            }

            for (int i = 0; i < circles.Length; i++)
            {
                circles[i] = new Circle();
                circles[i].r = rand.Next(20, 50);

                circles[i].x = rand.Next((int)circles[i].r, (int)(panel1.Width - circles[i].r));
                circles[i].y = rand.Next((int)circles[i].r, (int)(panel1.Height - circles[i].r));

                circles[i].velo.x = rand.Next(1, 10);
                circles[i].velo.y = rand.Next(1, 10);
            }
        }
    }

    public struct Vec2
    {
        public int x, y;
    }

    class Rect
    {
        public float x, y;
        public float w, h;
        public Vec2 velo = new Vec2();
    }

    class Circle
    {
        public float x, y;
        public float r;
        public Vec2 velo = new Vec2();
    }
}
