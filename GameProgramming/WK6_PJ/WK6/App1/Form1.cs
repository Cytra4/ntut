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
            DrawMap();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DrawMap();
        }

        void DrawMap()
        {
            Mine m = new Mine();
            int[][] map = m.GetMap();
            string result = "";
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    if (j != map[0].Length - 1)
                    {
                        result += $"{map[i][j]} ";
                    }
                    else
                    {
                        result += $"{map[i][j]}\n";
                    }
                }
            }
            label1.Text = result;
            DrawGrid(map);
        }

        void DrawGrid(int[][] map)
        {
            Graphics g = panel1.CreateGraphics();
            Pen p = new Pen(Color.Red, 2);
            g.Clear(Color.White);

            int size = panel1.Height / 10;
            for (int i = 0; i < 10 + 1; i++)
            {
                g.DrawLine(p, new Point(0, i * size), new Point(12 * size, i * size));
            }

            size = panel1.Width / 12;
            for (int i = 0; i < 12 + 1; i++)
            {
                g.DrawLine(p, new Point(i * size, 0), new Point(i * size, panel1.Height));
            }

            Font f = new Font("微軟正黑體", 15);
            Brush b = Brushes.Blue;

            int x_pos = 0;
            int y_pos = 0;
            int cellH = 32;
            int cellW = 30;
            for (int i = 0; i < map.Length; i++)
            {
                x_pos = 0;
                for (int j = 0; j < map[0].Length; j++)
                {
                    int num = map[i][j];
                    SizeF textSize = g.MeasureString($"{num}", f);
                    float px = (cellW - textSize.Width) / 2;
                    float py = (cellH - textSize.Height) / 2;
                    g.DrawString($"{num}", f, b, x_pos + px, y_pos + py);
                    x_pos += cellW;
                }
                y_pos += cellH;
            }
        }
    }
}

class Mine
{
    int[][] map = new int[10][];
    int[][] mines = new int[10][];

    public Mine()
    {
        for (int i = 0; i < 10; i++)
        {
            map[i] = new int[12];
            mines[i] = new int[12];
            for (int j = 0; j < 12; j++)
            {
                map[i][j] = 0;
                mines[i][j] = 0;
            }
        }
        GenerateMines();
        MarkMap();
    }

    public void GenerateMines()
    {
        int mine_count = 0;
        Random rand = new Random();
        while (mine_count < 15)
        {
            int i = rand.Next(0, 10);
            int j = rand.Next(0, 12);
            if (mines[i][j] == 0)
            {
                mines[i][j] = 1;
                mine_count++;
            }
        }
    }

    public void MarkMap()
    {
        int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

        for (int i = 0; i < mines.Length; i++)
        {
            for (int j = 0; j < mines[0].Length; j++)
            {
                if (mines[i][j] == 1)
                {
                    for (int d = 0; d < 8; d++)
                    {
                        int nx = j + dx[d];
                        int ny = i + dy[d];
                        if (nx>=0 && nx <= mines[0].Length-1 && ny>=0 && ny <= mines.Length-1)
                        {
                            if (mines[ny][nx] == 1)
                            {
                                map[ny][nx] = 9;
                            }
                            else
                            {
                                map[ny][nx] += 1;
                            }
                        }
                    }
                }
            }
        }
    }

    public void Show()
    {
        for (int i = 0; i < mines.Length; i++)
        {
            for (int j = 0; j < mines[0].Length; j++)
            {
                Console.Write($"{mines[i][j]}, ");
            }
            Console.Write("\n");
        }
        Console.Write("\n");
        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map[0].Length; j++)
            {
                Console.Write($"{map[i][j]}, ");
            }
            Console.Write("\n");
        }
    }

    public int[][] GetMap()
    {
        return map;
    }
}
