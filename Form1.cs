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
using System.Collections;
using System.Net;

namespace Do_An_CuoiKy
{
    public partial class Form1 : Form
    {
        private const string STR_Constant = "\r\n";
        public NetworkStream NetStr;
        public FormLogin LogonForm = new FormLogin();
        public Form2 Form_2 = new Form2();
        public string RemotePath = "";
        public string Server = "";
        public string usname = "";
        public string pass = "";
        public string newname = "";
        public string thisname = "";
            // public string[] path = null;
       // public event EventHandler DoubleClick;

     

        public string custPath { get; private set; }

        public Form1()
        {
            InitializeComponent();
            

             

        }

        public static int remote = -2;


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

       
        
        public void DemonstrateRenameDir(string originalName, string newName)
        {
            
         }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            LogonForm.AssForm = this;
            LogonForm.Visible = true;
        }

        public string sendFTP(string cmd)
        {
            byte[] szData;
            string returnedData = "";
            StreamReader RdStrm = new StreamReader(NetStr);
            szData = Encoding.ASCII.GetBytes(cmd.ToCharArray());
            NetStr.Write(szData, 0, szData.Length);
            txtStatus.Text += "\r\nSent:" + cmd;
            returnedData = RdStrm.ReadLine();
            txtStatus.Text += "\r\nRcvd:" + returnedData;
            return returnedData;
        }


        public bool getRemoteFolders()
        {
            
            string[] filesAndFolders;
            string fileOrFolder;
            string folderList = "";
            int lastSpace = 0;
            folderList = Encoding.ASCII.GetString(sendPassiveFTPcmd("LIST\r\n"));
            lbFile.Items.Clear();
            lbFolder.Items.Clear();
            filesAndFolders = folderList.Split("\n".ToCharArray());
            for (int i = 0; i < filesAndFolders.GetUpperBound(0); i++)
            {
                if (filesAndFolders[i].StartsWith("-") || filesAndFolders[i].StartsWith("d"))
                {
                    lastSpace = 56;
                }
                else
                {
                    lastSpace = 39;
                }
                fileOrFolder = filesAndFolders[i].Substring(lastSpace);
             
                if (fileOrFolder.IndexOf(".") != -1)
                {
                    

                    lbFile.Items.Add(fileOrFolder.Trim());

                                    

                }
                else
                {
                    lbFolder.Items.Add(fileOrFolder.Trim());
                }
            }


            return true;
           
        }

        public byte[] sendPassiveFTPcmd(string cmd)
        {
            byte[] szData;
            System.Collections.ArrayList al = new ArrayList();
            byte[] RecvBytes = new byte[Byte.MaxValue];
            Int32 bytes;
            Int32 totalLength = 0;
            szData = System.Text.Encoding.ASCII.GetBytes(cmd.ToCharArray());
            NetworkStream passiveConnection;
            passiveConnection = createPassiveConnection();
            txtStatus.Text += "\r\nSent:" + cmd;
            StreamReader commandStream = new StreamReader(NetStr);
            NetStr.Write(szData, 0, szData.Length);
            while (true)
            {
                bytes = passiveConnection.Read(RecvBytes, 0, RecvBytes.Length);
                if (bytes <= 0) break;
                totalLength += bytes;
                al.AddRange(RecvBytes);
            }
            al = al.GetRange(0, totalLength);
            txtStatus.Text += "\r\nRcvd:" + commandStream.ReadLine();
            txtStatus.Text += "\r\nRcvd:" + commandStream.ReadLine();
            return (byte[])al.ToArray((new byte()).GetType());
        }
        private NetworkStream createPassiveConnection()
        {
            string[] commaSeperatedValues;
            int highByte = 0;
            int lowByte = 0;
            int passivePort = 0;
            string response = "";
            TcpClient clientSocket;
            NetworkStream pasvStrm = null;
            response = sendFTP("PASV\r\n");
            commaSeperatedValues = response.Split(",".ToCharArray());
            highByte = Convert.ToInt16(commaSeperatedValues[4]) * 256;
            commaSeperatedValues[5] = commaSeperatedValues[5].Substring(0, commaSeperatedValues[5].IndexOf(")"));
            lowByte = Convert.ToInt16(commaSeperatedValues[5]);
            passivePort = lowByte + highByte;
            clientSocket = new TcpClient(Server, passivePort);
            pasvStrm = clientSocket.GetStream();
            return pasvStrm;
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            NetworkStream passiveConnection;
            FileInfo fileParse = new FileInfo(openFileDialog.FileName);

            FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open);
            byte[] fileData = new byte[fs.Length];
            fs.Read(fileData, 0, (int)fs.Length);
            passiveConnection = createPassiveConnection();
            string cmd = "STOR " + fileParse.Name + "\r\n";

