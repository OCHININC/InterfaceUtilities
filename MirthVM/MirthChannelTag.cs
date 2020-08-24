using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace org.ochin.interoperability.OCHINInterfaceUtilities.Mirth
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "channelTag")]
    public partial class MirthChannelTag
    {

        private string idField;

        private string nameField;

        private string serverField;

        private string[] channelIdsField;

        private channelTagBackgroundColor backgroundColorField;

        /// <remarks/>
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string server
        {
            get
            {
                return this.serverField;
            }
            set
            {
                this.serverField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public string[] channelIds
        {
            get
            {
                return this.channelIdsField;
            }
            set
            {
                this.channelIdsField = value;
            }
        }

        /// <remarks/>
        public channelTagBackgroundColor backgroundColor
        {
            get
            {
                return this.backgroundColorField;
            }
            set
            {
                this.backgroundColorField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class channelTagBackgroundColor
    {

        private byte redField;

        private byte greenField;

        private byte blueField;

        private byte alphaField;

        /// <remarks/>
        public byte red
        {
            get
            {
                return this.redField;
            }
            set
            {
                this.redField = value;
            }
        }

        /// <remarks/>
        public byte green
        {
            get
            {
                return this.greenField;
            }
            set
            {
                this.greenField = value;
            }
        }

        /// <remarks/>
        public byte blue
        {
            get
            {
                return this.blueField;
            }
            set
            {
                this.blueField = value;
            }
        }

        /// <remarks/>
        public byte alpha
        {
            get
            {
                return this.alphaField;
            }
            set
            {
                this.alphaField = value;
            }
        }
    }


}
