using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NextView.Tests
{
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    using NUnit.Framework;

    [Explicit]
    public class SettingsDumper
    {
        [Test]
        public void DumpUserSettings()
        {
            var userSetting = new UserSetting { UserName = "", Password = "", RememberMe = false };
            Console.WriteLine(typeof(UserSetting).FullName);
            var serializer = new XmlSerializer(typeof(UserSetting));
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer,userSetting);
                Console.Write(sb.ToString());
            }
        }
    }
}
