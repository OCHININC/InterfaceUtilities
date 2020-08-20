using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace org.ochin.interoperability.OCHINInterfaceUtilities.Mirth
{
    public class MirthServer
    {
        public List<MirthChannel> Channels { get; private set; }

        public MirthConnection MirthConnection { get; private set; }

        public string Alias { get; private set; }

        public MirthServer(string url)
        {
            Channels = new List<MirthChannel>();

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

        public void Parse(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            Channels.Clear();

            foreach (XmlNode channel in doc.SelectNodes("/list/dashboardStatus[statusType='CHANNEL']"))
            {
                MirthChannel c = new MirthChannel(channel.OuterXml, Alias);
                Channels.Add(c);
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

        public bool GetChannelStatuses(bool includeUndeployed)
        {
            bool res = MirthConnection.GetChannelsStatuses(includeUndeployed, out string statuses);
            if (res)
            {
                Parse(statuses);
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
                var imported = doc.ImportNode(channel.ToXml().DocumentElement, true);
                channels.AppendChild(imported);
            }

            server.AppendChild(channels);

            doc.AppendChild(server);

            return doc;
        }

        public override string ToString()
        {
            StringBuilder server = new StringBuilder();
            server.AppendLine("Server: " + Alias);

            foreach (MirthChannel channel in Channels)
            {
                server.AppendLine(channel.ToString());
            }

            return server.ToString();
        }
    }
}
