using App1.Properties;
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
        int[] chosenPokers = new int[26];
        PokerCard[] pokerList = new PokerCard[52];
        Random rand = new Random();

        int firstSlot = -1;
        bool selectionLocked = false; 

        public Form1()
        {
            InitializeComponent();
            panel1.MouseClick += panel1_MouseClick;
        }

        public int[] GenerateCardList()
        {
            int[] chosen = Enumerable.Range(0, 52)
                                     .OrderBy(_ => rand.Next())
                                     .Take(26)
                                     .ToArray();

            return chosen;
        }

        private int GetCardAtPoint(Point p)
        {
            if (pokerList == null) return -1;
            for (int i = pokerList.Length - 1; i >= 0; i--)
            {
                var c = pokerList[i];
                if (c == null || !c.enabled) continue;
                int w = 71, h = 96;
                Rectangle r = new Rectangle(c.x, c.y, w, h);
                if (r.Contains(p)) return i;
            }
            return -1;
        }

        private async void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (selectionLocked) return;

            int slot = GetCardAtPoint(e.Location);
            if (slot == -1) return;

            var clicked = pokerList[slot];
            if (clicked == null || !clicked.enabled) return;

            int highestIndex = -1;
            int highestY = int.MaxValue;

            for (int i = 0; i < pokerList.Length; i++)
            {
                var c = pokerList[i];
                if (c == null || !c.enabled) continue;

                if (c.y < highestY)
                {
                    highestY = c.y;
                    highestIndex = i;
                }
            }

            if (highestIndex != -1 && highestIndex != slot)
            {
                var topCard = pokerList[highestIndex];
                (clicked.x, topCard.x) = (topCard.x, clicked.x);
                (clicked.y, topCard.y) = (topCard.y, clicked.y);
            }

            panel1.Invalidate();

            if (firstSlot == -1)
            {
                firstSlot = slot;
                clicked.selected = true;
                return;
            }

            if (slot == firstSlot) return;

            clicked.selected = true;

            var firstCard = pokerList[firstSlot];
            var secondCard = pokerList[slot];

            if (firstCard.getIndex() == secondCard.getIndex())
            {
                firstCard.enabled = false;
                secondCard.enabled = false;
                firstCard.selected = false;
                secondCard.selected = false;
            }
            else
            {
                selectionLocked = true;
                await Task.Delay(700);
                firstCard.selected = false;
                secondCard.selected = false;
                selectionLocked = false;
            }

            firstSlot = -1;
            panel1.Invalidate();
        }



        PokerCard[] CardsInitial(int[] cards)
        {
            PokerCard[] pokers = new PokerCard[52];
            int[] spots = new int[52];
            int pokerW = 71;
            int pokerH = 96;
            
            for (int i = 0; i < 52; i++)
            {
                spots[i] = 100;
            }
            
            for (int i = 0; i < 26; i++)
            {
                int count = 0;
                int tmp;
                int posX;
                int posY;
                int vecX;
                int vecY;
                while (count < 2)
                {
                    tmp = rand.Next(0, 52);
                    if (spots[tmp] == 100)
                    {
                        spots[tmp] = cards[i];
                        posX = rand.Next(pokerW / 2, panel1.Width - pokerW / 2);
                        posY = rand.Next(pokerH / 2, panel1.Height - pokerH / 2);
                        vecX = rand.Next(1, 3);
                        vecY = rand.Next(1, 3);
                        pokers[tmp] = new PokerCard(cards[i], posX, posY);
                        pokers[tmp].setVec(vecX,vecY);
                        count += 1;
                    }
                }
            }
            return pokers;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chosenPokers = GenerateCardList();
            pokerList = CardsInitial(chosenPokers);

            Bitmap buffer = new Bitmap(panel1.Width, panel1.Height);
            long elapsedTime;
            long interval = 100000;

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                          ControlStyles.UserPaint | 
                          ControlStyles.OptimizedDoubleBuffer, true); 
            this.UpdateStyles(); 

            this.DoubleBuffered = true;

            while (checkBox1.Checked)
            {
                elapsedTime = DateTime.Now.Ticks;

                using (Graphics bufferG = Graphics.FromImage(buffer))
                {
                    bufferG.Clear(Color.White);

                    for (int i = 0; i < pokerList.Length; i++)
                    {
                        pokerList[i].update();
                        pokerList[i].collisionCheck(panel1.Width, panel1.Height);
                        pokerList[i].show(bufferG);
                    }
                }

                while (DateTime.Now.Ticks - elapsedTime < interval) ;

                using(Graphics g = panel1.CreateGraphics())
                {
                    g.DrawImage(buffer, 0, 0);
                }

                System.Windows.Forms.Application.DoEvents();
            }
        }

        class PokerCard
        {
            public int tag;
            public bool enabled = true;
            public int x, y;   // position 
            public int[] vec = new int[2];  // velocity 
            public bool flipped = false;
            public bool selected = false;

            Bitmap back;
            Bitmap suit;

            public PokerCard(int index, int px = 0, int py = 0)
            {
                tag = index;
                x = px;
                y = py;
                suit = getPoker(index);
            }

            int[] index2RC(int index, int col)
            {
                int[] rc = new int[2];
                rc[0] = index / col;  // row 
                rc[1] = index % col;  // column 
                return rc;
            }

            public void setPos(int nx, int ny)
            {
                this.x = nx;
                this.y = ny;
            }

            public void setVec(int vx, int vy)
            {
                this.vec[0] = vx;
                this.vec[1] = vy;
            }

            public Bitmap getPoker(int index)  // index: 0 ~ 51. 
            {
                int pokerW = 71;
                int pokerH = 96;
                Image poker = Resources.poker;

                Bitmap bmp = new Bitmap(pokerW, pokerH);

                int[] rc = index2RC(index, 13);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(poker,
                                new Rectangle(0, 0, pokerW, pokerH),
                                new Rectangle(rc[1] * pokerW, rc[0] * pokerH, pokerW, pokerH),
                                GraphicsUnit.Point);
                }
                return bmp;
            }

            public int getIndex()
            {
                return this.tag;
            }

            public void update()
            {
                x += vec[0];
                y += vec[1];
            }

            public void collisionCheck(int boundW, int boundH)
            {
                int pokerW = 71;
                int pokerH = 96;
                bool f = false;

                if (this.x < 0)
                {
                    this.x = 0;
                    this.vec[0] = -this.vec[0];
                    f = true;
                }
                else if (this.x + pokerW > boundW)
                {
                    this.x = boundW - pokerW;
                    this.vec[0] = -this.vec[0];
                    f = true;
                }

                if (this.y < 0)
                {
                    this.y = 0;
                    this.vec[1] = -this.vec[1];
                    f = true;
                }
                else if (this.y + pokerH > boundH)
                {
                    this.y = boundH - pokerH;
                    this.vec[1] = -this.vec[1];
                    f = true;
                }

                if (f)
                {
                    flipped = !flipped;
                    suit = getPoker(flipped ? 55 : tag);
                }
            }

            public void show(Graphics g)
            {
                if (!enabled) return;

                g.DrawImage(suit, x, y);
            }


            public void show(Graphics g, int px, int py)
            {
                if (enabled)
                {
                    g.DrawImage(suit, px, py);
                }
            }
        }
    }
}
