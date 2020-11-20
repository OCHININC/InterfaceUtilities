using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace org.ochin.interoperability.OCHINInterfaceUtilities.Mirth
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "map")]
    public partial class MirthConfigMap
    {
        private MirthConfigMapEntry[] entryFields;

        /// <remarks/>
        [XmlElement("entry")]
        public MirthConfigMapEntry[] Entries
        {
            get
            {
                return this.entryFields;
            }
            set
            {
                this.entryFields = value;
            }
        }

        public void AddEntry(string key, string value, string comment)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var entry = MirthConfigMapEntry.Create(key, value, comment);

                var temp = entryFields.ToList();
                temp.Add(entry);
                entryFields = temp.ToArray();
            }
        }

        public void RemoveEntry(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                Entries = Entries.Where(e => e.Key != key).ToArray();
            }
        }

        public void UpdateEntry(string key, string value, string comment, bool appendValue, bool appendComment)
        {
            if (!string.IsNullOrEmpty(key))
            {
                bool updatedEntry = false;

                for (int i = 0; i < Entries.Length; i++)
                {
                    if (Entries[i].Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        string valueCurrent = Entries[i].ConfigurationProperty?.Value;
                        string commentCurrent = Entries[i].ConfigurationProperty?.Comment;

                        if (appendValue)
                        {
                            value = valueCurrent + value;
                        }

                        if (appendComment)
                        {
                            comment = commentCurrent + comment;
                        }

                        var newEntry = MirthConfigMapEntry.Create(key, value, comment);

                        Entries[i] = newEntry;

                        updatedEntry = true;

                        break;
                    }
                }

                if (!updatedEntry)
                {
                    AddEntry(key, value, comment);
                }
            }
        }

        public string GetEntry(string key)
        {
            string value = null;

            if (!string.IsNullOrEmpty(key))
            {
                var entry = Entries.SingleOrDefault(e => e.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                value = entry?.ConfigurationProperty?.Value;
            }

            return value;
        }

        public XmlDocument GetXml()
        {
            XmlDocument doc = new XmlDocument();

            XmlSerializer xs = new XmlSerializer(typeof(MirthConfigMap));
            StringBuilder sb = new StringBuilder();
            using (TextWriter tw = new StringWriter(sb))
            {
                xs.Serialize(tw, this);
            }

            doc.LoadXml(sb.ToString());

            return doc;
        }

        public XmlDocument GetSimplifiedXml()
        {
            XmlDocument doc = new XmlDocument();

            XmlElement map = doc.CreateElement("map");

            foreach (var entry in Entries)
            {
                XmlElement e = doc.CreateElement("entry");

                XmlElement key = doc.CreateElement("key");
                key.InnerText = entry.Key;
                e.AppendChild(key);

                XmlElement value = doc.CreateElement("value");
                value.InnerText = entry.ConfigurationProperty.Value;
                e.AppendChild(value);

                XmlElement comment = doc.CreateElement("comment");
                comment.InnerText = entry.ConfigurationProperty.Comment;
                e.AppendChild(comment);

                map.AppendChild(e);
            }

            doc.AppendChild(map);

            return doc;
        }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class MirthConfigMapEntry
    {
        private string KeyField;

        private MirthConfigMapConfigurationProperty ConfigurationPropertyField;

        /// <remarks/>
        [XmlElement("string")]
        public string Key
        {
            get
            {
                return this.KeyField;
            }
            set
            {
                this.KeyField = value;
            }
        }

        /// <remarks/>
        [XmlElement("com.mirth.connect.util.ConfigurationProperty")]
        public MirthConfigMapConfigurationProperty ConfigurationProperty
        {
            get
            {
                return this.ConfigurationPropertyField;
            }
            set
            {
                this.ConfigurationPropertyField = value;
            }
        }

        public static MirthConfigMapEntry Create(string key, string value, string comment)
        {
            var entry = new MirthConfigMapEntry()
            {
                Key = key,
                ConfigurationProperty = new MirthConfigMapConfigurationProperty()
                {
                    Value = value,
                    Comment = comment
                }
            };

            return entry;
        }
    }

    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class MirthConfigMapConfigurationProperty
    {
        private string ValueField;

        private string CommentField;

        /// <remarks/>
        [XmlElement("value")]
        public string Value
        {
            get
            {
                return this.ValueField;
            }
            set
            {
                this.ValueField = value;
            }
        }

        /// <remarks/>
        [XmlElement("comment")]
        public string Comment
        {
            get
            {
                return this.CommentField;
            }
            set
            {
                this.CommentField = value;
            }
        }
    }
}
