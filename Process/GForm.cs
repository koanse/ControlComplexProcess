using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Process
{
    public partial class GForm : Form
    {
        public GForm(string x, string y, double[] arrX, double[] arrY)
        {
            InitializeComponent();
            zgc.GraphPane.Title.Text = string.Format("Зависимость {0}({1})", y, x);
            zgc.GraphPane.AddCurve(string.Format("{0}({1})", y, x), arrX, arrY, Color.Red);
            zgc.GraphPane.XAxis.Title.Text = x;
            zgc.GraphPane.YAxis.Title.Text = y;
            zgc.AxisChange();
            Text = string.Format("Зависимость {0}({1})", y, x);
        }
        void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
