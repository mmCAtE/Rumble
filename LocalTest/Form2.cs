using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LocalTest
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //Form1 f1 = new Form1();
            //f1.TopLevel = false;
            //f1.Parent = panel1;
            //f1.ControlBox = false;
            //f1.Dock = DockStyle.Fill;
            //f1.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.TopLevel = false;
            f1.Parent = panel2;
            f1.ControlBox = false;
            f1.Dock = DockStyle.Fill;
            f1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 f1 = new Form2();
            f1.TopLevel = false;
            f1.Parent = panel2;
            f1.ControlBox = false;
            f1.Dock = DockStyle.Fill;
            f1.Show();
        }
    }
}
