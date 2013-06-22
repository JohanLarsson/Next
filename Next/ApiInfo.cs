using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Next
{
    public class ApiInfo
    {
        [XmlIgnore]
        public string BaseUrl { get { return Url + "/" + Version; } }
        public string Url { get; set; }
        public string Version { get; set; }
        public RSAParameters PublicKey { get; set; }
    }
}
