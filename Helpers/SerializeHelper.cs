using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Aisger.Helpers
{
    public class SerializeHelper
    {
        public static string Serialize<T>(T value)
        {

            if (value == null)
            {
                return null;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
            //settings.Indent = false;
            ///settings.OmitXmlDeclaration = false;

            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, value);
                }
                return textWriter.ToString();
            }
        }

        public static string SerializeDataContract<T>(T value)
        {
            if (value == null)
            {
                return null;
            }

            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
//            XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateTextWriter(someStream, Encoding.UTF8);
//            dcs.WriteObject(xdw, p);



            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8; //new UTF8Encoding(false, false); // no BOM in a .NET string
            settings.Indent = false;
            settings.OmitXmlDeclaration = false;

            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlDictionaryWriter.Create(textWriter, settings))
                {
                    serializer.WriteObject(xmlWriter, value);
                }
                return textWriter.ToString();
            }
        }


        public static T Deserialize<T>(string xml)
        {

            if (string.IsNullOrEmpty(xml))
            {
                return default(T);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlReaderSettings settings = new XmlReaderSettings();
            // No settings need modifying here

            using (StringReader textReader = new StringReader(xml))
            {
                using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
                {
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
        }

        public static T DeserializeDataContract<T>(string xml)
        {

            if (string.IsNullOrEmpty(xml))
            {
                return default(T);
            }

            DataContractSerializer serializer = new DataContractSerializer(typeof(T));

            XmlReaderSettings settings = new XmlReaderSettings();
            // No settings need modifying here

            using (StringReader textReader = new StringReader(xml))
            {
                using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
                {
                    return (T)serializer.ReadObject(xmlReader);
                }
            }
        }
    }
}