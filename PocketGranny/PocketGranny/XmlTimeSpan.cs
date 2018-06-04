using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PocketGranny
{
    public class XmlTimeSpan : IXmlSerializable
    {
        private TimeSpan _time;

        public static implicit operator XmlTimeSpan(TimeSpan time)
        {
            return new XmlTimeSpan { _time = time };
        }

        public static implicit operator TimeSpan(XmlTimeSpan time)
        {
            return time._time;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            _time = TimeSpan.Parse((string)reader.ReadElementContentAs(typeof(string), null));
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteValue(_time.ToString());
        }
    }
}