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
    public class MirthServer
    {
        // Using a Dictionary for "indexing" purposes
        public Dictionary<string, MirthChannel> Channels { get; private set; }
        public List<MirthChannelTag> ChannelTags { get; private set; }
        public MirthConfigMap ConfigMap { get; private set; }

        public MirthConnection MirthConnection { get; private set; }

        public string Alias { get; private set; }

        public MirthServer(string url, string alias)
        {
            Channels = new Dictionary<string, MirthChannel>();
            ChannelTags = new List<MirthChannelTag>();

            Alias = alias;
            InitConnection(url);
        }

        ~MirthServer()
        {
            MirthConnection?.Dispose();
        }

        private void InitConnection(string baseUrl)
        {
            MirthConnection = new MirthConnection(baseUrl);
        }

        private void ParseChannels(string xml, bool updateOnly = false)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode channel in doc.SelectNodes("/list/channel"))
            {
                string id = channel.SelectSingleNode("descendant::id")?.InnerText;
                string name = channel.SelectSingleNode("descendant::name")?.InnerText;
                string description = channel.SelectSingleNode("descendant::description")?.InnerText;
                string sourceConnectorTransportName = channel.SelectSingleNode("descendant::sourceConnector/transportName")?.InnerText;

                if (Channels.ContainsKey(id))
                {
                    Channels[id].Description = description;
                    Channels[id].SourceConn.TransportName = sourceConnectorTransportName;
                }
                else if (!updateOnly)
                {
                    MirthChannel c = new MirthChannel(id, name, Alias, string.Empty, description);
                    c.SourceConn.TransportName = sourceConnectorTransportName;
                    Channels.Add(c.ChannelId, c);
                }
            }
        }

        private void ParseChannelStatuses(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode channel in doc.SelectNodes("/list/dashboardStatus[statusType='CHANNEL']|/list/com.mirth.connect.plugins.clusteringadvanced.shared.ClusterDashboardStatus[statusType='CHANNEL']"))
            {
                string id = channel.SelectSingleNode("descendant::channelId")?.InnerText;
                string name = channel.SelectSingleNode("descendant::name")?.InnerText;
                string state = channel.SelectSingleNode("descendant::state")?.InnerText;

                if (Channels.ContainsKey(id))
                {
                    Channels[id].State = state;
                }
                else
                {
                    MirthChannel c = new MirthChannel(id, name, Alias, state);
                    Channels.Add(c.ChannelId, c);
                }
            }
        }

        private void ParseChannelTags(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            ChannelTags.Clear();

            foreach (XmlNode channel in doc.SelectNodes("/set/channelTag"))
            {
                XmlSerializer xs = new XmlSerializer(typeof(MirthChannelTag));
                MirthChannelTag ct = (MirthChannelTag)xs.Deserialize(new XmlNodeReader(channel));
                ct.server = Alias;
                ChannelTags.Add(ct);
            }
        }

        public bool Login(string username, string password, out string statusMsg)
        {
            return MirthConnection.Login(username, password, out statusMsg);
        }

        public bool Logout(out string statusMsg)
        {
            return MirthConnection.Logout(out statusMsg);
        }

        public bool GetChannels(bool pollingOnly, bool updateOnly = false)
        {
            bool res = MirthConnection.GetChannels(pollingOnly, out string channels);
            if (res)
            {
                ParseChannels(channels, updateOnly);
            }
            return res;
        }

        public bool GetChannelStatuses(bool includeUndeployed)
        {
            bool res = MirthConnection.GetChannelsStatuses(includeUndeployed, out string statuses);
            if (res)
            {
                ParseChannelStatuses(statuses);
            }
            return res;
        }

        public bool GetServerChannelTags()
        {
            bool res = MirthConnection.GetServerChannelTags(out string tags);
            if (res)
            {
                ParseChannelTags(tags);
            }
            return res;
        }

        public bool ConfigMapGet()
        {
            bool res = MirthConnection.GetConfigMap(out string configMap);
            if (res)
            {
                XmlSerializer xs = new XmlSerializer(typeof(MirthConfigMap));
                using (TextReader tr = new StringReader(configMap))
                {
                    ConfigMap = (MirthConfigMap)xs.Deserialize(tr);
                }
            }
            return res;
        }

        public bool ConfigMapPut(out string resultString)
        {
            return MirthConnection.PutConfigMap(ConfigMap.GetXml().OuterXml, out resultString);
        }

        public bool ConfigMapGetEntry(string key, out string resultString)
        {
            bool res = false;
            resultString = string.Empty;

            if (ConfigMap == null)
            {
                ConfigMapGet();
            }

            if (ConfigMap != null)
            {
                resultString = ConfigMap.GetEntry(key);
                res = true;
            }

            return res;
        }

        public bool ConfigMapUpdateEntry(string key, string value, string comment, bool appendValue, bool appendComment, out string resultString)
        {
            bool res = false;
            resultString = string.Empty;

            if (ConfigMap == null)
            {
                ConfigMapGet();
            }

            if (ConfigMap != null)
            {
                ConfigMap.UpdateEntry(key, value, comment, appendValue, appendComment);

                res &= ConfigMapPut(out string result);

                if (!string.IsNullOrEmpty(resultString))
                    resultString += Alias + " - " + result + Environment.NewLine;
            }

            return res;
        }

        public bool ConfigMapRemoveEntry(string key, out string resultString)
        {
            bool res = false;
            resultString = string.Empty;

            if (ConfigMap == null)
            {
                ConfigMapGet();
            }

            if (ConfigMap != null)
            {
                ConfigMap.RemoveEntry(key);

                res &= ConfigMapPut(out string result);

                if (!string.IsNullOrEmpty(resultString))
                    resultString += Alias + " - " + result + Environment.NewLine;
            }

            return res;
        }

        public bool StartChannels(List<string> channelIds, bool returnErrors, out string statusMsg)
        {
            return MirthConnection.StartChannels(channelIds, returnErrors, out statusMsg);
        }

        public bool StopChannels(List<string> channelIds, bool returnErrors, out string statusMsg)
        {
            return MirthConnection.StopChannels(channelIds, returnErrors, out statusMsg);
        }

        public XmlDocument ToXml()
        {
            XmlDocument doc = new XmlDocument();
            var server = doc.CreateElement("server");

            var alias = doc.CreateElement("alias");
            alias.InnerText = Alias;
            server.AppendChild(alias);

            var channels = doc.CreateElement("channels");

            foreach (var channel in Channels)
            {
                var imported = doc.ImportNode(channel.Value.ToXml().DocumentElement, true);
                channels.AppendChild(imported);
            }

            server.AppendChild(channels);

            var channelTags = doc.CreateElement("channelTags");

            foreach (var channelTag in ChannelTags)
            {
                XmlSerializer xs = new XmlSerializer(typeof(MirthChannelTag));
                using (StringWriter sw = new StringWriter())
                {
                    var xns = new XmlSerializerNamespaces();
                    xns.Add(string.Empty, string.Empty);

                    xs.Serialize(sw, channelTag, xns);

                    XmlDocument xd = new XmlDocument();
                    xd.LoadXml(sw.ToString());
                    var imported = doc.ImportNode(xd.DocumentElement, true);

                    channelTags.AppendChild(imported);
                }
            }

            server.AppendChild(channelTags);

            doc.AppendChild(server);

            return doc;
        }

        public override string ToString()
        {
            StringBuilder server = new StringBuilder();
            server.AppendLine("Server: " + Alias);

            foreach (var channel in Channels)
            {
                server.AppendLine(channel.Value.ToString());
            }

            return server.ToString();
        }
    }
}
