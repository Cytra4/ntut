using System;
using System.Drawing;
using System.Windows.Forms;

namespace Problem2
{
    public partial class Form1 : Form
    {
        private SimpleSprite sprite;
        private Image explosion;

        public Form1()
        {
            InitializeComponent();

            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;
            this.BackColor = Form1.DefaultBackColor;

            sprite = new SimpleSprite(this);
            explosion = Properties.Resources.explosion;

            sprite.setImage(explosion);


            this.MouseClick += Form1_MouseClick;
            this.Paint += Form1_Paint;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (explosion != null)
            {
                sprite.start(e.Location);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            sprite.Draw(e.Graphics);
        }


    }
    class SimpleSprite
    {
        System.Windows.Forms.Timer uiTimer = null;
        Image image = null;

        int spriteCount = 0;
        int totalFrames = 23;
        int cols = 5;

        Point explosionPos;
        Form parentForm;

        public SimpleSprite(Form form)
        {
            parentForm = form;
            uiTimer = new System.Windows.Forms.Timer();
            uiTimer.Interval = 50;
            uiTimer.Tick += UITimer_Tick;
        }

        public void setImage(Image image)
        {
            this.image = image;
        }

        public void start()
        {
            spriteCount = 0;
            uiTimer.Start();
        }

        public void start(Point position)
        {
            explosionPos = position;
            spriteCount = 0;
            uiTimer.Start();
        }

        public void stop()
        {
            uiTimer.Stop();
        }

        int[] index2RC(int index, int col)
        {
            int[] rc = new int[2];
            rc[0] = index / col;
            rc[1] = index % col;
            return rc;
        }

        private void UITimer_Tick(object sender, EventArgs e)
        {
            if (spriteCount < totalFrames)
            {
                spriteCount++;
                parentForm.Invalidate();
            }
            else
            {
                uiTimer.Stop();
                spriteCount = 0;
                parentForm.Invalidate();
            }
        }


        public void Draw(Graphics g)
        {
            int[] rc = index2RC(spriteCount - 1, cols);

            Rectangle srcRect = new Rectangle(rc[1] * 64, rc[0] * 64, 64, 64);

            Rectangle destRect = new Rectangle(explosionPos.X - 64 / 2, explosionPos.Y - 64 / 2, 64, 64);

            g.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
        }
    }


}