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

        public MirthConnection MirthConnection { get; private set; }

        public string Alias { get; private set; }

        public MirthServer(string url)
        {
            Channels = new Dictionary<string, MirthChannel>();
            ChannelTags = new List<MirthChannelTag>();

            Alias = url;
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

        private void ParseChannels(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode channel in doc.SelectNodes("/list/channel"))
            {
                string id = channel.SelectSingleNode("descendant::id")?.InnerText;
                string name = channel.SelectSingleNode("descendant::name")?.InnerText;
                string description = channel.SelectSingleNode("descendant::description")?.InnerText;

                if (Channels.ContainsKey(id))
                {
                    Channels[id].Description = description;
                }
                else
                {
                    MirthChannel c = new MirthChannel(id, name, Alias, string.Empty, description);
                    Channels.Add(c.ChannelId, c);
                }
            }
        }

        private void ParseChannelStatuses(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            foreach (XmlNode channel in doc.SelectNodes("/list/dashboardStatus[statusType='CHANNEL']"))
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

        public bool GetChannels(bool pollingOnly)
        {
            bool res = MirthConnection.GetChannels(pollingOnly, out string channels);
            if (res)
            {
                ParseChannels(channels);
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
