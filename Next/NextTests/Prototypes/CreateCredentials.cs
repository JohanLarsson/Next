using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NextTests.Helpers;

namespace NextTests.Prototypes
{
    class Misc
    {
        [Test, Explicit]
        public void CreateCredentials()
        {
            Credentials credentials = new Credentials {Username = "user", Password = "pw"};
            credentials.Save(Properties.Resources.CredentialsPath);
        }
    }
}
