using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClickLoop
{

    public partial class Form1 : Form
    {
        //###################################
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);
        [DllImport("User32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        //###################################
        public static bool running = false;
        public static string version = "1.0.1";
        public static string TextToSend;
        CancellationTokenSource s_cts;
        //####################################
        public Form1()
        {
            InitializeComponent();
            label3.AutoSize = true;
            refresh();
            numericUpDown1.Value = Properties.Settings.Default.loopRotation;
            numericUpDown2.Value = Properties.Settings.Default.keySpeed;
        }
        private void StartLoopButton(object sender, EventArgs e)
        {
            string selectedProcess = ListProcesses.SelectedValue.ToString();
            try
            {
                startLoop(((int)(numericUpDown1.Value*1000)), ((int)(numericUpDown2.Value*1000)), TextToSend, selectedProcess);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Task canceled.");
            }
        }
        private void CancelLoopButton(object sender, EventArgs e)
        {
            running = false;
            s_cts.Cancel();
            button1.Enabled = false;
            button1.BackColor = Color.Gray;
            button2.Enabled = true;
        }
        private async void startLoop(int seconds, int sendingSpeed, string package, string selectedProcess)
        {

            if (selectedProcess != null && package != null)
            {
                button2.Enabled = false;
                s_cts = new CancellationTokenSource();
                running = true;
                button1.Enabled = true;
                button1.BackColor = Color.Red;
                while (running)
                {

                    try
                    {
                        s_cts.Token.ThrowIfCancellationRequested();
                        await Task.Delay(seconds , s_cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        MessageBox.Show("task is Canceled");
                        CancelLoopButton(button1, EventArgs.Empty);
                        break;
                    }                   
                    if (Process.GetProcessesByName(selectedProcess).FirstOrDefault() != null)
                    {
                        ShowWindow(Process.GetProcessesByName(selectedProcess).FirstOrDefault().MainWindowHandle, 1);
                        SetForegroundWindow(Process.GetProcessesByName(selectedProcess).FirstOrDefault().MainWindowHandle);
                    }
                    else
                    {
                        MessageBox.Show($"Process {selectedProcess} was killed,loop aborted !!", "Canceled");
                        CancelLoopButton(button1, EventArgs.Empty);
                        break;
                    }

                    char[] splitter = { ' ' };
                    string[] chain = package.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string i in chain)
                    {

                        if (i == "{ENTER}")
                        {

                            SendKeys.Send(i);
                            await Task.Delay(sendingSpeed);
                        }
                        else if (i == "{BS}")
                        {
                            SendKeys.Send(" ");
                            await Task.Delay(sendingSpeed);
                        }
                        else if (!string.IsNullOrEmpty(i))
                        {
                            foreach (char j in i)
                            {

                                SendKeys.Send(j.ToString());
                                await Task.Delay(sendingSpeed);
                            }
                        }



                    }
                }
            }
            else MessageBox.Show("Please pick a process and type keywords to start looping", "no process selected!!");
        }
        private void textInput_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                TextToSend = textBox.Text;
                TextToSend = TextToSend.Replace(" ", " {BS} ");
                TextToSend = TextToSend.Replace("\r\n", " {ENTER} ");

            }
        }
        private void BuyMeCoffee(object sender, EventArgs e)
        {
            MessageBoxButtons messageBoxButtons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show("You will be redirected to my paypal page paypal.me/aymenajroud", "Redirecting", messageBoxButtons);

            if (result == DialogResult.Yes) Process.Start("https://www.paypal.com/paypalme/aymenajroud");
        }
        private void supportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"How to use me:
-Find ur desired process name from the list ,if u can't find it then refresh and try again.
-Set the loop rotation, default is to send the keys every 20 seconds.
-Set up the speed of typing,default is 1 second per key
-For sending enter key,just type enter,all characters are allowed,including spaces,more will be worked on.
-This program is ideal for spamming in a global chat or doing something repetitif.
-please don't hesitate to send me feedback ajroudaymen@gmail.com.", "Tutorial and Tips");
        }
        private void versionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messageBoxButtons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show("Version " + version + " CopyRights reserved\r\nClick yes to check for new version", "Software Version", messageBoxButtons);
            if (result == DialogResult.Yes) Process.Start("https://portfoliodeaymen.social/Projects.html");
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void refreshButton(object sender, EventArgs e)
        {
            refresh();

        }
        private void contactMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messageBoxButtons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show("Open default mailbox ?", "Contact me", messageBoxButtons);
            if (result == DialogResult.Yes) Process.Start("mailto:ajroudaymen@gmail.com?subject=ClickLoopContactMe");

        }
        private void refresh()
        {
            List<Process> ProcessList = new List<Process>();
            List<string> processNames = new List<string>();
            ProcessList.AddRange(Process.GetProcesses().Where(p => p.Id > 4 & p.MainWindowHandle != IntPtr.Zero));

            foreach (Process process in ProcessList)
            {
                processNames.Add(process.ProcessName);
            }
            processNames.Sort();
            ListProcesses.DataSource = processNames;

        }
        private void ListProcessesComboBox_ValueChanged(object sender, EventArgs e)
        {

            string ProcInfo = ((ComboBox)sender).Text;
            try
            {
                label3.Text = "Description:\r\n" + Process.GetProcessesByName(ProcInfo).FirstOrDefault().MainModule.FileVersionInfo.FileDescription;
                pictureBox1.Image = Icon.ExtractAssociatedIcon(Process.GetProcessesByName(ProcInfo).FirstOrDefault().MainModule.FileName).ToBitmap();
                toolTip1.SetToolTip(label3, string.Empty);
            }
            catch (AccessViolationException)
            {
                label3.Text = "Description : access denied";
                pictureBox1.Image = null;
                //MessageBox.Show("Sorry,You don't have right to access this process,please make sure u are in administrator mode for this process to work");
            }
            catch (Exception ex)
            {
                label3.Text = "Description : "+ex.Message;
                toolTip1.SetToolTip(label3, "try running the application as administrator");
                pictureBox1.Image = null;
                //MessageBox.Show(ex.Message, "error");
            }
        }
        private void numericUpDown2_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.keySpeed = ((NumericUpDown)sender).Value;
            Properties.Settings.Default.Save();
        }

        private void numericUpDown1_Validated(object sender, EventArgs e)
        {
            Properties.Settings.Default.loopRotation = ((NumericUpDown)sender).Value;
            Properties.Settings.Default.Save();
        }
    }
}
