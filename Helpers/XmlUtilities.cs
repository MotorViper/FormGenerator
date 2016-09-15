using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Helpers
{
    /// <summary>
    ///     Utilities for use with XML.
    /// </summary>
    public static class XmlUtilities
    {
        /// <summary>
        ///     Deserializes XML.
        /// </summary>
        /// <typeparam name="T">The type of the data to deserialize.</typeparam>
        /// <param name="variable">The XML to deserialize</param>
        /// <returns>An object of type T.</returns>
        public static T Deserialize<T>(string variable)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(variable))
                return (T)serializer.Deserialize(reader);
        }

        /// <summary>
        ///     Serializes XML.
        /// </summary>
        /// <typeparam name="T">The type of the data to serialize.</typeparam>
        /// <param name="variable">The data to serialize.</param>
        /// <returns>The XML equivalent of the data.</returns>
        public static string Serialize<T>(T variable)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringWriter sww = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                serializer.Serialize(writer, variable);
                return sww.ToString();
            }
        }

        /// <summary>
        ///     Serializes XML.
        /// </summary>
        /// <typeparam name="T">The type of the data to serialize.</typeparam>
        /// <param name="variable">The data to serialize.</param>
        /// <param name="fileName">The file to save to.</param>
        /// <returns>The XML equivalent of the data.</returns>
        public static void SerializeToFile<T>(T variable, string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream file = File.Create(fileName))
                serializer.Serialize(file, variable);
        }
    }
}
