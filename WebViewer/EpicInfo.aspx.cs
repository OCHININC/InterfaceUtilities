using org.ochin.interoperability.OCHINInterfaceUtilities.Epic;
using org.ochin.interoperability.OCHINInterfaceUtilities.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace org.ochin.interoperability.OCHINInterfaceUtilities
{
    public partial class EpicInfo : Page
    {
        private static Dictionary<string, string> EpicICServers = new Dictionary<string, string>();

        private static List<string> Deps = new List<string>();
        private static List<string> LabAccts = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReadConfig();
                PopulateEnvironments();

                Deps.Clear();
            }
            else
            {
                ConvertListToTable(Deps, tblDepList);
                ConvertListToTable(LabAccts, tblLabAccts);
            }
        }

        private void ReadConfig()
        {
            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                string[] k = key.Split('|');
                if ((k.Length > 1) && (k[0].ToUpper() == "EPICIC"))
                {
                    if (!EpicICServers.ContainsKey(k[1]))
                    {
                        EpicICServers.Add(k[1], ConfigurationManager.AppSettings[key]);
                    }
                }
            }
        }

        private void PopulateEnvironments()
        {
            rblistEpicICEnvs.Items.Clear();

            foreach (var pair in EpicICServers)
            {
                rblistEpicICEnvs.Items.Add(new ListItem(pair.Key, pair.Value, true));
            }

            rblistEpicICEnvs.SelectedIndex = 0;
        }

        protected void btnGetDepList_Click(object sender, EventArgs e)
        {
            EpicInterConnectVM EpicICVM = GetSelectedEpicICEnv();

            if (EpicICVM != null)
            {
                Deps.Clear();
                bool ret = EpicICVM.GetDepList(tbSA.Text, tbIIT.Text, out Deps, out string response);

                if (ret)
                {
                    lblStatusMsg.Text = string.Empty;

                    ConvertListToTable(Deps, tblDepList);
                }
                else
                {
                    lblStatusMsg.Text = response;
                }
            }
        }

        protected void btnGetLabAccts_Click(object sender, EventArgs e)
        {
            EpicInterConnectVM EpicICVM = GetSelectedEpicICEnv();

            if (EpicICVM != null)
            {
                bool ret = EpicICVM.GetLabAccts(out LabAccts, out string response);

                if (ret)
                {
                    lblStatusMsg.Text = string.Empty;

                    ConvertListToTable(LabAccts, tblLabAccts);
                }
                else
                {
                    lblStatusMsg.Text = response;
                }
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
            string selected = rblistEpicICEnvs.SelectedItem?.Text;

            if (!string.IsNullOrEmpty(selected) && EpicICServers.TryGetValue(selected, out string epicICBaseUrl))
            {
                lblStatusMsg.Text = string.Empty;
                return new EpicInterConnectVM(epicICBaseUrl);
            }
            else
            {
                lblStatusMsg.Text = "Unrecognized Epic InterConnect environment: " + selected;
            }

            return null;
        }

        protected void lbtnDownloadDepList_Click(object sender, EventArgs e)
        {
            DownloadListAsCsv(Deps, "DepList.csv");
        }

        protected void lbtnDownloadLabAccts_Click(object sender, EventArgs e)
        {
            DownloadListAsCsv(LabAccts, "LabAccts.csv");
        }

        private void DownloadListAsCsv(List<string> list, string filename)
        {
            if (list.Count > 0)
            {
                Response.Clear();
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", $"attachment; filename={filename}");

                StringBuilder sb = new StringBuilder(list.Count);
                foreach (string l in list)
                {
                    sb.AppendLine(l.Replace('^', ','));
                }

                byte[] bytes = Encoding.Default.GetBytes(sb.ToString());
                Response.OutputStream.Write(bytes, 0, bytes.Length);

                Response.End();
            }
        }
    }
}