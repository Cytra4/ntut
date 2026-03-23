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
        int[][] bombSpots = new int[16][];
        int[][] points = new int[16][];
        Random rand = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BombInitial();
            PointInitial();
            DrawMap(panel1.CreateGraphics());
        }

        public void DrawMap(Graphics g)
        {
            if (bombSpots == null || points == null) return;

            int cols = 20;
            int rows = 16;
            int tileW = panel1.Width / cols;
            int tileH = panel1.Height / rows;
            int tile = Math.Min(tileW, tileH);

            int gridWidth = tile * cols;
            int gridHeight = tile * rows;
            int offsetX = (panel1.Width - gridWidth) / 2;
            int offsetY = (panel1.Height - gridHeight) / 2;

            Image mineImg = Properties.Resources.mine_ceil;
            Image zeroImg = Properties.Resources.question;
            Image[] openImgs = new Image[9];
            openImgs[1] = Properties.Resources.open1;
            openImgs[2] = Properties.Resources.open2;
            openImgs[3] = Properties.Resources.open3;
            openImgs[4] = Properties.Resources.open4;
            openImgs[5] = Properties.Resources.open5;
            openImgs[6] = Properties.Resources.open6;
            openImgs[7] = Properties.Resources.open7;
            openImgs[8] = Properties.Resources.open8;

            g.Clear(panel1.BackColor);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Rectangle dest = new Rectangle(offsetX + j * tile, offsetY + i * tile, tile, tile);

                    if (bombSpots[i][j] == 1)
                    {
                        if (mineImg != null) g.DrawImage(mineImg, dest);
                    }
                    else
                    {
                        int val = points[i][j];
                        if (val == 0)
                        {
                            if (zeroImg != null) g.DrawImage(zeroImg, dest);
                        }
                        else if (val >= 1 && val <= 8)
                        {
                            var img = openImgs[val];
                            if (img != null) g.DrawImage(img, dest);
                        }
                        else
                        {
                            using (Brush b = new SolidBrush(Color.LightGray))
                                g.FillRectangle(b, dest);
                        }
                    }
                }
            }
        }


        public void BombInitial()
        {
            for (int i = 0; i < 16; i++)
            {
                bombSpots[i] = new int[20];
                for (int j = 0; j < 20; j++)
                {
                    bombSpots[i][j] = 0;
                }
            }

            int bomb_counts = 0;
            while (bomb_counts < 20)
            {
                int i = rand.Next(0, 16);
                int j = rand.Next(0, 20);
                if (bombSpots[i][j] == 0)
                {
                    bombSpots[i][j] = 1;
                    bomb_counts += 1;
                }
            }
        }

        public void PointInitial()
        {
            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new int[20];
                for (int j = 0; j < points[0].Length; j++)
                    points[i][j] = 0;
            }

            for (int i = 0; i < bombSpots.Length; i++)
            {
                for (int j = 0; j < bombSpots[0].Length; j++)
                {
                    if (bombSpots[i][j] == 1)
                    {
                        for (int d = 0; d < 8; d++)
                        {
                            int nx = j + dx[d];
                            int ny = i + dy[d];
                            if (nx >= 0 && nx <= bombSpots[0].Length - 1 && ny >= 0 && ny <= bombSpots.Length - 1)
                            {
                                if (bombSpots[ny][nx] == 1)
                                {
                                    points[ny][nx] = 9;
                                }
                                else
                                {
                                    points[ny][nx] += 1;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
