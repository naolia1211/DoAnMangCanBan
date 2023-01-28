using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Do_An_CuoiKy
{
    public partial class Form2 : Form
    {
        public Form1 AssForm;
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AssForm.newname = textBox1.Text;
            if (AssForm.thisname == "CreateFolder")
                AssForm.CreateFolder();
            else if (AssForm.thisname == "RenameFile")
                AssForm.RenameFile();
            Visible = false;
        }
    }
}
