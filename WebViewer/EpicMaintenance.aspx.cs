using org.ochin.interoperability.OCHINInterfaceUtilities.Mirth;
using org.ochin.interoperability.OCHINInterfaceUtilities.Pentra;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace org.ochin.interoperability.OCHINInterfaceUtilities
{
    public partial class EpicMaintenance : Page
    {
        private static readonly string SessionKey_EpicMaint_Mirth_LoggedInUser = "EpicMaint_Mirth_LoggedInUser";
        private static readonly string SessionKey_EpicMaint_Mirth_Servers = "EpicMaint_Mirth_Servers";
        private static readonly string SessionKey_EpicMaint_Mirth_VM = "EpicMaint_Mirth_VM";

        private static readonly string SessionKey_EpicMaint_Pentra_LoggedInUser = "EpicMaint_Pentra_LoggedInUser";
        private static readonly string SessionKey_EpicMaint_Pentra_Servers = "EpicMaint_Pentra_Servers";
        private static readonly string SessionKey_EpicMaint_Pentra_VM = "EpicMaint_Pentra_VM";

        protected void Page_Load(object sender, EventArgs e)
        {
            ClearStatusMessages();

            if (Session[SessionKey_EpicMaint_Mirth_Servers] == null || Session[SessionKey_EpicMaint_Pentra_Servers] == null)
            {
                ReadConfig();
            }

            if (!IsPostBack)
            {
                PopulateEnvironments();

                string mirthLoggedInUser = (string)Session[SessionKey_EpicMaint_Mirth_LoggedInUser];
                if (!string.IsNullOrEmpty(mirthLoggedInUser))
                {
                    MirthUserLoggedIn();
                }
                else
                {
                    MirthUserLoggedOut();
                }

                string pentraLoggedInUser = (string)Session[SessionKey_EpicMaint_Pentra_LoggedInUser];
                if (!string.IsNullOrEmpty(pentraLoggedInUser))
                {
                    PentraUserLoggedIn();
                }
                else
                {
                    PentraUserLoggedOut();
                }
            }
        }

        private void ReadConfig()
        {
            Dictionary<string, string> mirthServers = new Dictionary<string, string>();
            Dictionary<string, string> pentraServers = new Dictionary<string, string>();

            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                string[] k = key.Split('|');
                if ((k.Length > 1) && ((k[0].ToUpper() == "MIRTH") || (k[0].ToUpper() == "PENTRA")))
                {
                    switch (k[0].ToUpper())
                    {
                        case "MIRTH":
                            mirthServers.Add(k[1], ConfigurationManager.AppSettings[key]);
                            break;
                        case "PENTRA":
                            pentraServers.Add(k[1], ConfigurationManager.AppSettings[key]);
                            break;
                        default:
                            break;
                    }
                }
            }

            Session[SessionKey_EpicMaint_Mirth_Servers] = mirthServers;
            Session[SessionKey_EpicMaint_Pentra_Servers] = pentraServers;
        }

        private void PopulateEnvironments()
        {
            rblistMirthEnvs.Items.Clear();

            if (Session[SessionKey_EpicMaint_Mirth_Servers] != null)
            {
                foreach (var pair in (Dictionary<string, string>)Session[SessionKey_EpicMaint_Mirth_Servers])
                {
                    rblistMirthEnvs.Items.Add(new ListItem(pair.Key, pair.Value, true));
                }
            }

            rblistMirthEnvs.SelectedIndex = 0;

            rblistPentraEnvs.Items.Clear();

            if (Session[SessionKey_EpicMaint_Mirth_Servers] != null)
            {
                foreach (var pair in (Dictionary<string, string>)Session[SessionKey_EpicMaint_Pentra_Servers])
                {
                    rblistPentraEnvs.Items.Add(new ListItem(pair.Key, pair.Value, true));
                }
            }

            rblistPentraEnvs.SelectedIndex = 0;
        }

        private void ClearStatusMessages()
        {
            lblStatusMsgMirthCredentials.Text = string.Empty;
            lblStatusMsgPentraCredentials.Text = string.Empty;
            lblStatusMsgMirthActions.Text = string.Empty;
            lblStatusMsgPentraActions.Text = string.Empty;
        }

        protected void btnMirthLogin_Click(object sender, EventArgs e)
        {
            MirthRestApiVM vm = new MirthRestApiVM(rblistMirthEnvs.SelectedValue);

            if (vm.Login(tbMirthUsername.Text, tbMirthPassword.Text, out string statusMsg))
            {
                MirthUserLoggedIn();

                Session[SessionKey_EpicMaint_Mirth_VM] = vm;
                Session[SessionKey_EpicMaint_Mirth_LoggedInUser] = tbMirthUsername.Text;

                lblStatusMsgMirthCredentials.Text = string.Empty;
            }
            else
            {
                lblStatusMsgMirthCredentials.Text = statusMsg;
            }
        }

        protected void btnMirthLogout_Click(object sender, EventArgs e)
        {
            MirthRestApiVM vm = (MirthRestApiVM)Session[SessionKey_EpicMaint_Mirth_VM];

            if ((vm == null) || vm.Logout(out string statusMsg))
            {
                MirthUserLoggedOut();

                Session[SessionKey_EpicMaint_Mirth_VM] = null;
                Session[SessionKey_EpicMaint_Mirth_LoggedInUser] = null;

                lblStatusMsgMirthCredentials.Text = string.Empty;
            }
            else
            {
                lblStatusMsgMirthCredentials.Text = statusMsg;
            }
        }

        protected void btnRefreshChannels_Click(object sender, EventArgs e)
        {
            MirthRestApiVM vm = (MirthRestApiVM)Session[SessionKey_EpicMaint_Mirth_VM];
            if (vm != null)
            {
                XmlDocument doc = vm.GetChannelStatuses(false, true);

                DataSet dsChannels = new DataSet();
                using (XmlReader xr = new XmlNodeReader(doc.DocumentElement))
                {
                    dsChannels.ReadXml(xr);
                }

                var tableChannel = dsChannels.Tables["channel"];
                var tableSourceConnector = dsChannels.Tables["sourceConnector"];

                tableChannel.Merge(tableSourceConnector);

                gridChannels.DataSource = tableChannel.DefaultView;
                gridChannels.DataBind();

                UpdateSelectedRows(sender, e);
            }
        }

        private void UpdateChannelsSummary()
        {
            uint selectedRows = 0;

            Dictionary<string, int> channelsSummary = new Dictionary<string, int>();
            foreach (GridViewRow row in gridChannels.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    string state = row.Cells[1].Text;
                    if (channelsSummary.ContainsKey(state))
                    {
                        channelsSummary[state]++;
                    }
                    else
                    {
                        channelsSummary.Add(state, 1);
                    }

                    if ((row.Cells[0].FindControl("cbSelect") as CheckBox).Checked)
                    {
                        selectedRows++;
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (var s in channelsSummary.OrderBy(c => c.Key))
            {
                sb.Append(s.Key + ": " + s.Value + " | ");
            }
            sb.Append("Selected: " + selectedRows);

            lblChannelsSummary.Text = sb.ToString();
        }

        protected void btnStopChannels_Click(object sender, EventArgs e)
        {
            MirthRestApiVM vm = (MirthRestApiVM)Session[SessionKey_EpicMaint_Mirth_VM];
            if (vm != null)
            {
                List<string> channelIds = new List<string>();
                foreach (GridViewRow row in gridChannels.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow &&
                        (row.Cells[0].FindControl("cbSelect") as CheckBox).Checked)
                    {
                        channelIds.Add(row.Cells[5].Text);
                    }
                }

                if (channelIds.Count > 0)
                {
                    if (vm.StopChannels(channelIds, true, out string statusMsg))
                    {
                        statusMsg = "Channels stop SUCCESSFUL";
                    }

                    btnRefreshChannels_Click(sender, e);

                    lblStatusMsgMirthActions.Text = statusMsg;
                }
            }
        }

        protected void btnStartChannels_Click(object sender, EventArgs e)
        {
            MirthRestApiVM vm = (MirthRestApiVM)Session[SessionKey_EpicMaint_Mirth_VM];
            if (vm != null)
            {
                List<string> channelIds = new List<string>();
                foreach (GridViewRow row in gridChannels.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow &&
                        (row.Cells[0].FindControl("cbSelect") as CheckBox).Checked)
                    {
                        channelIds.Add(row.Cells[5].Text);
                    }
                }

                if (channelIds.Count > 0)
                {
                    if (vm.StartChannels(channelIds, true, out string statusMsg))
                    {
                        statusMsg = "Channels start SUCCESSFUL";
                    }

                    btnRefreshChannels_Click(sender, e);

                    lblStatusMsgMirthActions.Text = statusMsg;
                }
            }
        }

        protected void UpdateSelectedRows(object sender, EventArgs e)
        {
            bool nonChannelReaderOnly = cbNonChannelReaderOnly.Checked;

            foreach (ListItem item in cblistState.Items)
            {
                foreach (GridViewRow row in gridChannels.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow &&
                        row.Cells[1].Text.Equals(item.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        // Select row if "State" and "Source Type" match checkbox selections
                        (row.Cells[0].FindControl("cbSelect") as CheckBox).Checked =
                            item.Selected
                            && (!nonChannelReaderOnly || !row.Cells[4].Text.Equals("Channel Reader", StringComparison.OrdinalIgnoreCase));
                    }
                }
            }

            UpdateChannelsSummary();
        }

        protected void gridChannels_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[1].Text.Equals("STARTED", StringComparison.OrdinalIgnoreCase))
                {
                    e.Row.Cells[1].BackColor = Color.Green;
                    e.Row.Cells[1].ForeColor = Color.White;
                }
            }
        }
        protected void btnPentraLogin_Click(object sender, EventArgs e)
        {
            string[] ipPort = rblistPentraEnvs.SelectedValue.Split(':');
            string remoteHost = ipPort[0];

            int port = 22;
            if (ipPort.Length > 1)
            {
                port = int.Parse(ipPort[1]);
            }

            PentraVM vm = new PentraVM(tbPentraUsername.Text, tbPentraPassword.Text, remoteHost, port);

            string statusMsg = string.Empty;
            if ((vm != null) && (vm.Connect(out statusMsg)))
            {
                PentraUserLoggedIn();

                Session[SessionKey_EpicMaint_Pentra_VM] = vm;
                Session[SessionKey_EpicMaint_Pentra_LoggedInUser] = tbPentraUsername.Text;

                lblStatusMsgPentraCredentials.Text = string.Empty;
            }
            else
            {
                lblStatusMsgPentraCredentials.Text = statusMsg;
            }
        }

        protected void btnPentraLogout_Click(object sender, EventArgs e)
        {
            PentraVM vm = (PentraVM)Session[SessionKey_EpicMaint_Pentra_VM];

            if ((vm == null) ||vm.Disconnect(out string statusMsg))
            {
                PentraUserLoggedOut();

                Session[SessionKey_EpicMaint_Pentra_VM] = null;
                Session[SessionKey_EpicMaint_Pentra_LoggedInUser] = null;

                lblStatusMsgPentraCredentials.Text = string.Empty;
            }
            else
            {
                lblStatusMsgPentraCredentials.Text = statusMsg;
            }
        }

        protected void btnRefreshPentraGatewayLogs_Click(object sender, EventArgs e)
        {
            PentraVM vm = (PentraVM)Session[SessionKey_EpicMaint_Pentra_VM];
            if (vm != null)
            {
                int lines = int.Parse(tbPentraGatewayLogLines.Text);

                _ = vm.TailGatewayLogs(lines, out string result, out string cmdExecuted);
                tbPentraGatewayLog.Text = result;
                lblPentraCmd.Text = cmdExecuted;
            }
        }

        protected void btnStopPentra_Click(object sender, EventArgs e)
        {
            PentraVM vm = (PentraVM)Session[SessionKey_EpicMaint_Pentra_VM];
            if (vm != null)
            {
                vm.StopGateway(out string result, out string cmdExecuted);

                lblStatusMsgPentraActions.Text = result;
                lblPentraCmd.Text = cmdExecuted;
            }
        }

        protected void btnStartPentra_Click(object sender, EventArgs e)
        {
            PentraVM vm = (PentraVM)Session[SessionKey_EpicMaint_Pentra_VM];
            if (vm != null)
            {
                vm.StartGateway(out string result, out string cmdExecuted);

                lblStatusMsgPentraActions.Text = result;
                lblPentraCmd.Text = cmdExecuted;
            }
        }

        private void MirthUserLoggedIn()
        {
            btnMirthLogin.Enabled = false;
            btnMirthLogout.Enabled = true;
            btnRefreshChannels.Enabled = true;
            btnStopChannels.Enabled = true;
            btnStartChannels.Enabled = true;
            rblistMirthEnvs.Enabled = false;
        }

        private void MirthUserLoggedOut()
        {
            btnMirthLogin.Enabled = true;
            btnMirthLogout.Enabled = false;
            btnRefreshChannels.Enabled = false;
            btnStopChannels.Enabled = false;
            btnStartChannels.Enabled = false;
            rblistMirthEnvs.Enabled = true;
        }

        private void PentraUserLoggedIn()
        {
            btnPentraLogin.Enabled = false;
            btnPentraLogout.Enabled = true;
            btnRefreshPentraGatewayLogs.Enabled = true;
            btnStartPentra.Enabled = true;
            btnStopPentra.Enabled = true;
        }

        private void PentraUserLoggedOut()
        {
            btnPentraLogin.Enabled = true;
            btnPentraLogout.Enabled = false;
            btnRefreshPentraGatewayLogs.Enabled = false;
            btnStartPentra.Enabled = false;
            btnStopPentra.Enabled = false;
        }
    }
}