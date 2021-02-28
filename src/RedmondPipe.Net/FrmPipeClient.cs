using RedmondPipe.Interop;
using RedmondPipe.Utilities;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO.Pipes;
using System.Text;
using System.Windows.Forms;

namespace RedmondPipe.Net
{
    public partial class FrmPipeClient : Form
    {
        #region Properties

        private const string _defaultNamedPipeString = "mynamedpipe";

        private NamedPipeClientStream pipeClient;

        public string NamedPipeString { get; set; }

        public Color ConsoleBackColor
        {
            get
            {
                return textBox2.BackColor;
            }

            set
            {
                textBox2.BackColor = value;
            }
        }

        public Color ConsoleTextColor
        {
            get
            {
                return textBox2.ForeColor;
            }

            set
            {
                textBox2.ForeColor = value;
            }
        }

        private bool IsPipeConnected
        {
            get
            {
                return pipeClient == null ? false : pipeClient.IsConnected;
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
                MessageBox.Show($"Byte(s) available: {aAvailBytes}");
            }
            else
            {
                MessageBox.Show($"Byte(s) not available!");
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

        private void btnSend_Click(object sender, EventArgs e)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(textBox1.Text);

            // Send the message to the server
            pipeClient.Write(messageBytes, 0, messageBytes.Length);
        }

        private void FrmClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (pipeClient != null)
            {
                pipeClient.Close();
            }
        }

        #endregion

        private void DoConnect()
        {
            if (string.IsNullOrEmpty(NamedPipeString))
            {
                MessageBox.Show(string.Format(Properties.Resources.NamedPipeStringWasEmpty, _defaultNamedPipeString), Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                NamedPipeString = _defaultNamedPipeString;
            }

            pipeClient = new NamedPipeClientStream(".", NamedPipeString, PipeDirection.InOut);
            pipeClient.Connect();

            miOptions_Connect.Visible = false;
            miOptions_Connect.Enabled = false;

            miOptions_Disconnect.Visible = true;
            miOptions_Disconnect.Enabled = true;

            miOptions_Peek.Enabled = true;

            textBox1.ReadOnly = true;
            btnSend.Enabled = true;
        }

        private void DoDisconnect()
        {
            miOptions_Connect.Visible = true;
            miOptions_Connect.Enabled = true;

            miOptions_Disconnect.Visible = false;
            miOptions_Disconnect.Enabled = false;

            miOptions_Peek.Enabled = false;

            textBox1.ReadOnly = false;
            btnSend.Enabled = false;
        }
    }
}