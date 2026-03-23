using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            this.ClientSize = new Size(500, 500);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Graphics g = this.CreateGraphics();
            g.Clear(Color.Black);
            Pen sin = new Pen(Color.Red,3);
            Pen cos = new Pen(Color.Yellow,3);
            Pen x_line = new Pen(Color.Cyan,3);
            g.DrawLine(x_line, 0, 250, 500, 250);

            float xScale = 500f / (float)(2 * Math.PI);
            float yScale = 150f;

            PointF prevSin = new PointF(0, 250 - (float)(Math.Sin(0) * yScale));
            PointF prevCos = new PointF(0, 250 - (float)(Math.Cos(0) * yScale));

            for (float x = 0; x <= 2 * Math.PI; x += 0.01f)
            {
                float px = x * xScale;
                float pySin = 250 - (float)(Math.Sin(x) * yScale);
                float pyCos = 250 - (float)(Math.Cos(x) * yScale);

                PointF sinPoint = new PointF(px, pySin);
                PointF cosPoint = new PointF(px, pyCos);

                g.DrawLine(sin, prevSin, sinPoint);
                g.DrawLine(cos, prevCos, cosPoint);

                prevSin = sinPoint;
                prevCos = cosPoint;
            }
        }
    }
}
