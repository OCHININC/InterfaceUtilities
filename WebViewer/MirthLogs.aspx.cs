using org.ochin.interoperability.OCHINInterfaceUtilities.Mirth;
using org.ochin.interoperability.OCHINInterfaceUtilities.Utilities;
using System;
using System.Configuration;
using System.Web.UI;

namespace org.ochin.interoperability.OCHINInterfaceUtilities
{
    public partial class MirthLogs : Page
    {
        private static string LogServer;

        private static MirthSSHVM MirthSSHVM;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReadConfig();
            }
        }

        private void ReadConfig()
        {
            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                string[] k = key.Split('|');
                if ((k.Length > 1) && (k[0].ToUpper() == "MIRTHLOGS"))
                {
                    switch (k[1].ToUpper())
                    {
                        case "SERVER":
                            LogServer = ConfigurationManager.AppSettings[key];
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string[] ipPort = LogServer.Split(':');
            string remoteHost = ipPort[0];

            int port = 22;
            if (ipPort.Length > 1)
            {
                port = int.Parse(ipPort[1]);
            }

            MirthSSHVM = new MirthSSHVM(tbUsername.Text, tbPassword.Text, remoteHost, port);

            string statusMsg = string.Empty;
            if ((MirthSSHVM != null) && (MirthSSHVM.Connect(out statusMsg)))
            {
                btnLogin.Enabled = false;
                btnLogout.Enabled = true;
                btnSearch.Enabled = true;
            }

            lblStatusMsg.Text = statusMsg;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            if (MirthSSHVM != null)
            {
                if (MirthSSHVM.Disconnect(out string statusMsg))
                {
                    btnLogin.Enabled = true;
                    btnLogout.Enabled = false;
                    btnSearch.Enabled = false;
                }

                lblStatusMsg.Text = statusMsg;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (MirthSSHVM != null)
            {
                _ = MirthSSHVM.SearchPrdLogs(tbSearchText.Text, tbSearchFiles.Value, cbIgnoreCase.Checked, cbRegEx.Checked, out string result, out string cmdExecuted);
                tbSearchResults.Text = result;
                lblSearchCmd.Text = cmdExecuted;
            }
        }
    }
}