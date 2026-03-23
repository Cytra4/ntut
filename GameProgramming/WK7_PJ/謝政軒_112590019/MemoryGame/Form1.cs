using MemoryGame.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryGame
{
    public partial class Form1 : Form
    {
        Image image = Resources.poker;
        int rows = 4;
        int cols = 5;
        int cardWidth;
        int cardHeight;
        int ds = 2;

        int[] card1 = new int[2];
        int[] card2 = new int[2];
        int picked = 0;


        int[][] pokerList = new int[4][];
        bool[][] flipped = new bool[4][];

        public Form1()
        {
            InitializeComponent();
            cardWidth = image.Width / 13;
            cardHeight = image.Height / 5;
            board.Width = ds + (cardWidth + ds) * cols;
            board.Height = ds + (cardHeight + ds) * rows;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DrawBackground();
            pokerList = GenerateCardList();
        }

        public void DrawBackground()
        {
            Graphics g = board.CreateGraphics();
            for (int i = 0; i < rows; i++)
            {
                flipped[i] = new bool[cols];
                for (int j = 0; j < cols; j++)
                {
                    DrawPoker(g, 54, j * (cardWidth + ds), i * (cardHeight + ds));
                    flipped[i][j] = false;
                }
            }
        }

        public void DrawPokerList()
        {
            Graphics g = board.CreateGraphics();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    DrawPoker(g, pokerList[i][j], j * (cardWidth + ds), i * (cardHeight + ds));
                }
            }
        }

        public int[][] GenerateCardList()
        {
            Random rand = new Random();
            int[] chosen = Enumerable.Range(0, 52)
                                     .OrderBy(_ => rand.Next())
                                     .Take(10)
                                     .ToArray();

            var allCards = chosen.Concat(chosen).OrderBy(_ => rand.Next()).ToList();

            int[][] result = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    result[i][j] = allCards[i * cols + j];
                }
            }

            return result;
        }


        public int[] IndexToRC(int index, int col)
        {
            int r = (int)(index / col);
            int c = index % col;
            return new int[]{r, c};
        }

        public void DrawPoker(Graphics g, int n, int x, int y)
        {
            int[] rc = IndexToRC(n, 13);
            g.DrawImage(image, 
                new Rectangle(x, y, cardWidth, cardHeight),
                new Rectangle(rc[1]*cardWidth, rc[0]*cardHeight, cardWidth, cardHeight),
                GraphicsUnit.Pixel
            );
        }

        public void UpdatePoker()
        {
            Graphics g = board.CreateGraphics();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (flipped[i][j] == true)
                    {
                        DrawPoker(g, pokerList[i][j], j * (cardWidth + ds), i * (cardHeight + ds));
                    }
                    else
                    {
                        DrawPoker(g, 54, j * (cardWidth + ds), i * (cardHeight + ds));
                    }
                }
            }
        }

        private async void board_MouseClick(object sender, MouseEventArgs e)
        {
            int px = e.X;
            int py = e.Y;
            int r = py / (cardHeight + ds);
            int c = px / (cardWidth + ds);

            if (flipped[r][c] == true)
            {
                return;
            }

            if (picked == 0)
            {
                card1[0] = r;
                card1[1] = c;
                flipped[r][c] = true;
                picked += 1;
                UpdatePoker();
            }
            else if (picked == 1)
            {
                card2[0] = r;
                card2[1] = c;
                flipped[r][c] = true;
                picked = 0;

                UpdatePoker();
                await Task.Delay(1000);
                if (pokerList[card1[0]][card1[1]] == pokerList[card2[0]][card2[1]])
                {
                    flipped[card1[0]][card1[1]] = true;
                    flipped[card2[0]][card2[1]] = true;
                }
                else
                {
                    flipped[card1[0]][card1[1]] = false;
                    flipped[card2[0]][card2[1]] = false;
                }
                UpdatePoker();
            }
        }
    }
}
