using System.IO;
using System.Xml.Serialization;

namespace NextTests.Helpers
{
    public class Credentials
    {
        public static Credentials Load(string fileName)
        {
            if (!File.Exists(fileName))
                return new Credentials { Username = "file", Password = "missing" };
            var serializer = new XmlSerializer(typeof(Credentials));
            using (FileStream readAllText = File.Open(fileName, FileMode.Open))
            {
                return (Credentials) serializer.Deserialize(readAllText);
            }
        }

        public string Username { get; set; }
        public string Password { get; set; }

        public void Save(string fileName)
        {
            var serializer = new XmlSerializer(typeof(Credentials));
            using (FileStream fileStream = File.OpenWrite(fileName))
            {
                serializer.Serialize(fileStream,this);
            }
        }
    }
}