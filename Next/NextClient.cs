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
        public NextClient()
        {
            _restClient = new RestClient(Properties.Settings.Default.BaseUrl + "/" + Properties.Settings.Default.Version + "/");
            //_restClient.AddDefaultHeader("Accept", "application/json");
            //_restClient.AddDefaultHeader("ContentType", "application/x-www-form-urlencoded");
        }

        private RestClient _restClient;
        private LoginResult _loginResult;
        private const string _login = "login";
        public async Task<bool> Login(string username, string password)
        {
            RestRequest restRequest = new RestRequest(_login, Method.POST);
            restRequest.AddParameter("service", "NEXTAPI");
            restRequest.AddParameter("auth", Encrypt(username, password));
            IRestResponse<LoginResult> response = await _restClient.ExecuteTaskAsync<LoginResult>(restRequest);
            if (response.Data.session_key != null)
            {
                _loginResult = response.Data;
                _restClient.Authenticator = new HttpBasicAuthenticator(_loginResult.session_key, _loginResult.session_key);
                return true;
            }
            return false;
        }

        public async Task<bool> Logout()
        {
            string resource = string.Format("{0}/{1}", _login, _loginResult.session_key);
            RestRequest restRequest = new RestRequest(resource, Method.DELETE);
            IRestResponse<LogoutResult> response = await _restClient.ExecuteTaskAsync<LogoutResult>( restRequest);
            return !response.Data.logged_in;
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
