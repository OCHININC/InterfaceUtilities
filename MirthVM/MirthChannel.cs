﻿using System;
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
        public string State { get; private set; }
        public string Server { get; private set; }

        public MirthChannel(string id, string name, string state, string server)
        {
            ChannelId = id;
            Name = name;
            State = state;
            Server = server;
        }

        public MirthChannel(string xml, string server)
        {
            Server = server;
            Parse(xml);
        }

        public void Parse(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            ChannelId = doc.SelectSingleNode("/dashboardStatus/channelId")?.InnerText;
            Name = doc.SelectSingleNode("/dashboardStatus/name")?.InnerText;
            State = doc.SelectSingleNode("/dashboardStatus/state")?.InnerText;
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

            doc.AppendChild(channel);

            return doc;
        }

        public override string ToString()
        {
            return ChannelId + "|" + Name + "|" + State;
        }
    }
}