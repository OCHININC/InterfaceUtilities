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
    public partial class MirthConfigMap : Page
    {
        private static readonly string SessionKey_MirthConfigMap_Mirth_LoggedInUser = "MirthConfigMap_Mirth_LoggedInUser";
        private static readonly string SessionKey_MirthConfigMap_Mirth_Servers = "MirthConfigMap_Mirth_Servers";
        private static readonly string SessionKey_MirthConfigMap_Mirth_VM = "MirthConfigMap_Mirth_VM";
        private static readonly string SessionKey_MirthConfigMap_ConfigMap = "MirthConfigMap_ConfigMap";
        private static readonly string SessionKey_MirthConfigMap_Features = "MirthConfigMap_Features";

        protected void Page_Load(object sender, EventArgs e)
        {
            ClearStatusMessages();

            if (Session[SessionKey_MirthConfigMap_Mirth_Servers] == null)
            {
                ReadConfig();
            }

            if (!IsPostBack)
            {
                PopulateEnvironments();

                string mirthLoggedInUser = (string)Session[SessionKey_MirthConfigMap_Mirth_LoggedInUser];
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
            Dictionary<string, string[]> features = new Dictionary<string, string[]>();

            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                string[] k = key.Split('|');

                if (k.Length > 1)
                {
                    string group = k[0].ToUpper();

                    if (group == "MIRTH")
                    {
                        // List all the individual servers
                        string serverList = ConfigurationManager.AppSettings[key];
                        if (!string.IsNullOrEmpty(serverList))
                        {
                            foreach (string server in serverList.Split(','))
                            {
                                string serverFriendlyName = server.Split('|')[1];

                                mirthServers.Add(serverFriendlyName, server);
                            }
                        }
                    }
                    else if (group == "FEATURES")
                    {
                        string featureList = ConfigurationManager.AppSettings[key];
                        features.Add(k[1], featureList.Split(','));
                    }
                }
            }

            Session[SessionKey_MirthConfigMap_Mirth_Servers] = mirthServers;
            Session[SessionKey_MirthConfigMap_Features] = features;
        }

        private void PopulateEnvironments()
        {
            rblistMirthEnvs.Items.Clear();

            if (Session[SessionKey_MirthConfigMap_Mirth_Servers] != null)
            {
                foreach (var pair in (Dictionary<string, string>)Session[SessionKey_MirthConfigMap_Mirth_Servers])
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

                Session[SessionKey_MirthConfigMap_Mirth_VM] = vm;
                Session[SessionKey_MirthConfigMap_Mirth_LoggedInUser] = tbMirthUsername.Text;

                ClearStatusMessages();
            }
            else
            {
                lblStatusMsg.Text = statusMsg;
            }
        }

        protected void btnMirthLogout_Click(object sender, EventArgs e)
        {
            MirthRestApiVM vm = (MirthRestApiVM)Session[SessionKey_MirthConfigMap_Mirth_VM];
            if ((vm == null) || vm.Logout(out string statusMsg))
            {
                MirthUserLoggedOut();

                Session[SessionKey_MirthConfigMap_Mirth_VM] = null;
                Session[SessionKey_MirthConfigMap_Mirth_LoggedInUser] = null;

                ClearStatusMessages();
            }
            else
            {
                lblStatusMsg.Text = statusMsg;
            }
        }

        protected void btnGetMirthConfigMap_Click(object sender, EventArgs e)
        {
            MirthRestApiVM vm = (MirthRestApiVM)Session[SessionKey_MirthConfigMap_Mirth_VM];
            if (vm != null)
            {
                XmlDocument doc = vm.GetConfigMap();

                DataSet dsConfigMap = new DataSet();
                using (XmlReader xr = new XmlNodeReader(doc.DocumentElement))
                {
                    dsConfigMap.ReadXml(xr);
                }

                Session[SessionKey_MirthConfigMap_ConfigMap] = dsConfigMap.Tables[0].DefaultView;

                dsConfigMap.Tables[0].DefaultView.Sort = "key";
                gridMirthConfigMap.DataSource = dsConfigMap.Tables[0].DefaultView;
                gridMirthConfigMap.DataBind();
            }
        }

        protected void lbtnDownloadMirthConfigMap_Click(object sender, EventArgs e)
        {
            string filename = "MirthConfigMap.csv";

            if (!string.IsNullOrEmpty(hfMirthConfigMap.Value))
            {
                HTTPUtilities.Download(hfMirthConfigMap.Value, filename, Response);
            }
        }

        protected void btnAddMirthConfigMapEntry_Click(object sender, EventArgs e)
        {
            string key = tbAddMirthConfigMapKey.Value;
            string value = tbAddMirthConfigMapValue.Value;
            string comment = tbAddMirthConfigMapComment.Value;

            if (!string.IsNullOrEmpty(key))
            {
                MirthRestApiVM vm = (MirthRestApiVM)Session[SessionKey_MirthConfigMap_Mirth_VM];
                if (vm != null)
                {
                    if (!vm.ConfigMapUpdateEntry(key, value, comment, false, false, out string result))
                    {
                        lblMirthConfigMapStatus.Text = result;
                    }
                }

                btnGetMirthConfigMap_Click(sender, e);
            }
            else
            {
                lblMirthConfigMapStatus.Text = "A value must be specified for [Key]";
            }
        }

        protected void btnAddRemitInboundSAAdd_Click(object sender, EventArgs e)
        {
            string sa = tbAddRemitInboundSASA.Value;
            string name = tbAddRemitInboundSAName.Value;
            string contacts = tbAddRemitInboundSAContacts.Value;

            if (!string.IsNullOrEmpty(sa))
            {
                MirthRestApiVM vm = (MirthRestApiVM)Session[SessionKey_MirthConfigMap_Mirth_VM];
                if (vm != null)
                {
                    string loggedInUser = (string)Session[SessionKey_MirthConfigMap_Mirth_LoggedInUser];

                    if (!vm.AddTrizettoRemitInboundSA(sa, name, contacts, loggedInUser, out string result))
                    {
                        lblMirthConfigMapStatus.Text += result;
                    }
                    else
                    {
                        btnGetMirthConfigMap_Click(sender, e);
                    }
                }
            }
            else
            {
                lblMirthConfigMapStatus.Text = "A value must be specified for [SA]";
            }
        }

        protected void gridMirthConfigMap_Sorting(object sender, GridViewSortEventArgs e)
        {
            GridView grid = sender as GridView;
            DataView data = Session[SessionKey_MirthConfigMap_ConfigMap] as DataView;

            // if sorting on this column for the first time, always sort ascending (default)
            // else flip the current order
            if (data.Sort == e.SortExpression &&
                e.SortDirection == SortDirection.Ascending)
            {
                data.Sort = e.SortExpression + " DESC";
            }
            else
                data.Sort = e.SortExpression;

            grid.DataSource = data;
            grid.DataBind();
        }

        private void MirthUserLoggedIn()
        {
            btnMirthLogin.Enabled = false;
            btnMirthLogout.Enabled = true;
            btnGetMirthConfigMap.Enabled = true;
            rblistMirthEnvs.Enabled = false;

            Dictionary<string, string[]> features = Session[SessionKey_MirthConfigMap_Features] as Dictionary<string, string[]>;
            string selectedEnv = rblistMirthEnvs.SelectedItem.Text;
            if ((features?.ContainsKey(selectedEnv)).Value)
            {
                if (features[selectedEnv].Contains(btnAddTrizettoRemitInboundSA.ID))
                {
                    btnAddTrizettoRemitInboundSA.Visible = true;
                }
            }
        }

        private void MirthUserLoggedOut()
        {
            btnMirthLogin.Enabled = true;
            btnMirthLogout.Enabled = false;
            btnGetMirthConfigMap.Enabled = false;
            rblistMirthEnvs.Enabled = true;
            btnAddTrizettoRemitInboundSA.Visible = false;
        }
    }
}