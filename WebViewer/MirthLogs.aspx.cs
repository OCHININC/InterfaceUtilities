using org.ochin.interoperability.OCHINInterfaceUtilities.Mirth;
using System;
using System.Configuration;
using System.Web.UI;

namespace org.ochin.interoperability.OCHINInterfaceUtilities
{
    public partial class MirthLogs : Page
    {
        private static readonly string SessionKey_MirthLogs_Server = "MirthLogs_Server";
        private static readonly string SessionKey_MirthLogs_LoggedInUser = "MirthLogs_LoggedInUser";
        private static readonly string SessionKey_MirthLogs_MirthSSHVM = "MirthLogs_MirthSSHVM";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[SessionKey_MirthLogs_Server] == null)
            {
                ReadConfig();
            }

            string loggedInUser = (string)Session[SessionKey_MirthLogs_LoggedInUser];

            if (!string.IsNullOrEmpty(loggedInUser))
            {
                UserLoggedIn();
            }
            else
            {
                UserLoggedOut();
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
                            Session[SessionKey_MirthLogs_Server] = ConfigurationManager.AppSettings[key];
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string mirthLogsServer = (string)Session[SessionKey_MirthLogs_Server];
            string[] ipPort = mirthLogsServer.Split(':');
            string remoteHost = ipPort[0];

            int port = 22;
            if (ipPort.Length > 1)
            {
                port = int.Parse(ipPort[1]);
            }

            MirthSSHVM vm = new MirthSSHVM(tbUsername.Text, tbPassword.Text, remoteHost, port);

            string statusMsg = string.Empty;
            if ((vm != null) && (vm.Connect(out statusMsg)))
            {
                UserLoggedIn();

                Session[SessionKey_MirthLogs_MirthSSHVM] = vm;
                Session[SessionKey_MirthLogs_LoggedInUser] = tbUsername.Text;
            }

            lblStatusMsg.Text = statusMsg;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            MirthSSHVM vm = (MirthSSHVM)Session[SessionKey_MirthLogs_MirthSSHVM];
            if (vm != null)
            {
                if (vm.Disconnect(out string statusMsg))
                {
                    UserLoggedOut();

                    Session[SessionKey_MirthLogs_MirthSSHVM] = null;
                    Session[SessionKey_MirthLogs_LoggedInUser] = null;
                }

                lblStatusMsg.Text = statusMsg;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            MirthSSHVM vm = (MirthSSHVM)Session[SessionKey_MirthLogs_MirthSSHVM];
            if (vm != null)
            {
                _ = vm.SearchPrdLogs(tbSearchText.Text, tbSearchFiles.Value, cbIgnoreCase.Checked, cbRegEx.Checked, out string result, out string cmdExecuted);
                tbSearchResults.Text = result;
                lblSearchCmd.Text = cmdExecuted;
            }
        }

        private void UserLoggedIn()
        {
            btnLogin.Enabled = false;
            btnLogout.Enabled = true;
            btnSearch.Enabled = true;
        }

        private void UserLoggedOut()
        {
            btnLogin.Enabled = true;
            btnLogout.Enabled = false;
            btnSearch.Enabled = false;
        }
    }
}