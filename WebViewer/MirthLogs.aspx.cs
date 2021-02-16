using org.ochin.interoperability.OCHINInterfaceUtilities.Mirth;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace org.ochin.interoperability.OCHINInterfaceUtilities
{
    public partial class MirthLogs : Page
    {
        private static readonly string SessionKey_MirthLogs_Server = "MirthLogs_Server";
        private static readonly string SessionKey_MirthLogs_Environments = "MirthLogs_Environments";
        private static readonly string SessionKey_MirthLogs_Locations = "MirthLogs_Locations";
        private static readonly string SessionKey_MirthLogs_LoggedInUser = "MirthLogs_LoggedInUser";
        private static readonly string SessionKey_MirthLogs_MirthSSHVM = "MirthLogs_MirthSSHVM";

        protected void Page_Load(object sender, EventArgs e)
        {
            ClearStatusMessages();

            if (Session[SessionKey_MirthLogs_Server] == null)
            {
                ReadConfig();
            }

            if (!IsPostBack)
            {
                PopulateEnvironments();

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
        }

        private void ReadConfig()
        {
            Dictionary<string, string> environments = new Dictionary<string, string>();
            Dictionary<string, string> locations = new Dictionary<string, string>();

            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                string[] k = key.Split('|');
                if ((k.Length > 1) && (k[0].ToUpper() == "MIRTHLOGS"))
                {
                    string value = ConfigurationManager.AppSettings[key];
                    switch (k[1].ToUpper())
                    {
                        case "SERVER":
                            Session[SessionKey_MirthLogs_Server] = value;
                            break;
                        case "ENVIRONMENTS":
                            string[] envs = value.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string env in envs)
                            {
                                string[] details = env.Split('|');
                                if (details.Length > 1)
                                {
                                    environments.Add(details[0], details[1]);
                                }
                            }
                            break;
                        case "LOCATIONS":
                            string[] locs = value.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string loc in locs)
                            {
                                string[] details = loc.Split('|');
                                if (details.Length > 1)
                                {
                                    locations.Add(details[0], details[1]);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            Session[SessionKey_MirthLogs_Environments] = environments;
            Session[SessionKey_MirthLogs_Locations] = locations;
        }

        private void PopulateEnvironments()
        {
            rblistMirthEnvs.Items.Clear();

            if (Session[SessionKey_MirthLogs_Environments] != null)
            {
                foreach (var pair in (Dictionary<string, string>)Session[SessionKey_MirthLogs_Environments])
                {
                    rblistMirthEnvs.Items.Add(new ListItem(pair.Key, pair.Value, true));
                }
            }

            rblistMirthEnvs.SelectedIndex = 0;

            rblistLogLocations.Items.Clear();

            if (Session[SessionKey_MirthLogs_Locations] != null)
            {
                foreach (var pair in (Dictionary<string, string>)Session[SessionKey_MirthLogs_Locations])
                {
                    rblistLogLocations.Items.Add(new ListItem(pair.Key, pair.Value, true));
                }
            }

            rblistLogLocations.SelectedIndex = 0;
        }

        private void ClearStatusMessages()
        {
            lblStatusMsg.Text = string.Empty;
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

                ClearStatusMessages();
            }
            else
            {
                lblStatusMsg.Text = statusMsg;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            MirthSSHVM vm = (MirthSSHVM)Session[SessionKey_MirthLogs_MirthSSHVM];

            if ((vm == null) || vm.Disconnect(out string statusMsg))
            {
                UserLoggedOut();

                Session[SessionKey_MirthLogs_MirthSSHVM] = null;
                Session[SessionKey_MirthLogs_LoggedInUser] = null;

                ClearStatusMessages();
            }
            else
            {
                lblStatusMsg.Text = statusMsg;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            MirthSSHVM vm = (MirthSSHVM)Session[SessionKey_MirthLogs_MirthSSHVM];
            if (vm != null)
            {
                string env = rblistMirthEnvs.SelectedValue;
                string loc = rblistLogLocations.SelectedValue;

                _ = vm.SearchPrdLogs(env, loc, tbSearchText.Text, tbSearchFiles.Value, cbIgnoreCase.Checked, cbRegEx.Checked, out string result, out string cmdExecuted);

                if (cbSpaceLogLines.Checked)
                {
                    result = result.Replace("\r\n", "\r\n\r\n");
                }

                tbSearchResults.Text = result;
                lblCmd.Text = cmdExecuted;
            }
        }

        protected void btnListFiles_Click(object sender, EventArgs e)
        {
            MirthSSHVM vm = (MirthSSHVM)Session[SessionKey_MirthLogs_MirthSSHVM];
            if (vm != null)
            {
                string env = rblistMirthEnvs.SelectedValue;
                string loc = rblistLogLocations.SelectedValue;

                _ = vm.ListFiles(env, loc, tbSearchFiles.Value, cbIgnoreCase.Checked, cbRegEx.Checked, out string result, out string cmdExecuted);
                tbSearchResults.Text = result;
                lblCmd.Text = cmdExecuted;
            }
        }

        protected void btnRefreshLogFilesList_Click(object sender, EventArgs e)
        {
            MirthSSHVM vm = (MirthSSHVM)Session[SessionKey_MirthLogs_MirthSSHVM];
            if (vm != null)
            {
                ddlLogFiles.Items.Clear();

                string env = rblistMirthEnvs.SelectedValue;
                string loc = rblistLogLocations.SelectedValue;

                _ = vm.ListUniqueFiles(env, loc, out string result, out string cmdExecuted);
                lblCmd.Text = cmdExecuted;

                foreach (string logFile in result.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!logFile.StartsWith("."))
                        ddlLogFiles.Items.Add(logFile);
                }
            }
        }

        private void UserLoggedIn()
        {
            btnLogin.Enabled = false;
            btnLogout.Enabled = true;
            btnSearch.Enabled = true;
            btnListFiles.Enabled = true;
            btnRefreshLogFilesList.Enabled = true;
        }

        private void UserLoggedOut()
        {
            btnLogin.Enabled = true;
            btnLogout.Enabled = false;
            btnSearch.Enabled = false;
            btnListFiles.Enabled = false;
            btnRefreshLogFilesList.Enabled = false;
        }
    }
}