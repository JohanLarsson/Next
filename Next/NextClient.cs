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

        }

        private RestClient _restClient
        {
            get
            {
                var client = new RestClient(Properties.Settings.Default.BaseUrl + "/" + Properties.Settings.Default.Version + "/");
                if(_loginResult!=null)
                    client.Authenticator = new HttpBasicAuthenticator(_loginResult.session_key, _loginResult.session_key);
                return client;
            }
        }
        private LoginResult _loginResult;
        private const string _login = "login";
        public async Task<bool> Login(string username, string password)
        {
            var request = new RestRequest(_login, Method.POST);
            request.AddParameter("service", "NEXTAPI");
            request.AddParameter("auth", Encrypt(username, password));
            IRestResponse<LoginResult> response = await _restClient.ExecuteTaskAsync<LoginResult>(request);
            if (response.Data.session_key != null)
            {
                _loginResult = response.Data;
                return true;
            }
            return false;
        }

        public async Task<bool> Logout()
        {
            if (_loginResult == null)
                return true;
            var resource = string.Format("{0}/{1}", _login, _loginResult.session_key);
            var restRequest = new RestRequest(resource, Method.DELETE);
            IRestResponse<LoggedInStatus> response = await _restClient.ExecuteTaskAsync<LoggedInStatus>( restRequest);
            if (response.Data.IsLoggedIn)
                return false; //This is probably an exception
            _loginResult = null;
            _restClient.Authenticator = null;
            return !response.Data.IsLoggedIn;
        }

        public async Task<bool> Touch()
        {
            var resource = string.Format("{0}/{1}", _login, _loginResult.session_key);
            var request = new RestRequest(resource, Method.PUT);
            IRestResponse<LoggedInStatus> response = await _restClient.ExecuteTaskAsync<LoggedInStatus>(request);
            return response.Data.IsLoggedIn;
        }

        private static RSAParameters _publicKey;
        public static RSAParameters PublicKey
        {
            get
            {
                if (_publicKey.Equals(default(RSAParameters)))
                    _publicKey = Properties.Settings.Default.PublicKey.Deserialize<RSAParameters>();
                return _publicKey;
            }
        }

        public string Encrypt(string username, string password)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();

            RSAParameters parameters = NextClient.PublicKey;

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
