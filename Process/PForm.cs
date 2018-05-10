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
    public partial class PForm : Form
    {
        public double min, max;
        public string x, y;
        public PForm(string[] arrEl)
        {
            InitializeComponent();
            List<string> l = new List<string>(arrEl);
            l.Add("ПТисх"); l.Add("e");
            comboBox1.Items.AddRange(l.ToArray());
            comboBox2.Items.AddRange(new string[] { "ПТ", "ПП", "HB" });
        }
        void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        void button1_Click(object sender, EventArgs e)
        {
            try
            {
                min = double.Parse(textBox1.Text);
                max = double.Parse(textBox2.Text);
                x = comboBox1.Text;
                y = comboBox2.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch { }
        }
    }
}
