using System.Collections.Generic;
using NUnit.Framework;
using Next;
using Next.Dtos;

namespace NextTests.Helpers
{
    public class NextTestsBase
    {
        protected NextClient LoggedInClient
        {
            get
            {
                var nextClient = NextClient.TestClient;
                Assert.IsTrue(nextClient.Login(Credentials.Username, Credentials.Password).Result); // this hangs
                return nextClient;
            }
        }

        public Credentials Credentials { get { return Credentials.Load(Properties.Resources.CredentialsPath); } }

        protected List<Account> Accounts
        {
            get
            {
                List<Account> accounts = LoggedInClient.Accounts().Result;
                return accounts;
            }
        }

        protected InstrumentMatch Ericcson
        {
            get { return Properties.Settings.Default.EricssonInstrumentMatch; }
        }
    }
}