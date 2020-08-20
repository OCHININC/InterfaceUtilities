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
        public static Dictionary<string, string> MirthServers = new Dictionary<string, string>();
        public static Dictionary<string, string> PentraServers = new Dictionary<string, string>();

        private static MirthRestApiVM MirthVM;
        private static PentraVM PentraVM;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReadConfig();
                PopulateEnvironments();
            }
        }

        private void ReadConfig()
        {
            MirthServers.Clear();
            PentraServers.Clear();

            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                string[] k = key.Split('|');
                if ((k.Length > 2) && (k[0].ToUpper() == "EPICMAINT"))
                {
                    switch (k[1].ToUpper())
                    {
                        case "MIRTH":
                            MirthServers.Add(k[2], ConfigurationManager.AppSettings[key]);
                            break;
                        case "PENTRA":
                            PentraServers.Add(k[2], ConfigurationManager.AppSettings[key]);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void PopulateEnvironments()
        {
            rblistMirthEnvs.Items.Clear();

            foreach (var pair in MirthServers)
            {
                rblistMirthEnvs.Items.Add(new ListItem(pair.Key, pair.Value, true));
            }

            rblistMirthEnvs.SelectedIndex = 0;

            rblistPentraEnvs.Items.Clear();

            foreach (var pair in PentraServers)
            {
                rblistPentraEnvs.Items.Add(new ListItem(pair.Key, pair.Value, true));
            }

            rblistPentraEnvs.SelectedIndex = 0;
        }

        protected void btnMirthLogin_Click(object sender, EventArgs e)
        {
            MirthVM = new MirthRestApiVM(rblistMirthEnvs.SelectedValue);

            if (MirthVM.Login(tbMirthUsername.Text, tbMirthPassword.Text, out string statusMsg))
            {
                btnMirthLogin.Enabled = false;
                btnMirthLogout.Enabled = true;
                btnRefreshChannels.Enabled = true;
                btnStopChannels.Enabled = true;
                btnStartChannels.Enabled = true;
                rblistMirthEnvs.Enabled = false;
            }

            lblStatusMsg.Text = statusMsg;
        }

        protected void btnMirthLogout_Click(object sender, EventArgs e)
        {
            if (MirthVM != null)
            {
                if (MirthVM.Logout(out string statusMsg))
                {
                    btnMirthLogin.Enabled = true;
                    btnMirthLogout.Enabled = false;
                    btnRefreshChannels.Enabled = false;
                    btnStopChannels.Enabled = false;
                    btnStartChannels.Enabled = false;
                    rblistMirthEnvs.Enabled = true;
                }

                lblStatusMsg.Text = statusMsg;
            }
        }

        protected void btnRefreshChannels_Click(object sender, EventArgs e)
        {
            if (MirthVM != null)
            {
                XmlDocument doc = MirthVM.GetChannelStatuses(false);

                DataSet dsChannels = new DataSet();
                using (XmlReader xr = new XmlNodeReader(doc.DocumentElement))
                {
                    dsChannels.ReadXml(xr);
                }

                gridChannels.DataSource = dsChannels.Tables[2].DefaultView;
                gridChannels.DataBind();

                UpdateChannelsSummary();

                cblistState_SelectedIndexChanged(sender, e);
            }
        }

        private void UpdateChannelsSummary()
        {
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
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (var s in channelsSummary.OrderBy(c => c.Key))
            {
                sb.Append(s.Key + ": " + s.Value + " | ");
            }

            lblChannelsSummary.Text = sb.ToString();
        }

        protected void btnStopChannels_Click(object sender, EventArgs e)
        {
            if (MirthVM != null)
            {
                List<string> channelIds = new List<string>();
                foreach (GridViewRow row in gridChannels.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow &&
                        (row.Cells[0].FindControl("cbSelect") as CheckBox).Checked)
                    {
                        channelIds.Add(row.Cells[4].Text);
                    }
                }

                if (channelIds.Count > 0)
                {
                    if (MirthVM.StopChannels(channelIds, true, out string statusMsg))
                    {
                        statusMsg = "Channels stop SUCCESSFUL";
                    }

                    btnRefreshChannels_Click(sender, e);

                    lblStatusMsg.Text = statusMsg;
                }
            }
        }

        protected void btnStartChannels_Click(object sender, EventArgs e)
        {
            if (MirthVM != null)
            {
                List<string> channelIds = new List<string>();
                foreach (GridViewRow row in gridChannels.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow &&
                        (row.Cells[0].FindControl("cbSelect") as CheckBox).Checked)
                    {
                        channelIds.Add(row.Cells[4].Text);
                    }
                }

                if (channelIds.Count > 0)
                {
                    if (MirthVM.StartChannels(channelIds, true, out string statusMsg))
                    {
                        statusMsg = "Channels start SUCCESSFUL";
                    }

                    btnRefreshChannels_Click(sender, e);

                    lblStatusMsg.Text = statusMsg;
                }
            }
        }

        protected void cblistState_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (ListItem item in cblistState.Items)
            {
                foreach (GridViewRow row in gridChannels.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow &&
                        row.Cells[1].Text.Equals(item.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        (row.Cells[0].FindControl("cbSelect") as CheckBox).Checked = item.Selected;
                    }
                }
            }
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

            PentraVM = new PentraVM(tbPentraUsername.Text, tbPentraPassword.Text, remoteHost, port);

            string statusMsg = string.Empty;
            if ((PentraVM != null) && (PentraVM.Connect(out statusMsg)))
            {
                btnPentraLogin.Enabled = false;
                btnPentraLogout.Enabled = true;
                btnRefreshPentraGatewayLogs.Enabled = true;
                btnStartPentra.Enabled = true;
                btnStopPentra.Enabled = true;
            }

            lblStatusMsg.Text = statusMsg;
        }

        protected void btnPentraLogout_Click(object sender, EventArgs e)
        {
            if (PentraVM != null)
            {
                if (PentraVM.Disconnect(out string statusMsg))
                {
                    btnPentraLogin.Enabled = true;
                    btnPentraLogout.Enabled = false;
                    btnRefreshPentraGatewayLogs.Enabled = false;
                    btnStartPentra.Enabled = false;
                    btnStopPentra.Enabled = false;
                }

                lblStatusMsg.Text = statusMsg;
            }
        }

        protected void btnRefreshPentraGatewayLogs_Click(object sender, EventArgs e)
        {
            if (PentraVM != null)
            {
                int lines = int.Parse(tbPentraGatewayLogLines.Text);

                _ = PentraVM.TailGatewayLogs(lines, out string result, out string cmdExecuted);
                tbPentraGatewayLog.Text = result;
                lblPentraCmd.Text = cmdExecuted;
            }
        }

        protected void btnStopPentra_Click(object sender, EventArgs e)
        {
            if (PentraVM != null)
            {
                PentraVM.StopGateway(out string result, out string cmdExecuted);

                lblStatusMsg.Text = result;
                lblPentraCmd.Text = cmdExecuted;
            }
        }

        protected void btnStartPentra_Click(object sender, EventArgs e)
        {
            if (PentraVM != null)
            {
                PentraVM.StartGateway(out string result, out string cmdExecuted);

                lblStatusMsg.Text = result;
                lblPentraCmd.Text = cmdExecuted;
            }
        }
    }
}