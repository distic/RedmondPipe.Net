using RedmondPipe.Interfaces;
using RedmondPipe.Interop;
using RedmondPipe.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using System.Windows.Forms;

namespace RedmondPipe.Net.Forms
{
    public partial class FrmPipeClient : Form, IPipeCustomEvents
    {
        #region Properties

        public PipeClientModel Model { get; set; }

        private Timer _timer;

        private bool _peekFoundData;

        private const string _defaultNamedPipeString = "mynamedpipe";

        private NamedPipeClientStream pipeClient;

        public string NamedPipeString { get; set; }

        private bool IsConnected
        {
            get
            {
                return pipeClient != null ? pipeClient.IsConnected : false;
            }

            set
            {
                miOptions_Connect.Visible = value;
                miOptions_Connect.Enabled = value;

                miOptions_Disconnect.Visible = !value;
                miOptions_Disconnect.Enabled = !value;

                miOptions_Peek.Enabled = !value;

                textBox1.ReadOnly = !value;
                btnSend.Enabled = !value;

                if (value == false)
                {
                    pipeClient.Close();

                    if (Model.AutoPeek)
                    {
                        _timer.Stop();
                    }

                    _peekFoundData = false;
                }
                else
                {
                    if (Model.AutoPeek)
                    {
                        _timer.Start();
                    }
                }
            }
        }

        #endregion

        #region Constructor(s)

        public FrmPipeClient()
        {
            InitializeComponent();
        }

        #endregion

        #region --- Main Menu ---

        #region File

        private void miFile_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Options

        private void miOptions_Connect_Click(object sender, EventArgs e)
        {
            DoConnect();
        }

        private void miOptions_Peek_Click(object sender, EventArgs e)
        {
            byte[] aPeekBuffer = new byte[1];
            uint aPeekedBytes = 0;
            uint aAvailBytes = 0;
            uint aLeftBytes = 0;

            bool aPeekedSuccess = NativeFunctions.PeekNamedPipe(
                pipeClient.SafePipeHandle,
                aPeekBuffer, 1,
                ref aPeekedBytes, ref aAvailBytes, ref aLeftBytes);

            if (aPeekedSuccess && aPeekBuffer[0] != 0)
            {
                _peekFoundData = true;
                toolStripStatusLabel1.Text = $"Byte(s) available: {aAvailBytes}";
            }
            else
            {
                _peekFoundData = false;
                toolStripStatusLabel1.Text = $"Byte(s) not available!";
            }
        }

        private void miOptions_Disconnect_Click(object sender, EventArgs e)
        {
            DoDisconnect();
        }

        #endregion

        #region Help

        private void miHelp_AboutThisProduct_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"{Application.ProductName}\n{Properties.Resources.Version}: {Application.ProductVersion}\n\n{Program.AssemblyCopyright}", Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void miHelp_ViewOnGitHub_Click(object sender, EventArgs e)
        {
            Process.Start(Properties.Resources.GitHubUrl);
        }

        #endregion

        #endregion

        #region --- Form Events ---

        private void FrmPipeClient_Load(object sender, EventArgs e)
        {
            Text = Model.Title;

            if (Model.AutoPeek)
            {
                _timer = new Timer
                {
                    Interval = 1000
                };

                _timer.Tick += _timer_Tick;
            }

            textBox2.ForeColor = Model.ConsoleTextColor;
            textBox2.BackColor = Model.ConsoleBackColor;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Peeking...";
            miOptions_Peek.PerformClick();

            if (_peekFoundData)
            {
                byte[] buffer = new byte[1024];
                int sz = pipeClient.Read(buffer, 0, 1024);

                OnMessageReceived(Encoding.ASCII.GetString(buffer), sz);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend.PerformClick();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            btnSend.Enabled = textBox1.Text.Length > 0;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(textBox1.Text);

            // Send the message to the server
            pipeClient.Write(messageBytes, 0, messageBytes.Length);

            textBox2.AppendText($"[Send ({messageBytes.Length})]  {textBox1.Text}");
            textBox2.AppendText(Environment.NewLine);

            textBox1.Text = string.Empty;
        }

        private void FrmClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (pipeClient != null)
            {
                IsConnected = false;
            }
        }

        #endregion

        #region Implementation for IPipeCustomEvents

        public void OnMessageReceived(string message, int size)
        {
            textBox2.AppendText($"[Recv ({size})]  {message}");
            textBox2.AppendText(Environment.NewLine);
            textBox2.AppendText(Environment.NewLine);
        }

        #endregion

        private void DoConnect()
        {
            if (IsConnected)
            {
                MessageBox.Show("Already connected!");
                return;
            }

            if (string.IsNullOrEmpty(NamedPipeString))
            {
                MessageBox.Show(string.Format(Properties.Resources.NamedPipeStringWasEmpty, _defaultNamedPipeString), Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                NamedPipeString = _defaultNamedPipeString;
            }

            pipeClient = new NamedPipeClientStream(".", NamedPipeString, PipeDirection.InOut);
            pipeClient.Connect();

            IsConnected = true;
        }

        private void DoDisconnect()
        {
            IsConnected = false;
        }
    }
}