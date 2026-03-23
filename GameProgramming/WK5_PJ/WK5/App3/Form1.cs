using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App3
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
            Random rand = new Random();
            int[] cards = new int[5];
            Point[] points = new Point[]
            {
                new Point(0, 0),
                new Point(238, 0),
                new Point(120, 100),
                new Point(0, 198),
                new Point(238, 198)
            };

            for (int i = 0; i < 5; i++)
            {
                int tmp = rand.Next(0, 52);
                while (cards.Contains(tmp))
                {
                    tmp = rand.Next(0, 52);
                }
                cards[i] = tmp;
            }

            var type = typeof(Properties.Resources);
            for (int i = 0; i < 5; i++)
            {
                string resourceName = "poker" + cards[i].ToString();
                object obj = Properties.Resources.ResourceManager.GetObject(resourceName);
                if (obj is Image card)
                {
                    float scaledWidth = card.Width * 1.5f;
                    float scaledHeight = card.Height * 1.5f;
                    g.DrawImage(card, points[i].X, points[i].Y, scaledWidth, scaledHeight);
                }
            }
        }
    }
}
