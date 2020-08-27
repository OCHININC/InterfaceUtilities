using org.ochin.interoperability.OCHINInterfaceUtilities.Mirth;
using org.ochin.interoperability.OCHINInterfaceUtilities.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace org.ochin.interoperability.OCHINInterfaceUtilities
{
    public partial class MirthInventory : Page
    {
        private static readonly string SessionKey_MirthInfo_Mirth_LoggedInUser = "MirthInfo_Mirth_LoggedInUser";
        private static readonly string SessionKey_MirthInfo_Mirth_Servers = "MirthInfo_Mirth_Servers";
        private static readonly string SessionKey_MirthInfo_Mirth_VM = "MirthInfo_Mirth_VM";

        private static readonly string ViewStateKey_MirthInfo_MirthInventory = "MirthInfo_MirthInventory";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[SessionKey_MirthInfo_Mirth_Servers] == null)
            {
                ReadConfig();
            }

            if (!IsPostBack)
            {
                PopulateEnvironments();

                string mirthLoggedInUser = (string)Session[SessionKey_MirthInfo_Mirth_LoggedInUser];
                if (!string.IsNullOrEmpty(mirthLoggedInUser))
                {
                    MirthUserLoggedIn();
                }
                else
                {
                    MirthUserLoggedOut();
                }
            }
        }

        private void ReadConfig()
        {
            Dictionary<string, string> mirthServers = new Dictionary<string, string>();

            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                string[] k = key.Split('|');
                if ((k.Length > 1) && ((k[0].ToUpper() == "MIRTH")))
                {
                    mirthServers.Add(k[1], ConfigurationManager.AppSettings[key]);
                }
            }

            Session[SessionKey_MirthInfo_Mirth_Servers] = mirthServers;
        }

        private void PopulateEnvironments()
        {
            rblistMirthEnvs.Items.Clear();

            if (Session[SessionKey_MirthInfo_Mirth_Servers] != null)
            {
                foreach (var pair in (Dictionary<string, string>)Session[SessionKey_MirthInfo_Mirth_Servers])
                {
                    rblistMirthEnvs.Items.Add(new ListItem(pair.Key, pair.Value, true));
                }
            }

            rblistMirthEnvs.SelectedIndex = 0;
        }

        protected void btnMirthLogin_Click(object sender, EventArgs e)
        {
            MirthRestApiVM vm = new MirthRestApiVM(rblistMirthEnvs.SelectedValue);

            if (vm.Login(tbMirthUsername.Text, tbMirthPassword.Text, out string statusMsg))
            {
                MirthUserLoggedIn();

                Session[SessionKey_MirthInfo_Mirth_VM] = vm;
                Session[SessionKey_MirthInfo_Mirth_LoggedInUser] = tbMirthUsername.Text;
            }

            lblStatusMsg.Text = statusMsg;
        }

        protected void btnMirthLogout_Click(object sender, EventArgs e)
        {
            MirthRestApiVM vm = (MirthRestApiVM)Session[SessionKey_MirthInfo_Mirth_VM];
            if (vm != null)
            {
                if (vm.Logout(out string statusMsg))
                {
                    MirthUserLoggedOut();

                    Session[SessionKey_MirthInfo_Mirth_VM] = null;
                    Session[SessionKey_MirthInfo_Mirth_LoggedInUser] = null;
                }

                lblStatusMsg.Text = statusMsg;
            }
        }

        protected void btnGetMirthInventory_Click(object sender, EventArgs e)
        {
            MirthRestApiVM vm = (MirthRestApiVM)Session[SessionKey_MirthInfo_Mirth_VM];
            if (vm != null)
            {
                XmlDocument doc = vm.GetChannelTags(cbIncludeDesc.Checked);

                DataSet dsChannels = new DataSet();
                using (XmlReader xr = new XmlNodeReader(doc.DocumentElement))
                {
                    dsChannels.ReadXml(xr);
                }

                gridMirthInventory.DataSource = dsChannels.Tables[2].DefaultView;
                gridMirthInventory.DataBind();
            }
        }

        protected void lbtnDownloadMirthInventory_Click(object sender, EventArgs e)
        {
            string filename = "MirthInventory.csv";

            if (!string.IsNullOrEmpty(hfMirthInventory.Value))
            {
                HTTPUtilities.Download(hfMirthInventory.Value, filename, Response);
            }

            else if (ViewState[ViewStateKey_MirthInfo_MirthInventory] != null)
            {
                HTTPUtilities.DownloadListAsCsv((List<string>)ViewState[ViewStateKey_MirthInfo_MirthInventory], filename, Response);
            }
        }

        private void MirthUserLoggedIn()
        {
            btnMirthLogin.Enabled = false;
            btnMirthLogout.Enabled = true;
            btnGetMirthInventory.Enabled = true;
            rblistMirthEnvs.Enabled = false;
            cbIncludeDesc.Enabled = true;
        }

        private void MirthUserLoggedOut()
        {
            btnMirthLogin.Enabled = true;
            btnMirthLogout.Enabled = false;
            btnGetMirthInventory.Enabled = false;
            rblistMirthEnvs.Enabled = true;
            cbIncludeDesc.Enabled = false;
        }
    }
}