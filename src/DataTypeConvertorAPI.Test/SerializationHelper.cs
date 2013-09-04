using System.IO;
using System.Xml.Serialization;

namespace DataTypeConvertorAPI.Test
{
    public static class SerializationHelper
    {
        public static string Serialize<T>(T obj)
        {
            var outStream = new StringWriter();
            var ser = new XmlSerializer(typeof(T));
            ser.Serialize(outStream, obj);
            return outStream.ToString();
        }

        public static T Deserialize<T>(string serialized)
        {
            var inStream = new StringReader(serialized);
            var ser = new XmlSerializer(typeof(T));
            return (T)ser.Deserialize(inStream);
        }
    }
}