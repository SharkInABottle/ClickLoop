using ClickLoop.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClickLoop
{
    public partial class MainForm : Form
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
        private readonly ProcessService _processService;
        private readonly LoopService _loopService;
        CancellationTokenSource s_cts;
        private IList<string> _runningProcesses;
        //####################################
        public MainForm()
        {
            _loopService = new LoopService();
            _processService = new ProcessService();
            InitializeComponent();
            label3.AutoSize = true;
            refresh();
            numericUpDown1.Value = Properties.Settings.Default.loopRotation;
            numericUpDown2.Value = Properties.Settings.Default.keySpeed;
        }
        private async void StartLoopButton(object sender, EventArgs e)
        {
            string selectedProcess = ListProcesses.SelectedValue?.ToString();
            if (string.IsNullOrEmpty(selectedProcess) || string.IsNullOrEmpty(TextToSend))
            {
                MessageBox.Show("no matching process found", "error");
                return;
            }

            int interval = (int)(numericUpDown1.Value * 1000);
            int speed = (int)(numericUpDown2.Value * 1000);
            UpdateUIForLoop(true);
            await _loopService.StartLoopAsync(interval, speed, TextToSend, _processService.GetProcessByName(selectedProcess),
                onCancel: () => UpdateUIForLoop(false),
                onError: message => MessageBox.Show(message, "error"));
        }
        private void CancelLoopButton(object sender, EventArgs e)
        {
            _loopService.CancelLoop();
            UpdateUIForLoop(false);
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
                        await Task.Delay(seconds, s_cts.Token);
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
            if (result == DialogResult.Yes) Process.Start("https://ajroudsoftwares.tn");
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
            if (result == DialogResult.Yes) Process.Start("mailto:contact@ajroudsoftwares.tn?subject=ClickLoopContactMe");

        }
        private void refresh()
        {
            _runningProcesses = _processService.GetRunningProcesses();
            ListProcesses.DataSource = _runningProcesses;

        }
        private void ListProcessesComboBox_ValueChanged(object sender, EventArgs e)
        {

            string ProcInfo = ((ComboBox)sender).Text;
            try
            {
                var process = _processService.GetProcessByName(ProcInfo);
                label3.Text = "Description:\r\n" + process.MainModule.FileVersionInfo.FileDescription;
                pictureBox1.Image = Icon.ExtractAssociatedIcon(process.MainModule.FileName).ToBitmap();
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
                label3.Text = "Description : " + ex.Message;
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
        private void UpdateUIForLoop(bool isRunning)
        {
            running = isRunning;
            button1.Enabled = isRunning;
            button1.BackColor = isRunning ? Color.Red : Color.Gray;
            button2.Enabled = !isRunning;
        }
    }
}
