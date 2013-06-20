using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using RestSharp;

namespace Next
{
    public class NextClient
    {
        public NextClient()
        {
            _restClient = new RestClient(Properties.Settings.Default.BaseUrl + "/" + Properties.Settings.Default.Version + "/");
            _restClient.AddDefaultHeader("Accept", "application/json");
            _restClient.AddDefaultHeader("ContentType", "application/x-www-form-urlencoded");
        }
        private RestClient _restClient;
        private LoginResult _loginResult;

        public bool Login(string username, string password)
        {
            RestRequest restRequest = new RestRequest("login", Method.POST);
            restRequest.AddParameter("service", "NEXTAPI");
            restRequest.AddParameter("auth", Encrypt(username, password));

            IRestResponse<LoginResult> restResponse = _restClient.Execute<LoginResult>(restRequest);
            _loginResult = restResponse.Data;
            return true;
        }

        public string Logout()
        {
            RestRequest restRequest = new RestRequest(string.Format("login/{0}",_loginResult.session_key), Method.DELETE);
            IRestResponse restResponse = _restClient.Execute(restRequest);
            return restResponse.Content;
        }

        private static RSAParameters _rsaParameters;
        public static RSAParameters RsaParameters
        {
            get
            {
                if (_rsaParameters.Equals(default(RSAParameters)))
                    _rsaParameters = Properties.Settings.Default.PublicKey.Deserialize<RSAParameters>();
                return _rsaParameters;
            }
        }

        public string Encrypt(string username, string password)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

            RSAParameters parameters = NextClient.RsaParameters;

            // Set the public key
            RSA.ImportParameters(parameters);

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