            txtStatus.Text += "\r\nSent:" + cmd;
            string response = sendFTP(cmd);
            txtStatus.Text += "\r\nRcvd:" + response;
            passiveConnection.Write(fileData, 0, (int)fs.Length);
            passiveConnection.Close();
            MessageBox.Show("Uploaded");
            txtStatus.Text += "\r\nRcvd:" + new StreamReader(NetStr).ReadLine();
            getRemoteFolders();



        }



        private void lbFolder_DoubleClick(object sender, EventArgs e)
        {
            RemotePath += "/" + lbFolder.SelectedItem.ToString();
            sendFTP("CWD /" + RemotePath + STR_Constant);
            getRemoteFolders();

        }

        private void btnRoot_Click(object sender, EventArgs e)
        {
            RemotePath = "/";
            sendFTP("CWD /\r\n");
            remote = 0;
            getRemoteFolders();
           
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {

            byte[] fileData;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.ShowDialog();
            fileData = sendPassiveFTPcmd("RETR " + lbFile.SelectedItem.ToString() + "\r\n");

            FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.CreateNew);

            fs.Write(fileData, 0, fileData.Length);
            fs.Close();
            MessageBox.Show("Downloaded");
        }

        private void btnCreateFolder_Click(object sender, EventArgs e)
        {
            thisname = "CreateFolder";
            Form_2.AssForm = this;
            Form_2.Visible = true;
            
        }
        public void CreateFolder()
        {
            string filepath;
            DirectoryInfo dirInfo;
            WebRequest request;

            if (lbFolder.SelectedIndex != -1)
            {
                filepath = lbFolder.Items[lbFolder.SelectedIndex].ToString();
                dirInfo = new DirectoryInfo(filepath);
                request = WebRequest.Create("ftp://" + Server + RemotePath + "/" + filepath + "/" + newname);
            }
            else
            {
                dirInfo = new DirectoryInfo(Server);
                request = WebRequest.Create("ftp://" + Server + "/" + newname);
            }           
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(usname, pass);
                using (var resp = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine(resp.StatusCode);
                }
                MessageBox.Show("Đã tạo folder");

                getRemoteFolders();
            
            
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            RemotePath = RemotePath.Substring(0, RemotePath.LastIndexOf("/"));
            sendFTP("CWD ../\r\n");
            remote = remote - 2;
            getRemoteFolders();         
        }


        private void btnDeleteFile_Click(object sender, EventArgs e)
        {
            if (lbFile.SelectedIndex != -1)
            {
                string filepath = lbFile.Items[lbFile.SelectedIndex].ToString();

                FileInfo fileInfo = new FileInfo(filepath);

                fileInfo.Delete();
                string cmd = "STOR " + fileInfo.Name + "\r\n";
                string response = sendFTP(cmd);
                txtStatus.Text += "\r\nRcvd:" + response;
                txtStatus.Text += "\r\nRcvd:" + new StreamReader(NetStr).ReadLine();
                getRemoteFolders();

            }
        }
    

        private void btnRenameFolder_Click(object sender, EventArgs e)
        {
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void lbFolder_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {


            thisname = "RenameFile";
            Form_2.AssForm = this;
            Form_2.Visible = true;

        }

        public void RenameFile()
        {
            if (lbFile.SelectedIndex != -1)
            {

                FtpWebRequest reqFTP;
                

                string filepath = lbFile.Items[lbFile.SelectedIndex].ToString();


                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + Server + RemotePath + "/" + filepath));
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.RenameTo = newname;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(usname, pass);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();

                getRemoteFolders();
            }
        }


        public void RenameFolder()
        {

        }

        private void txtStatus_TextChanged(object sender, EventArgs e)
        {

        }

        void DeleteFtpDirectory(string url, NetworkCredential credentials)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Ftp.RemoveDirectory;
            request.Credentials = credentials;
            request.GetResponse().Close();
            getRemoteFolders();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string filepath = lbFolder.Items[lbFolder.SelectedIndex].ToString();
            string url = "ftp://" + Server + RemotePath + "/" + filepath;
            NetworkCredential credentials = new NetworkCredential(usname, pass);
            DeleteFtpDirectory(url, credentials);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

       
    }
}
