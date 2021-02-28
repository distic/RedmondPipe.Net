using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace RedmondPipe.Net
{
    static class Program
    {
        internal static string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var form = new Form
            {
                Text = System.Reflection.Assembly.GetCallingAssembly().FullName,
                Size = new Size(800, 600),
                Icon = SystemIcons.WinLogo,
                IsMdiContainer = true
            };

            var appSettings = Utilities.UserSettingsReader.ReadFromFile();
            
            if (appSettings.PipeClients == null)
            {
                MessageBox.Show(Properties.Resources.No_pipe_clients_were_setup_up__Please_refer_to_documentation_, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (appSettings.PipeClients.Count < 1)
            {
                MessageBox.Show(Properties.Resources.No_pipe_clients_were_setup_up__Please_refer_to_documentation_, Properties.Resources.AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            form.Load += delegate
            {
                var loc = 2;

                foreach (var data in appSettings.PipeClients)
                {
                    var frm = new FrmPipeClient
                    {
                        NamedPipeString = appSettings.NamedPipe,
                        Text = data.Title,
                        ConsoleBackColor = data.ConsoleBackColor,
                        ConsoleTextColor = data.ConsoleTextColor,
                        MdiParent = form,
                        Location = new Point(loc, loc)
                    };

                    frm.Show();

                    loc += 20;
                }
            };

            Application.Run(form);
        }
    }
}
