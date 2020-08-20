using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
                MirthServers.Add(new MirthServer(url));
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

        public XmlDocument GetChannelStatuses(bool includeUndeployed)
        {
            XmlDocument doc = new XmlDocument();
            var servers = doc.CreateElement("servers");

            foreach (MirthServer server in MirthServers)
            {
                if (server.GetChannelStatuses(includeUndeployed))
                {
                    var imported = doc.ImportNode(server.ToXml().DocumentElement, true);
                    servers.AppendChild(imported);
                }
            }

            doc.AppendChild(servers);

            return doc;
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
