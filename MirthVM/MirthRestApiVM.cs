using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace org.ochin.interoperability.OCHINInterfaceUtilities.Mirth
{
    public class MirthRestApiVM
    {
        private readonly List<MirthServer> MirthServers = new List<MirthServer>();

        public MirthRestApiVM(string serverUrls)
        {
            CreateMirthConnections(serverUrls);
        }

        private void CreateMirthConnections(string serverUrls)
        {
            string[] urls = serverUrls.Split(new char[] { ',', ';'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string url in urls)
            {
                string[] urlAlias = url.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                MirthServers.Add(new MirthServer(urlAlias[0], urlAlias.Length > 1 ? urlAlias[1] : urlAlias[0]));
            }
        }

        public bool Login(string username, string password, out string statusMsg)
        {
            statusMsg = string.Empty;
            string s;
            bool allLoggedIn = true;
            foreach (MirthServer server in MirthServers)
            {
                allLoggedIn &= server.Login(username, password, out s);
                statusMsg += server.Alias + " - " + s + Environment.NewLine;
            }

            return allLoggedIn;
        }

        public bool Logout(out string statusMsg)
        {
            statusMsg = string.Empty;
            string s;
            bool allLoggedOut = true;
            foreach (MirthServer server in MirthServers)
            {
                allLoggedOut &= server.Logout(out s);
                statusMsg += server.Alias + " - " + s + Environment.NewLine;
            }

            return allLoggedOut;
        }

        public XmlDocument GetChannelStatuses(bool includeUndeployed, bool includeInputType = false)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement servers = doc.CreateElement("servers");

            foreach (MirthServer server in MirthServers)
            {
                if (server.GetChannelStatuses(includeUndeployed) && (!includeInputType || server.GetChannels(false, true)))
                {
                    XmlNode imported = doc.ImportNode(server.ToXml().DocumentElement, true);
                    servers.AppendChild(imported);
                }
            }

            doc.AppendChild(servers);

            return doc;
        }

        public XmlDocument GetChannelTags(bool includeDesc, out HashSet<string> uniqueTags, out HashSet<string> uniqueServers, out HashSet<string> uniqueStates)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement servers = doc.CreateElement("servers");

            uniqueTags = new HashSet<string>();
            uniqueServers = new HashSet<string>();
            uniqueStates = new HashSet<string>();

            // Loop through all the servers
            foreach (MirthServer mirthServer in MirthServers)
            {
                if (mirthServer.GetChannelStatuses(true) && mirthServer.GetServerChannelTags() && (!includeDesc || mirthServer.GetChannels(false)))
                {
                    XmlDocument serverDoc = mirthServer.ToXml();

                    // Loop through all the channels in the server
                    foreach (XmlNode channel in serverDoc.SelectNodes("/server/channels/channel"))
                    {
                        string id = channel.SelectSingleNode("id")?.InnerText;

                        string server = channel.SelectSingleNode("server")?.InnerText;
                        if (!string.IsNullOrEmpty(server))
                        {
                            uniqueServers.Add(server);
                        }

                        string state = channel.SelectSingleNode("state")?.InnerText;
                        if (!string.IsNullOrEmpty(state))
                        {
                            uniqueStates.Add(state);
                        }

                        // Find all the tags that are associated to this channel
                        List<string> tagNames = new List<string>();
                        foreach (var channelTag in mirthServer.ChannelTags.Where(ct => ct.channelIds.Contains(id)))
                        {
                            tagNames.Add(channelTag.name);

                            uniqueTags.Add(channelTag.name);
                        }

                        // Add the tag names to the channel
                        XmlElement tags = serverDoc.CreateElement("tags");
                        tags.InnerText = string.Join(" | ", tagNames);
                        channel.AppendChild(tags);
                    }

                    XmlNode imported = doc.ImportNode(serverDoc.DocumentElement, true);
                    servers.AppendChild(imported);
                }
            }

            doc.AppendChild(servers);

            return doc;
        }

        public XmlDocument GetConfigMap()
        {
            XmlDocument doc = new XmlDocument();

            // Loop through all the servers
            foreach (MirthServer mirthServer in MirthServers)
            {
                if (mirthServer.ConfigMapGet())
                {
                    doc = mirthServer.ConfigMap.GetSimplifiedXml();
                }
            }

            return doc;
        }

        //public bool AddConfigMapEntry(string key, string value, string comment, out string result)
        //{
        //    bool res = true;
        //    result = string.Empty;

        //    // Loop through all the servers
        //    foreach (MirthServer mirthServer in MirthServers)
        //    {
        //        if (mirthServer.ConfigMap == null)
        //        {
        //            mirthServer.GetConfigMap();
        //        }

        //        if (mirthServer.ConfigMap != null)
        //        {
        //            mirthServer.ConfigMap.AddEntry(key, value, comment);

        //            res &= mirthServer.PutConfigMap(out string resultString);

        //            if (!string.IsNullOrEmpty(resultString))
        //                result += mirthServer.Alias + " - " + resultString + Environment.NewLine;
        //        }
        //    }
        //    return res;
        //}

        public bool ConfigMapGetEntry(string key, out string result)
        {
            bool res = false;
            result = string.Empty;

            // Get the value from the first Mirth server
            if (MirthServers.Count > 0)
            {
                res = MirthServers[0].ConfigMapGetEntry(key, out result);
            }
            return res;
        }

        public bool ConfigMapUpdateEntry(string key, string value, string comment, bool appendValue, bool appendComment, out string result)
        {
            bool res = true;
            result = string.Empty;

            // Loop through all the servers
            foreach (MirthServer mirthServer in MirthServers)
            {
                res &= mirthServer.ConfigMapUpdateEntry(key, value, comment, appendValue, appendComment, out string resultString);

                if (!string.IsNullOrEmpty(resultString))
                    result += mirthServer.Alias + " - " + resultString + Environment.NewLine;
            }
            return res;
        }

        public bool ConfigMapRemoveEntry(string key, out string result)
        {
            bool res = true;
            result = string.Empty;

            // Loop through all the servers
            foreach (MirthServer mirthServer in MirthServers)
            {
                res &= mirthServer.ConfigMapRemoveEntry(key, out string resultString);

                if (!string.IsNullOrEmpty(resultString))
                    result += mirthServer.Alias + " - " + resultString + Environment.NewLine;
            }
            return res;
        }

        public bool AddTrizettoRemitInboundSA(string sa, string name, string contacts, string loggedInUser, out string result)
        {
            bool res = true;
            result = string.Empty;

            string gatewayUser = "C" + sa.PadLeft(3, '0');            
            string comment = $"Added by user [{loggedInUser}] on {DateTime.Now}";

            string key = sa + ".gatewayUser";
            string value = gatewayUser;

            if (res &= ConfigMapUpdateEntry(key, value, comment, false, true, out result))
            {
                if (!string.IsNullOrEmpty(name))
                {
                    key = sa + ".name";
                    value = name;
                    res &= ConfigMapUpdateEntry(key, value, comment, false, true, out result);
                }

                if (!string.IsNullOrEmpty(contacts))
                {
                    key = sa + ".contacts";
                    value = contacts;
                    res &= ConfigMapUpdateEntry(key, value, comment, false, true, out result);
                }

                // Add SA to filter list
                res &= ConfigMapUpdateEntry("Gateway EDI remit Inbound - ALL SAs_SAFilter", "," + sa, Environment.NewLine + $"SA {sa} " + comment, true, true, out result);
            }

            return res;
        }

        public bool RemoveTrizettoRemitInboundSA(string sa, string loggedInUser, out string result)
        {
            bool res = true;
            result = string.Empty;

            string comment = $"Removed by user [{loggedInUser}] on {DateTime.Now}";
            string key = sa + ".gatewayUser";

            if (res &= ConfigMapRemoveEntry(key, out result))
            {
                key = sa + ".name";
                res &= ConfigMapRemoveEntry(key, out result);

                key = sa + ".contacts";
                res &= ConfigMapRemoveEntry(key, out result);

                // Remove SA from filter list
                string configKey = "Gateway EDI remit Inbound - ALL SAs_SAFilter";
                if (res &= ConfigMapGetEntry(configKey, out string saFilter))
                {
                    var newSAList = string.Join(",", saFilter.Split(',').Where(s => s != sa));
                    res &= ConfigMapUpdateEntry(configKey, newSAList, Environment.NewLine + $"SA {sa} " + comment, true, true, out result);
                }
            }

            return res;
        }

        public bool StartChannels(List<string> channelIds, bool returnErrors, out string statusMsg)
        {
            statusMsg = string.Empty;
            string s;
            bool allStarted = true;
            foreach (MirthServer server in MirthServers)
            {
                allStarted &= server.StartChannels(channelIds, returnErrors, out s);
                statusMsg += server.Alias + " - " + s + Environment.NewLine;
            }

            return allStarted;
        }

        public bool StopChannels(List<string> channelIds, bool returnErrors, out string statusMsg)
        {
            statusMsg = string.Empty;
            string s;
            bool allStarted = true;
            foreach (MirthServer server in MirthServers)
            {
                allStarted &= server.StopChannels(channelIds, returnErrors, out s);
                statusMsg += server.Alias + " - " + s + Environment.NewLine;
            }

            return allStarted;
        }
    }
}
