using org.ochin.interoperability.OCHINInterfaceUtilities.Epic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace org.ochin.interoperability.OCHINInterfaceUtilities
{
    public partial class EpicInfo : Page
    {
        private static readonly string SessionKey_EpicInfo_EpicICServers = "EpicInfo_EpicICServers";

        private static readonly string ViewStateKey_EpicInfo_Deps = "EpicInfo_Deps";
        private static readonly string ViewStateKey_EpicInfo_LabAccts = "EpicInfo_LabAccts";

        protected void Page_Load(object sender, EventArgs e)
        {
            ClearStatusMessages();

            if (Session[SessionKey_EpicInfo_EpicICServers] == null)
            {
                ReadConfig();
            }

            if (!IsPostBack)
            {
                PopulateEnvironments();
            }
            else
            {
                ConvertDepListToTable();
                ConvertLabAcctsListToTable();
            }
        }

        private void ReadConfig()
        {
            Dictionary<string, string> epicICServers = new Dictionary<string, string>();

            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                string[] k = key.Split('|');
                if ((k.Length > 1) && (k[0].ToUpper() == "EPICIC"))
                {
                    if (!epicICServers.ContainsKey(k[1]))
                    {
                        epicICServers.Add(k[1], ConfigurationManager.AppSettings[key]);
                    }
                }
            }

            Session[SessionKey_EpicInfo_EpicICServers] = epicICServers;
        }

        private void PopulateEnvironments()
        {
            if (Session[SessionKey_EpicInfo_EpicICServers] != null)
            {
                rblistEpicICEnvs.Items.Clear();

                foreach (var pair in (Dictionary<string, string>)Session[SessionKey_EpicInfo_EpicICServers])
                {
                    rblistEpicICEnvs.Items.Add(new ListItem(pair.Key, pair.Value, true));
                }

                rblistEpicICEnvs.SelectedIndex = 0;
            }
        }

        private void ClearStatusMessages()
        {
            lblStatusMsgEnvs.Text = string.Empty;
            lblStatusMsgDepList.Text = string.Empty;
            lblStatusMsgGetLabAccts.Text = string.Empty;
            lblStatusMsgRebuildLabAccts.Text = string.Empty;
        }

        protected void btnGetDepList_Click(object sender, EventArgs e)
        {
            EpicInterConnectVM EpicICVM = GetSelectedEpicICEnv();

            if (EpicICVM != null)
            {
                bool ret = EpicICVM.GetDepList(tbSA.Text, tbIIT.Text, out List<string> deps, out string response);
                ViewState[ViewStateKey_EpicInfo_Deps] = deps;

                if (ret)
                {
                    lblStatusMsgDepList.Text = string.Empty;

                    ConvertDepListToTable();
                }
                else
                {
                    lblStatusMsgDepList.Text = response;
                }
            }
        }

        protected void btnGetLabAccts_Click(object sender, EventArgs e)
        {
            EpicInterConnectVM EpicICVM = GetSelectedEpicICEnv();

            if (EpicICVM != null)
            {
                bool ret = EpicICVM.GetLabAccts(out List<string> labAccts, out string response);
                ViewState[ViewStateKey_EpicInfo_LabAccts] = labAccts;

                if (ret)
                {
                    lblStatusMsgGetLabAccts.Text = string.Empty;

                    ConvertLabAcctsListToTable();
                }
                else
                {
                    lblStatusMsgGetLabAccts.Text = response;
                }
            }
        }

        protected void btnRebuildLabAccts_Click(object sender, EventArgs e)
        {
            EpicInterConnectVM EpicICVM = GetSelectedEpicICEnv();

            if (EpicICVM != null)
            {
                bool ret = EpicICVM.RebuildLabAccts(out string response);

                if (ret)
                {
                    lblStatusMsgRebuildLabAccts.Text = "Lab accounts index rebuilt successfully!";
                }
                else
                {
                    lblStatusMsgRebuildLabAccts.Text = response;
                }
            }
        }
        
        private void ConvertDepListToTable()
        {
            var deps = (List<string>)ViewState[ViewStateKey_EpicInfo_Deps];
            if (deps != null)
            {
                ConvertListToTable(deps, tblDepList);
            }
        }

        private void ConvertLabAcctsListToTable()
        {
            var labAccts = (List<string>)ViewState[ViewStateKey_EpicInfo_LabAccts];
            if (labAccts != null)
            {
                ConvertListToTable(labAccts, tblLabAccts);
            }
        }

        private void ConvertListToTable(List<string> list, Table tbl)
        {
            if (list != null)
            {
                tbl.Rows.Clear();

                for (int i = 0; i < list.Count; i++)
                {
                    string[] dep = list[i].Split('^');
                    TableRow row = (i == 0) ? new TableHeaderRow() : new TableRow();
                    foreach (string value in dep)
                    {
                        TableCell cell = (i == 0) ? new TableHeaderCell() : new TableCell();
                        cell.Text = value;
                        _ = row.Cells.Add(cell);
                    }

                    _ = tbl.Rows.Add(row);
                }
            }
        }

        private EpicInterConnectVM GetSelectedEpicICEnv()
        {
            string selectedValue = rblistEpicICEnvs.SelectedItem?.Value;

            if (!string.IsNullOrEmpty(selectedValue))
            {
                lblStatusMsgEnvs.Text = string.Empty;
                return new EpicInterConnectVM(selectedValue);
            }
            else
            {
                lblStatusMsgEnvs.Text = "Unrecognized Epic InterConnect environment: " + rblistEpicICEnvs.SelectedItem?.Text;
            }

            return null;
        }

        protected void lbtnDownloadDepList_Click(object sender, EventArgs e)
        {
            if (ViewState[ViewStateKey_EpicInfo_Deps] != null)
            {
                DownloadListAsCsv((List<string>)ViewState[ViewStateKey_EpicInfo_Deps], "DepList.csv");
            }
        }

        protected void lbtnDownloadLabAccts_Click(object sender, EventArgs e)
        {
            string filename = "LabAccts.csv";

            if (!string.IsNullOrEmpty(hfLabAccts.Value))
            {
                DownloadAsCsv(hfLabAccts.Value, filename);
            }

            else if (ViewState[ViewStateKey_EpicInfo_LabAccts] != null)
            {
                DownloadListAsCsv((List<string>)ViewState[ViewStateKey_EpicInfo_LabAccts], filename);
            }
        }

        private void DownloadListAsCsv(List<string> list, string filename)
        {
            if (list.Count > 0)
            {
                StringBuilder sb = new StringBuilder(list.Count);
                foreach (string l in list)
                {
                    sb.AppendLine(l.Replace('^', ','));
                }

                DownloadAsCsv(sb.ToString(), filename);
            }
        }

        private void DownloadAsCsv(string content, string filename)
        {
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", $"attachment; filename={filename}");

            byte[] bytes = Encoding.Default.GetBytes(content);
            Response.OutputStream.Write(bytes, 0, bytes.Length);

            Response.End();
        }
    }
}