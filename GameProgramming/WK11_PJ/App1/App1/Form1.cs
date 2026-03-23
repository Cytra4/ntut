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

namespace App1
{
    public partial class Form1 : Form
    {
        int[][] map = new int[10][];
        char[][] mapDisplay = new char[10][];
        Random rand = new Random();

        public Form1()
        {
            InitializeComponent();
            MapInitial();
            panel1.MouseClick += panel1_MouseClick;
        }

        public void MapInitial()
        {
            for (int i = 0; i < 10; i++)
            {
                map[i] = new int[10];
                mapDisplay[i] = new char[10];
                for (int j = 0; j < 10; j++)
                {
                    map[i][j] = 0;
                    mapDisplay[i][j] = '#';
                }
            }
        }

        public void DrawMap()
        {
            Graphics g = panel1.CreateGraphics();
            Pen p = new Pen(Color.Red, 2);
            g.Clear(Color.White);

            float y_pos = 10;
            for (int i = 0; i < 11; i++)
            {
                g.DrawLine(p, 10, y_pos, 410, y_pos);
                y_pos += 40;
            }

            float x_pos = 10;
            for (int i = 0; i < 11; i++)
            {
                g.DrawLine(p, x_pos, 10, x_pos, 410);
                x_pos += 40;
            }

            Font f = new Font("微軟正黑體", 15, FontStyle.Bold);
            Brush blue = Brushes.Blue;
            Brush red = Brushes.Red;
            Brush black = Brushes.Black;

            x_pos = 10;
            y_pos = 10;

            float cellW = 40;
            float cellH = 40;

            for (int i = 0; i < 10; i++)
            {
                x_pos = 10;
                for (int j = 0; j < 10; j++)
                {
                    int num = map[i][j];
                    SizeF textSize = g.MeasureString($"{num}", f);
                    float px = (cellW - textSize.Width) / 2;
                    float py = (cellH - textSize.Height) / 2;
                    if (map[i][j] == 0)
                    {
                        g.DrawString($"{num}", f, black, x_pos + px, y_pos + py);
                    }   
                    else if (map[i][j] == 9)
                    {
                        g.DrawString($"{num}", f, red, x_pos + px, y_pos + py);
                    }
                    else
                    {
                        g.DrawString($"{num}", f, blue, x_pos + px, y_pos + py);
                    }
                    x_pos += 40;
                }
                y_pos += 40;
            }
        }

        public void UpdateMapAfterClick(int row, int col)
        {
            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };
            for (int d = 0; d < 8; d++)
            {
                int nx = col + dx[d];
                int ny = row + dy[d];
                if (nx >= 0 && nx <= map[0].Length - 1 && ny >= 0 && ny <= map.Length - 1)
                {
                    if (map[ny][nx] != 9)
                    {
                        map[ny][nx] += 1;
                    }
                }
            }
        }

        public void GenerateRandomMap()
        {
            int bombX;
            int bombY;
            int bombCount = 0;
            while (bombCount < 10)
            {
                bombX = rand.Next(0, 10);
                bombY = rand.Next(0, 10);
                if (map[bombY][bombX] == 0)
                {
                    map[bombY][bombX] = 9;
                    bombCount += 1;
                }
            }

            MapScoreInit();
        }

        public void MapScoreInit()
        {
            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    if (map[i][j] == 9)
                    {
                        for (int d = 0; d < 8; d++)
                        {
                            int nx = j + dx[d];
                            int ny = i + dy[d];
                            if (nx >= 0 && nx <= map[0].Length - 1 && ny >= 0 && ny <= map.Length - 1)
                            {
                                if (map[ny][nx] != 9)
                                {
                                    map[ny][nx] += 1;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DrawDisplayMap()
        {
            Graphics g = panel1.CreateGraphics();
            Pen p = new Pen(Color.Red, 2);
            g.Clear(Color.White);

            float y_pos = 10;
            for (int i = 0; i < 11; i++)
            {
                g.DrawLine(p, 10, y_pos, 410, y_pos);
                y_pos += 40;
            }

            float x_pos = 10;
            for (int i = 0; i < 11; i++)
            {
                g.DrawLine(p, x_pos, 10, x_pos, 410);
                x_pos += 40;
            }

            Font f = new Font("微軟正黑體", 15, FontStyle.Bold);
            Brush blue = Brushes.Blue;
            Brush red = Brushes.Red;
            Brush black = Brushes.Black;

            x_pos = 10;
            y_pos = 10;

            float cellW = 40;
            float cellH = 40;

            for (int i = 0; i < 10; i++)
            {
                x_pos = 10;
                for (int j = 0; j < 10; j++)
                {
                    char num = mapDisplay[i][j];
                    SizeF textSize = g.MeasureString($"{num}", f);
                    float px = (cellW - textSize.Width) / 2;
                    float py = (cellH - textSize.Height) / 2;
                    if (mapDisplay[i][j] == '#')
                    {
                        g.DrawString($"{num}", f, black, x_pos + px, y_pos + py);
                    }
                    else if (mapDisplay[i][j] == '9')
                    {
                        g.DrawString($"{num}", f, red, x_pos + px, y_pos + py);
                    }
                    else
                    {
                        g.DrawString($"{num}", f, blue, x_pos + px, y_pos + py);
                    }
                    x_pos += 40;
                }
                y_pos += 40;
            }
        }

        private void RevealCell(int r, int c)
        {
            if (map[r][c] == 0)
                mapDisplay[r][c] = ' ';
            else if (map[r][c] == 9)
                mapDisplay[r][c] = '9';
            else
                mapDisplay[r][c] = (char)('0' + map[r][c]);
        }

        public void DisplayUpdate(int startRow, int startCol)
        {
            int rows = map.Length;
            int cols = map[0].Length;

            if (mapDisplay[startRow][startCol] != '#')
            {
                DrawDisplayMap();
                return;
            }

            bool[,] visited = new bool[rows, cols];
            Queue<(int r, int c)> q = new Queue<(int r, int c)>();
            q.Enqueue((startRow, startCol));
            visited[startRow, startCol] = true;

            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

            while (q.Count > 0)
            {
                var (r, c) = q.Dequeue();

                RevealCell(r, c);

                if (map[r][c] == 0)
                {
                    for (int d = 0; d < 8; d++)
                    {
                        int nr = r + dy[d];
                        int nc = c + dx[d];
                        if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && !visited[nr, nc])
                        {
                            visited[nr, nc] = true;
                            if (map[nr][nc] == 0)
                                q.Enqueue((nr, nc));
                            RevealCell(nr, nc);
                        }
                    }
                }
            }
            DrawDisplayMap();
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            int left = 10, top = 10;
            int cellW = 40, cellH = 40;
            int rows = 10, cols = 10;

            if (e.X < left || e.Y < top) return;

            int col = (e.X - left) / cellW;
            int row = (e.Y - top) / cellH;

            if (row >= 0 && row < rows && col >= 0 && col < cols)
            {
                //第一題
                if (e.Button == MouseButtons.Right)
                {
                    map[row][col] = 9;
                    UpdateMapAfterClick(row, col);
                    DrawMap();
                }
                //第二題
                else if (e.Button == MouseButtons.Left)
                {
                    DisplayUpdate(row,col);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MapInitial();
            DrawMap();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MapInitial();
            GenerateRandomMap();
            DrawDisplayMap();
        }
    }
}
