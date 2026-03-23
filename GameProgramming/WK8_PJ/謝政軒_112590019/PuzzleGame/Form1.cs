using PuzzleGame.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuzzleGame
{
    public partial class Form1 : Form
    {
        Image img = Resources.sunset_with_snoopy;
        int rows;
        int cols;
        PuzzlePiece[] pieces;

        private bool gridVisible = false;
        private int gridSX = 0, gridSY = 0, gridRows = 0, gridCols = 0, gridPieceW = 0, gridPieceH = 0;

        public Form1()
        {
            InitializeComponent();
            panel1.Width = img.Width * 2 + 100;
            panel1.Height = img.Height * 2;
            this.WindowState = FormWindowState.Maximized;

            typeof(Panel).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                 .SetValue(panel1, true, null);

            panel1.Paint += Panel1_Paint;
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            if (!gridVisible) return;

            using (Pen pen = new Pen(Color.Red, 1f))
            {
                for (int r = 0; r <= gridRows; ++r)
                {
                    int y = gridSY + r * gridPieceH;
                    e.Graphics.DrawLine(pen, gridSX, y, gridSX + gridCols * gridPieceW, y);
                }

                for (int c = 0; c <= gridCols; ++c)
                {
                    int x = gridSX + c * gridPieceW;
                    e.Graphics.DrawLine(pen, x, gridSY, x, gridSY + gridRows * gridPieceH);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            rows = 3;
            cols = 3;
            int pieceW = (int)(img.Width / cols * 0.99f);
            int pieceH = (int)(img.Height / rows * 0.99f);
            pieces = createPuzzles(img, rows, cols);
            panel1.Controls.Clear();
            DrawPuzzles(img, rows, cols);
            ShufflePieces();

            gridVisible = true;
            gridSX = img.Width + 50;
            gridSY = 0;
            gridRows = rows;
            gridCols = cols;
            gridPieceW = pieceW;
            gridPieceH = pieceH;

            panel1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            rows = 4;
            cols = 5;
            int pieceW = (int)(img.Width / cols * 0.99f);
            int pieceH = (int)(img.Height / rows * 0.99f);
            pieces = createPuzzles(img, rows, cols);
            panel1.Controls.Clear();
            DrawPuzzles(img, rows, cols);
            ShufflePieces();
            
            gridVisible = true;
            gridSX = img.Width + 50;
            gridSY = 0;
            gridRows = rows;
            gridCols = cols;
            gridPieceW = pieceW;
            gridPieceH = pieceH;

            panel1.Invalidate();
        }

        public void ShufflePieces()
        {
            if (pieces == null || pieces.Length == 0)
                return;

            Random rand = new Random();

            // Get the bounds where pieces can be randomly placed
            int maxX = panel1.Width - pieces[0].Width;
            int maxY = panel1.Height - pieces[0].Height;

            // Shuffle each piece position
            foreach (var piece in pieces)
            {
                int x = rand.Next(0, maxX);
                int y = rand.Next(0, maxY);
                piece.Left = x;
                piece.Top = y;
            }
        }

        public void DrawPuzzles(Image image, int row, int col)
        {
            int index = 0;
            int pieceW = image.Width / col;
            int pieceH = image.Height / row;

            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    PuzzlePiece piece = pieces[index];
                    piece.Left = c * pieceW;
                    piece.Top = r * pieceH;
                    piece.SizeMode = PictureBoxSizeMode.StretchImage;

                    panel1.Controls.Add(piece);
                    index++;
                }
            }
        }

        public PuzzlePiece[] createPuzzles(Image image, int row, int col)
        {
            int pieceW = image.Width / col;
            int pieceH = image.Height / row;

            PuzzlePiece[] puzzles = new PuzzlePiece[row * col];
            Bitmap piece;

            int index = 0;
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; ++c)
                {
                    puzzles[index] = new PuzzlePiece();
                    piece = new Bitmap(pieceW, pieceH);
                    Graphics g = Graphics.FromImage(piece);
                    g.DrawImage(image, new Rectangle(0, 0, pieceW, pieceH),
                                       new Rectangle(c * pieceW, r * pieceH, pieceW, pieceH),
                                       GraphicsUnit.Pixel);
                    puzzles[index].Image = piece;
                    puzzles[index].Width = (int)(pieceW * 0.99f);
                    puzzles[index].Height = (int)(pieceH * 0.99f);
                    puzzles[index].tag = index;

                    index++;
                }
            }
            return puzzles;
        }

        public void drawGrid(Graphics g, int sx, int sy, int row, int col, int pieceW, int pieceH)
        {
            Pen pen = new Pen(Color.Red);
            for (int r = 0; r <= row; ++r)
            {
                g.DrawLine(pen, sx, sy + r * pieceH, sx + col * pieceW, sy + r * pieceH);
            }
            for (int c = 0; c <= col; ++c)
            {
                g.DrawLine(pen, sx + c * pieceW, sy, sx + c * pieceW, sy + row * pieceH);
            }
        }
    }

    public class PuzzlePiece : PictureBox
    {
        public static int released = -1;  // 記錄當前的被釋放的拼圖區塊(piece)。

        public bool dragging = false;  // 是否點按了該拼圖(piece)。
        int prevX, prevY;
        public int tag = -1;  // 記錄拼圖(piece)的編號。

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (tag >= 0)
            {
                dragging = true;
                prevX = e.X;
                prevY = e.Y;

                // child control 會依序加入，先加的數值小，會被畫在上面！
                // 將點選到的piece提升到最上面:
                this.Parent.Controls.SetChildIndex(this, 0);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (dragging)
            {
                int dx = e.X - prevX;
                int dy = e.Y - prevY;
                Left += dx;
                Top += dy;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            dragging = false;  // 只在此設定會造成靠近但尚未釋放滑鼠時，出現來回晃動的現象！

            released = tag;
            this.Parent.Invalidate();  // 讓 parent 啟動重繪事件。
        }
    }
}
