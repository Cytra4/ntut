using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Graphics g = panel1.CreateGraphics();
            g.Clear(Color.Azure);

            Random rand = new Random();
            Pen p = new Pen(Color.Black, 1);
            for (int i = 0; i < 10; i++)
            {
                float pos_x = rand.Next(0, panel1.Width - 30);
                float pos_y = rand.Next(0, panel1.Height - 30);
                int radius = rand.Next(50, 120);
                p = new Pen(Color.FromArgb(rand.Next(256),rand.Next(256),rand.Next(256)), 1);
                g.DrawEllipse(p, pos_x, pos_y, radius, radius);
            }

            g.Dispose();
            p.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Graphics g = panel1.CreateGraphics();
            g.Clear(Color.Azure);

            Random rand = new Random();
            Pen p = new Pen(Color.Black, 1);
            for (int i = 0; i < 10; i++)
            {
                float pos_x = rand.Next(0, panel1.Width - 30);
                float pos_y = rand.Next(0, panel1.Height - 30);
                int width = rand.Next(30, 100);
                int height = rand.Next(30, 100);
                p = new Pen(Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256)), 1);
                g.DrawRectangle(p, pos_x, pos_y, width, height);
            }

            g.Dispose();
            p.Dispose();
        }
    }
}
