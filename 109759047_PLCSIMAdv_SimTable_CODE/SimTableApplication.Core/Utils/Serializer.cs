using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace SimTableApplication.Core.Utils
{
    /// <summary>
    /// Serialize and Deserialize Objects
    /// </summary>
    public static class Serializer
    {
        private static readonly object Locker = new object();


        /// <summary>
        /// Serialize Object of Type T into XML file
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="obj">object to be serialized</param>
        /// <param name="path">path where the XML file is stored</param>
        /// <exception cref="SerializationException"></exception>
        public static void Serialize<T>(T obj, string path)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));

                var xmlSettings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = new string(' ', 2),
                    NewLineChars = Environment.NewLine,
                    NewLineHandling = NewLineHandling.Replace
                };

                lock (Locker)
                {
                    using (StreamWriter stream = new StreamWriter(path))
                    {
                        using (XmlWriter writer = XmlWriter.Create(stream, xmlSettings))
                        {
                            serializer.Serialize(writer, obj);
                            
                        }
                    }                    
                }

            }
            catch (SerializationException)
            {               
                throw;
            }
        }

        /// <summary>
        /// Deserialize Object of Type T into Type T from a XML file
        /// </summary>
        /// <typeparam name="T">Object Type</typeparam>
        /// <param name="path">path where the XML file is stored</param>
        /// <exception cref="SerializationException"></exception>
        /// <returns></returns>
        public static T Deserialize<T>(string path) where T : class
        {
            try
            {
                lock (Locker)
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open))
                    {
                        using (TextReader rdr = new StreamReader(fs))
                        {
                            var serializer = new XmlSerializer(typeof(T));
                            return (T)serializer.Deserialize(rdr);
                        }
                    }                  
                }


            }
            catch (SerializationException)
            {              
                throw;
            }
        }

    }
}
