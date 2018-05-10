using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Process
{
    public partial class MainForm : Form
    {
        DataTable table;
        List<Variable> lV = new List<Variable>();
        string[,] matrTran;
        int indexPrev;
        public MainForm()
        {
            InitializeComponent();
            (dgvF.Columns[1] as DataGridViewComboBoxColumn).Items.AddRange(new string[] {
                    "x", "x^2", "x^3", "1/x", "1/x^2", "sqrt(x)", "1/sqrt(x)", "ln(x)", "exp(x)" });
            List<string> lS = new List<string>(new string[] {
                    "S", "C", "P", "Si", "Mg", "Mn", "Cu", "As", "Ni", "Cr", "N", "Mo", "V", "W", "Co"  });
            lS.Sort();
            (dgvEl.Columns[0] as DataGridViewComboBoxColumn).Items.AddRange(lS.ToArray());
            Steel.Initialize();
        }
        void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            tbFile.Text = openFileDialog1.FileName;
        }
        void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                string s = "provider = Microsoft.Jet.OLEDB.4.0;" +
                    "data source = " + tbFile.Text + ";" +
                    "extended properties = Excel 8.0;";
                OleDbConnection conn = new OleDbConnection(s);
                conn.Open();
                s = string.Format("SELECT * FROM [{0}$]", tbSheet.Text);
                OleDbDataAdapter da = new OleDbDataAdapter(s, conn);
                table = new DataTable();
                da.Fill(table);
                conn.Close();
                string[] arrS = new string[table.Columns.Count];
                for (int i = 0; i < table.Columns.Count; i++)
                    arrS[i] = table.Columns[i].Caption;
                lbV.Items.Clear();
                cbD.Items.Clear();
                cbD.Items.AddRange(arrS);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbN.Text == "")
                    return;
                Variable v = new Variable() { id = tbN.Text, name = cbD.Text };
                lbV.Items.Add(v);
                lV.Add(v);
                Data(lV);
            }
            catch { }
        }
        void btnDel_Click(object sender, EventArgs e)
        {
            try
            {
                int index = lbV.SelectedIndex;
                lbV.Items.RemoveAt(index);
                lV.RemoveAt(index);
            }
            catch { }
        }
        void cbD_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbD.SelectedIndex != -1)
                tbN.Text = cbD.Text;
        }
        void saveVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                formatter.Serialize(stream, lV);
                stream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void loadVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(openFileDialog1.FileName, FileMode.Open);
                lV = (List<Variable>)formatter.Deserialize(stream);
                lbV.Items.Clear();
                lbV.Items.AddRange(lV.ToArray());
                stream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        void btnRefresh_Click(object sender, EventArgs e)
        {
            /*lbQSyn.Items.Clear();
            lbQSyn.Items.AddRange(lQ.ToArray());
            indexPrev = 0;
            if (lQ.Count > 0)
                lbQSyn.SelectedIndex = 0;
            dgvF.Rows.Clear();
            foreach (Variable v in lV)
                dgvF.Rows.Add(v.ToString(), "x");
            matrTran = new string[lV.Count, lQ.Count];
            for (int i = 0; i < lV.Count; i++)
                for (int j = 0; j < lQ.Count; j++)
                    matrTran[i, j] = "x";*/
        }
        void lbQSyn_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int index = lbQSyn.SelectedIndex;
                for (int i = 0; i < lV.Count; i++)
                    matrTran[i, indexPrev] = dgvF[1, i].Value.ToString();
                dgvF.Rows.Clear();
                for (int i = 0; i < lV.Count; i++)
                    dgvF.Rows.Add(lV[i].ToString(), matrTran[i, index]);
                indexPrev = index;
            }
            catch { }
        }        
        void btnSyn_Click(object sender, EventArgs e)
        {
            //try
            /*{
                string rep = "";
                Sample[] arrSmp = new Sample[lV.Count];
                for (int i = 0; i < lV.Count; i++)
                    arrSmp[i] = new Sample(lV[i].name, lV[i].id, lV[i].arr);
                tbSyn.Text = "";
                for (int i = 0; i < lQ.Count; i++)
                {
                    Sample[] arrTSmp = new Sample[lV.Count];
                    for (int j = 0; j < lV.Count; j++)
                        arrTSmp[j] = new TranSample(arrSmp[j], matrTran[j, i]);
                    Sample smp = new Sample(lQ[i].name, lQ[i].id, lQ[i].arr);
                    Regression r = new Regression(smp, arrSmp);
                    tbSyn.Text += Formula(r.arrB, smp, arrTSmp) + "\r\n";
                    rep += r.GetRegReport() + "<br>";
                }
                wb.DocumentText = rep;
            }
            //catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }*/
        }
        string Formula(double[] arrB, Sample smp, Sample[] arrTSmp)
        {
            string s = string.Format("{0} = {1:g4}", smp.GetMark(), arrB[0]);
            for (int i = 0; i < arrTSmp.Length; i++)
            {
                if (arrB[i + 1] >= 0)
                    s += "+";
                switch ((arrTSmp[i] as TranSample).GetTransform())
                {
                    case "x^2":
                        s += string.Format("{0:g4}*{1}^2", arrB[i + 1], (arrTSmp[i] as TranSample).GetMark());
                        break;
                    case "x^3":
                        s += string.Format("{0:g4}*{1}^3", arrB[i + 1], (arrTSmp[i] as TranSample).GetMark());
                        break;
                    case "1/x":
                        s += string.Format("{0:g4}/{1}", arrB[i + 1], (arrTSmp[i] as TranSample).GetMark());
                        break;
                    case "1/x^2":
                        s += string.Format("{0:g4}/{1}^2", arrB[i + 1], (arrTSmp[i] as TranSample).GetMark());
                        break;
                    case "sqrt(x)":
                        s += string.Format("{0:g4}*{1}^0.5", arrB[i + 1], (arrTSmp[i] as TranSample).GetMark());
                        break;
                    case "1/sqrt(x)":
                        s += string.Format("{0:g4}/{1}^0.5", arrB[i + 1], (arrTSmp[i] as TranSample).GetMark());
                        break;
                    case "ln(x)":
                        s += string.Format("{0:g4}*log({1})", arrB[i + 1], (arrTSmp[i] as TranSample).GetMark());
                        break;
                    case "exp(x)":
                        s += string.Format("{0:g4}*exp({1})", arrB[i + 1], (arrTSmp[i] as TranSample).GetMark());
                        break;
                    case "x":
                        s += string.Format("{0:g4}*{1}", arrB[i + 1], (arrTSmp[i] as TranSample).GetMark());
                        break;
                }
            }
            s.Replace(',', '.');
            return s;
        }
        void Data(List<Variable> lVar)
        {
            List<int> lIndex = new List<int>();
            double tmp;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                int j;
                for (j = 0; j < lVar.Count; j++)
                    if (!double.TryParse(table.Rows[i][lVar[j].name].ToString(), out tmp))
                        break;
                if (j == lVar.Count)
                    lIndex.Add(i);
            }
            foreach (Variable v in lVar)
            {
                double[] arr = new double[lIndex.Count];
                for (int i = 0; i < arr.Length; i++)
                    arr[i] = double.Parse(table.Rows[lIndex[i]][v.name].ToString());
                v.arr = arr;
                double av = 0, av2 = 0;
                for (int i = 0; i < arr.Length; i++)
                {
                    av += arr[i];
                    av2 += arr[i] * arr[i];
                }
                double sigma = av2 / arr.Length - (av / arr.Length) * (av / arr.Length);
                for (int i = 0; i < arr.Length; i++)
                    arr[i] /= sigma;
            }
        }
        
        void btnCalc_Click(object sender, EventArgs e)
        {
            try
            {
                string report;
                double sT = double.Parse(tbSTOld.Text), E = double.Parse(tbE.Text);
                string[] arrEl = new string[dgvEl.Rows.Count - 1];
                double[] arrC = new double[dgvEl.Rows.Count - 1];
                for (int i = 0; i < dgvEl.Rows.Count - 1; i++)
                {
                    arrEl[i] = dgvEl[0, i].Value.ToString();
                    arrC[i] = double.Parse(dgvEl[1, i].Value.ToString());
                }
                double aT, bT, aP, bP, aH, bH;
                Steel.SetGroup(arrEl, arrC, out aT, out bT, out aP, out bP, out aH, out bH, out report);
                string rep;
                Steel.Calculate(arrEl, arrC, sT, E, "e", "ПТ", E, out rep);
                report += "<br>" + rep;
                Steel.Calculate(arrEl, arrC, sT, E, "e", "ПП", E, out rep);
                report += "<br>" + rep;
                Steel.Calculate(arrEl, arrC, sT, E, "e", "HB", E, out rep);
                report += "<br>" + rep;
                wbS.DocumentText = report;
            }
            catch { }
        }
        void btnGraph_Click(object sender, EventArgs e)
        {
            try
            {
                double sT = double.Parse(tbSTOld.Text), E = double.Parse(tbE.Text);
                string[] arrEl = new string[dgvEl.Rows.Count - 1];
                double[] arrC = new double[dgvEl.Rows.Count - 1];
                for (int i = 0; i < dgvEl.Rows.Count - 1; i++)
                {
                    arrEl[i] = dgvEl[0, i].Value.ToString();
                    arrC[i] = double.Parse(dgvEl[1, i].Value.ToString());
                }

                PForm pf = new PForm(arrEl);
                if (pf.ShowDialog() != DialogResult.OK)
                    return;
                int n = 30;
                double[] arrX = new double[n], arrY = new double[n];
                double step = (pf.max - pf.min) / (n - 1);
                string rep;
                for (int i = 0; i < arrX.Length; i++)
                {
                    arrX[i] = pf.min + step * i;
                    arrY[i] = Steel.Calculate(arrEl, arrC, sT, E, pf.x, pf.y, arrX[i], out rep);
                }
                GForm gf = new GForm(pf.x, pf.y, arrX, arrY);
                gf.Show();
            }
            catch { }
        }        
    }
    [Serializable]
    public class Variable
    {
        public string name, id;
        public double[] arr;
        public override string ToString()
        {
            return string.Format("{0} ({1})", name, id);
        } 
    }
}
