using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Next.Dtos;
using RestSharp;

namespace Next
{
    public class NextClient
    {
        public NextClient(ApiVersion apiVersion)
        {
            if (apiVersion == ApiVersion.Test)          
                _apiInfo = Properties.Settings.Default.TestApiInfo;
            else
                throw new NotImplementedException("message");
        }

        private ApiInfo _apiInfo;

        private RestClient Client
        {
            get
            {
                var client = new RestClient(_apiInfo.BaseUrl);
                if (Session != null)
                    client.Authenticator = new HttpBasicAuthenticator(Session.SessionKey, Session.SessionKey);
                return client;
            }
        }

        public async Task<ServiceStatus> ServiceStatus()
        {
            IRestResponse<ServiceStatus> response = await Client.ExecuteTaskAsync<ServiceStatus>(new RestRequest(Method.GET));
            return response.Data;
        } 

        public SessionInfo Session { get; set; }
        private const string _login = "login";
        public async Task<bool> Login(string username, string password)
        {
            var request = new RestRequest(_login, Method.POST);
            request.AddParameter("service", "NEXTAPI");
            request.AddParameter("auth", Encrypt(username, password));
            IRestResponse<SessionInfo> response = await Client.ExecuteTaskAsync<SessionInfo>(request);
            if (response.Data.SessionKey == null)
            {
                Session = null;
                return false;
            }
            Session = response.Data;
            return true;
        }

        public async Task<bool> Logout()
        {
            if (Session == null)
                return true; // Exception?
            var resource = string.Format("{0}/{1}", _login, Session.SessionKey);
            var restRequest = new RestRequest(resource, Method.DELETE);
            IRestResponse<LoggedInStatus> response = await Client.ExecuteTaskAsync<LoggedInStatus>(restRequest);
            if (response.Data.IsLoggedIn)
                return false; //This is probably an exception
            Session = null;
            Client.Authenticator = null;
            return !response.Data.IsLoggedIn;
        }

        public async Task<bool> Touch()
        {
            var resource = string.Format("{0}/{1}", _login, Session.SessionKey);
            var request = new RestRequest(resource, Method.PUT);
            IRestResponse<LoggedInStatus> response = await Client.ExecuteTaskAsync<LoggedInStatus>(request);
            return response.Data.IsLoggedIn;
        }

        public async Task<List<RealtimeAccesMarket>> RealtimeAccess()
        {
            var request = new RestRequest("realtime_access", Method.GET);
            IRestResponse<List<RealtimeAccesMarket>> response = await Client.ExecuteTaskAsync<List<RealtimeAccesMarket>>(request);
            return response.Data;
        }

        public string Encrypt(string username, string password)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

            //RSAParameters parameters = NextClient.PublicKey;

            // Set the public key
            RSA.ImportParameters(_apiInfo.PublicKey);

            // Create timestamp (Unix timestamp in milliseconds)
            string timestamp = DateTime.UtcNow.ToUnixTimeStamp().ToString();

            // Base64 encode each field and concatenate with ":" 
            string encoded = string.Join(":", new[] { username, password, timestamp }.Select(s => s.ToBase64()));

            // Encrypt
            byte[] encrypted = RSA.Encrypt(Encoding.UTF8.GetBytes(encoded), false);

            // Base 64 encode the blob.
            string blob = Convert.ToBase64String(encrypted);
            return blob;
        }
    }
}
