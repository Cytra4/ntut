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
        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(500, 500);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Graphics g = this.CreateGraphics();
            Pen p = new Pen(Color.Red, 2);
            g.Clear(Color.White);

            float y_pos = 80;
            for (int i = 0; i < 6; i++)
            {
                g.DrawLine(p, 50, y_pos, 450, y_pos);
                y_pos += 80;
            }

            float x_pos = 50;
            for (int i = 0; i < 6; i++)
            {
                g.DrawLine(p, x_pos, 80, x_pos, 480);
                x_pos += 80;
            }

            Font f = new Font("微軟正黑體", 20);
            Brush b = Brushes.Blue;

            x_pos = 50;
            y_pos = 80;

            float cellW = 80;
            float cellH = 80;

            int[][] numbers = randomGen(5); 

            for (int i = 0; i < 5; i++)
            {
                x_pos = 50;
                for (int j = 0; j < 5; j++)
                {
                    int num = numbers[i][j];
                    SizeF textSize = g.MeasureString($"{num}", f);
                    float px = (cellW - textSize.Width) / 2;
                    float py = (cellH - textSize.Height) / 2;
                    g.DrawString($"{num}", f, b, x_pos + px, y_pos + py);
                    x_pos += 80;
                }
                y_pos += 80;
            }
        }

        int[][] randomGen(int n)
        {
            int[][] gen = new int[n][];
            int[] appeared = new int[n * n];
            int counter = 0;
            Random rand = new Random();

            for (int i = 0; i < n; i++)
            {
                gen[i] = new int[n];
                int[] tmp = new int[n];
                int number = rand.Next(1, n * n + 1);
                for (int j = 0; j < n; j++)
                {
                    while (appeared.Contains(number))
                    {
                        number = rand.Next(1, n * n + 1);
                    }
                    tmp[j] = number;
                    appeared[counter] = number;
                    counter++;
                }
                gen[i] = tmp;
            }
            return gen;
        }
    }
}
