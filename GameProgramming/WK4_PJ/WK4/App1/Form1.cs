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
        System.Drawing.Color[] colors = 
            { System.Drawing.Color.Red, System.Drawing.Color.Green, System.Drawing.Color.Blue };
        int idx = 0;

        public Form1()
        {
            InitializeComponent();
            this.Text = "資工三 112590019 謝政軒";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String result = Generate9x9();
            label1.Text = result;
            label1.ForeColor = colors[idx];
            idx++;
            if (idx == 3)
            {
                idx = 0;
            }
        }

        String Generate9x9()
        {
            String result = "九九乘法表\n";
            for (int i = 1; i < 10; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    if (j == 1)
                    {
                        result += $"{i}x{j}={i * j} ";
                    }
                    else
                    {
                        result += $"{i}x{j}={i * j,2} ";
                    }
                }
                result += "\n";
            }
            return result;
        }
    }
}
