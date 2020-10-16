using Microsoft.Ajax.Utilities;
using org.ochin.interoperability.OCHINInterfaceUtilities.Mirth;
using org.ochin.interoperability.OCHINInterfaceUtilities.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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
            ClearStatusMessages();

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

        private void ClearStatusMessages()
        {
            lblStatusMsg.Text = string.Empty;
        }

        protected void btnMirthLogin_Click(object sender, EventArgs e)
        {
            MirthRestApiVM vm = new MirthRestApiVM(rblistMirthEnvs.SelectedValue);

            if (vm.Login(tbMirthUsername.Text, tbMirthPassword.Text, out string statusMsg))
            {
                MirthUserLoggedIn();

                Session[SessionKey_MirthInfo_Mirth_VM] = vm;
                Session[SessionKey_MirthInfo_Mirth_LoggedInUser] = tbMirthUsername.Text;

                ClearStatusMessages();
            }
            else
            {
                lblStatusMsg.Text = statusMsg;
            }
        }

        protected void btnMirthLogout_Click(object sender, EventArgs e)
        {
            MirthRestApiVM vm = (MirthRestApiVM)Session[SessionKey_MirthInfo_Mirth_VM];
            if ((vm == null) || vm.Logout(out string statusMsg))
            {
                MirthUserLoggedOut();

                Session[SessionKey_MirthInfo_Mirth_VM] = null;
                Session[SessionKey_MirthInfo_Mirth_LoggedInUser] = null;

                ClearStatusMessages();
            }
            else
            {
                lblStatusMsg.Text = statusMsg;
            }
        }

        protected void btnGetMirthInventory_Click(object sender, EventArgs e)
        {
            MirthRestApiVM vm = (MirthRestApiVM)Session[SessionKey_MirthInfo_Mirth_VM];
            if (vm != null)
            {
                XmlDocument doc = vm.GetChannelTags(cbIncludeDesc.Checked, out HashSet<string> uniqueTags, out HashSet<string> uniqueServers, out HashSet<string> uniqueStates);

                DataSet dsChannels = new DataSet();
                using (XmlReader xr = new XmlNodeReader(doc.DocumentElement))
                {
                    dsChannels.ReadXml(xr);
                }

                gridMirthInventory.DataSource = dsChannels.Tables[2].DefaultView;
                gridMirthInventory.DataBind();

                // Populate "Tags" filter list
                //HashSet<string> uniqueTags = new HashSet<string>();
                //XmlNodeList tagsNodes = doc.SelectNodes("/servers/server/channels/channel/tags");
                //foreach (XmlNode tagsNode in tagsNodes)
                //{
                //    string[] tags = tagsNode.InnerText.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                //    foreach (string tag in tags)
                //    {

                //        uniqueTags.Add(tag.Trim());
                //    }
                //}

                var uniqueTagsList = uniqueTags.ToList();
                uniqueTagsList.Sort();
                foreach (var tag in uniqueTagsList)
                {
                    lbTags.Items.Add(new ListItem(tag));
                }

                var uniqueServersList = uniqueServers.ToList();
                uniqueServersList.Sort();
                foreach (var server in uniqueServersList)
                {
                    lbServers.Items.Add(new ListItem(server));
                }

                var uniqueStatesList = uniqueStates.ToList();
                uniqueStatesList.Sort();
                foreach (var state in uniqueStatesList)
                {
                    lbStates.Items.Add(new ListItem(state));
                }
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