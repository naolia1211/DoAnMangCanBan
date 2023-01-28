using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;


namespace Do_An_CuoiKy
{   
    public partial class FormLogin : Form
    {
        private const string STR_Constant = "\r\n";
        public Form1 AssForm;
        public FormLogin()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void FormLogin_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            TcpClient ClientSocket = new TcpClient(txtServer.Text, 21);
            AssForm.NetStr = ClientSocket.GetStream();
            StreamReader RdStrm = new StreamReader(AssForm.NetStr);
            txtStatus.Text = RdStrm.ReadLine();
            txtStatus.Text = AssForm.sendFTP("USER " + txtUsename.Text + STR_Constant);
            txtStatus.Text = AssForm.sendFTP("PASS " + txtPassword.Text + STR_Constant);

            if (txtStatus.Text.Substring(0, 3) != "230")
            {
                MessageBox.Show("Failed to log in");
            }
            else
            {
                AssForm.usname = txtUsename.Text;
                AssForm.pass = txtPassword.Text;
                AssForm.Server = txtServer.Text;
                AssForm.getRemoteFolders();
                AssForm.Text += "[logged in]";
                Visible = false;
            }
        }

        private void txtUsename_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
