using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArmRenderer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            DirectoryInfo dir = new DirectoryInfo(ConfigurationSettings.AppSettings["QueueDirectory"].ToString().Trim());
            DirectoryInfo[] chDirs = dir.GetDirectories();
            label2.Text = chDirs.Length.ToString();
            if (chDirs.Length > 0)
            {
                string[] d = chDirs[0].ToString().Split('_');
                string DirPathDest = ConfigurationSettings.AppSettings["OutputPath"].ToString().Trim() + "\\" + d[0] + "\\" + ConfigurationSettings.AppSettings["OutputFolderName"].ToString().Trim();
                if (!Directory.Exists(DirPathDest))
                    Directory.CreateDirectory(DirPathDest);
                if (File.Exists(ConfigurationSettings.AppSettings["QueueDirectory"].ToString().Trim() + "\\" + chDirs[0] + "\\data.xml"))
                {
                    File.Copy(ConfigurationSettings.AppSettings["QueueDirectory"].ToString().Trim() + "\\" + chDirs[0] + "\\1.mp4", ConfigurationSettings.AppSettings["VideoFootage"].ToString().Trim() + "\\1.mp4", true);
                    File.Copy(ConfigurationSettings.AppSettings["QueueDirectory"].ToString().Trim() + "\\" + chDirs[0] + "\\data.xml", ConfigurationSettings.AppSettings["xml"].ToString().Trim(), true);
                    Render(DirPathDest + "\\" + ConfigurationSettings.AppSettings["OutputFilePrefix"].ToString().Trim() + "_" + d[1] + ".mp4");
                    try
                    {

                        foreach (System.IO.FileInfo file in chDirs[0].GetFiles())
                            file.Delete();
                        chDirs[0].Delete();
                    }
                    catch
                    { }
                }
            }
            timer1.Enabled = true;
        }
        protected void Render(string filename)
        {
            timer1.Enabled = false;
            richTextBox1.Text += "Start Render" + " \n";
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
            Application.DoEvents();
            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = "\"" + ConfigurationSettings.AppSettings["AeRenderPath"].ToString().Trim() + "aerender.exe" + "\"";
                string Comp = ConfigurationSettings.AppSettings["AeComposition"].ToString().Trim();
                proc.StartInfo.Arguments = " -project " + "\"" + ConfigurationSettings.AppSettings["AeProjectPath"].ToString().Trim() + "\"" + "   -comp   \"" + Comp + "\" -output " + "\"" + filename + "\"";
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.EnableRaisingEvents = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.Start();
                StreamReader reader = proc.StandardOutput;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (richTextBox1.Lines.Length > 10)
                    {
                        richTextBox1.Text = "";
                    }
                    richTextBox1.Text += (line) + " \n";
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                    Application.DoEvents();
                }
                proc.Close();
            }
            catch (Exception Exp)
            {
                richTextBox1.Text += Exp.Message;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                Application.DoEvents();
            }
            richTextBox1.Text += DateTime.Now.ToString() + " \n";
            richTextBox1.Text += "=======Task Finished======" + " \n";
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
            Application.DoEvents();
            timer1.Enabled = true;
        }
    }
}
