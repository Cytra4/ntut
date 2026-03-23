using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Problem1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Bitmap GetNameInDots(String name)
        {

            Bitmap dotImage = Properties.Resources.png_1089;

            int imageSize = 18;
            int spacing = 18;

            int width = 1800;
            int height = 450;
            Bitmap textBmp = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(textBmp))
            {
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                Font font = new Font("Arial", 250, FontStyle.Bold);
                Brush brush = new SolidBrush(Color.Black);
                g.DrawString(name, font, brush, 20, 20);
            }

            Bitmap dotsBmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(dotsBmp))
            {
                g.Clear(Color.White);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                for (int y = 0; y < textBmp.Height; y += spacing)
                {
                    for (int x = 0; x < textBmp.Width; x += spacing)
                    {
                        int checkX = x + spacing / 2;
                        int checkY = y + spacing / 2;

                        if (checkX < textBmp.Width && checkY < textBmp.Height)
                        {
                            Color pixelColor = textBmp.GetPixel(checkX, checkY);

                            if (pixelColor.R < 128)
                            {
                                g.DrawImage(dotImage, x, y, imageSize, imageSize);
                            }
                        }
                    }
                }
            }

            textBmp.Dispose();

            return dotsBmp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap result = GetNameInDots("謝政軒");
            if (result != null)
            {
                PictureBox pictureBox = new PictureBox();
                pictureBox.Image = result;
                pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
                pictureBox.Location = new Point(10, 10);
                this.Controls.Add(pictureBox);
            }
        }
    }
}