using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace org.ochin.interoperability.OCHINInterfaceUtilities.Mirth
{
    public class MirthChannel
    {
        public string ChannelId { get; private set; }
        public string Name { get; private set; }
        public string State { get; set; }
        public string Server { get; private set; }
        public string Description { get; set; }
        public SourceConnector SourceConn { get; private set; }
        public List<DestinationConnector> DestinationConns { get; set; }

        public MirthChannel(string id, string name, string server, string state = "", string description = "")
        {
            ChannelId = id;
            Name = name;
            State = state;
            Server = server;
            Description = description;

            SourceConn = new SourceConnector();
            DestinationConns = new List<DestinationConnector>();
        }

        public void ParseDestinationConnectors(XmlNodeList connectors)
        {
            foreach(XmlNode connector in connectors)
            {
                string name = connector.SelectSingleNode("descendant::name").InnerText;
                string enabled = connector.SelectSingleNode("descendant::enabled").InnerText;
                string transportName = connector.SelectSingleNode("descendant::transportName").InnerText;

                DestinationConns.Add(
                    new DestinationConnector()
                    {
                        Name = name,
                        Enabled = bool.Parse(enabled),
                        TransportName = transportName
                    }
                );
            }
        }

        public XmlDocument ToXml()
        {
            XmlDocument doc = new XmlDocument();
            var channel = doc.CreateElement("channel");

            var id = doc.CreateElement("id");
            id.InnerText = ChannelId;
            channel.AppendChild(id);

            var name = doc.CreateElement("name");
            name.InnerText = Name;
            channel.AppendChild(name);

            var state = doc.CreateElement("state");
            state.InnerText = State;
            channel.AppendChild(state);

            var server = doc.CreateElement("server");
            server.InnerText = Server;
            channel.AppendChild(server);

            var description = doc.CreateElement("description");
            description.InnerText = Description;
            channel.AppendChild(description);

            var sourceConnector = doc.ImportNode(SourceConn.ToXml().DocumentElement, true);
            channel.AppendChild(sourceConnector);

            var destinationConnectors = doc.CreateElement("destinationConnectors");
            foreach (var dc in DestinationConns)
            {
                var destinationConnector = doc.ImportNode(dc.ToXml().DocumentElement, true);
                destinationConnectors.AppendChild(destinationConnector);
            }
            channel.AppendChild(destinationConnectors);

            doc.AppendChild(channel);

            return doc;
        }

        public override string ToString()
        {
            return ChannelId + "|" + Name + "|" + State;
        }

        public class SourceConnector
        {
            public string TransportName { get; set; }

            public XmlDocument ToXml()
            {
                XmlDocument doc = new XmlDocument();
                var sourceConnector = doc.CreateElement("sourceConnector");

                var transportName = doc.CreateElement("transportName");
                transportName.InnerText = TransportName;
                sourceConnector.AppendChild(transportName);

                doc.AppendChild(sourceConnector);

                return doc;
            }
        }

        public class DestinationConnector
        {
            public string Name { get; set; }
            public bool Enabled { get; set; }
            public string TransportName { get; set; }

            public XmlDocument ToXml()
            {
                XmlDocument doc = new XmlDocument();
                var destConnector = doc.CreateElement("destinationConnector");

                var name = doc.CreateElement("name");
                name.InnerText = Name;
                destConnector.AppendChild(name);

                var enabled = doc.CreateElement("enabled");
                enabled.InnerText = Enabled.ToString();
                destConnector.AppendChild(enabled);

                var transportName = doc.CreateElement("transportName");
                transportName.InnerText = TransportName;
                destConnector.AppendChild(transportName);

                doc.AppendChild(destConnector);

                return doc;
            }
        }
    }
}
