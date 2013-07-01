﻿using System;
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
        public static ApiInfo TestApiInfo()
        {
            return Properties.Settings.Default.TestApiInfo;
        }
        [XmlIgnore]
        public string BaseUrl { get { return string.Format(@"{0}/{1}/{2}", Host, Path, Version); } }
        public string Host { get; set; }
        public string Path { get; set; }
        public string Version { get; set; }
        public RSAParameters PublicKey { get; set; }
        
    }
}
