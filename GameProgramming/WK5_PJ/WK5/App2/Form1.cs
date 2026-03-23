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
        Random rand = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Graphics g = panel1.CreateGraphics();
            g.Clear(Color.Azure);

            for (int i = 0; i < 10; i++)
            {
                CircleGenerate(g, panel1.Width, panel1.Height);
            }

            g.Dispose();
        }

        private void CircleGenerate(Graphics g, int p_width, int p_height)
        {
            int radius = rand.Next(20, 80);
            float pos_x = rand.Next(radius, p_width - radius);
            float pos_y = rand.Next(radius, p_height - radius);

            Pen p = new Pen(Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256)), 1);
            g.DrawEllipse(p, pos_x - radius, pos_y - radius, radius * 2, radius * 2);

            p.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Graphics g = panel1.CreateGraphics();
            g.Clear(Color.Azure);

            List<Rectangle> rectangles = new List<Rectangle>();
            for (int i = 0; i < 10; i++)
            {
                RectangleGenerate(g, rectangles, panel1.Width, panel1.Height);
            }
           
            g.Dispose();
        }

        private void RectangleGenerate(Graphics g, List<Rectangle> recs, int p_width, int p_height)
        {
            bool unvalid = true;
            Pen p = new Pen(Color.Black, 1);
            int width;
            int height;
            int pos_x;
            int pos_y;

            while (unvalid)
            {
                width = rand.Next(30, 50);
                height = rand.Next(30, 50);
                pos_x = rand.Next(width, panel1.Width - width);
                pos_y = rand.Next(height, panel1.Height - height);

                Rectangle new_rect = new Rectangle(pos_x, pos_y, width, height);

                unvalid = false;
                foreach(Rectangle rec in recs)
                {
                    bool overlap =
                    !(new_rect.Right < rec.Left ||
                    new_rect.Left > rec.Right ||
                    new_rect.Bottom < rec.Top ||
                    new_rect.Top > rec.Bottom);

                    if (overlap)
                    {
                        unvalid = true;
                        break;
                    }
                }

                if (!unvalid)
                {
                    p = new Pen(Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256)), 1);
                    g.DrawRectangle(p, new_rect);
                    recs.Add(new_rect);
                }
            }
        }
    }
}
