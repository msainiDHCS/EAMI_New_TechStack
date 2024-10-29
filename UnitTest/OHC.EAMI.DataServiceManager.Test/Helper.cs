using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace OHC.EAMI.DataServiceManager.Test
{
    internal static class Helper
    {
        internal static T DeserializeXMLFileToObject<T>(string XmlFilename)
        {
            T returnObject = default(T);
            if (string.IsNullOrEmpty(XmlFilename)) return default(T);

            try
            {
                StreamReader xmlStream = new StreamReader(XmlFilename);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                returnObject = (T)serializer.Deserialize(xmlStream);
            }
            catch (Exception ex)
            {
                //ExceptionLogger.WriteExceptionToConsole(ex, DateTime.Now);
            }
            return returnObject;
        }


        internal static object Deserialize(string xml, Type toType)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(toType);
                return deserializer.ReadObject(stream);
            }
        }


        internal static string Serialize(object obj, Type type)
        {
            DataContractSerializer serializer = new DataContractSerializer(type);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, obj);
                //string xml =  @"<?xml version=""1.0"" encoding=""utf-8""?>" + Encoding.UTF8.GetString(memoryStream.GetBuffer());
                return Encoding.UTF8.GetString(memoryStream.GetBuffer());
            }
        }

        internal static T GetCloneCopy<T>(T obj)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(Serialize(obj, obj.GetType()));
            return (T)Deserialize(xdoc.OuterXml, typeof(T));
        }
    }
}
