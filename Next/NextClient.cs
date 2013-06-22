﻿using System;
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

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
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

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Logout
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Touch-session
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Touch()
        {
            var resource = string.Format("{0}/{1}", _login, Session.SessionKey);
            var request = new RestRequest(resource, Method.PUT);
            IRestResponse<LoggedInStatus> response = await Client.ExecuteTaskAsync<LoggedInStatus>(request);
            return response.Data.IsLoggedIn;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Realtime-access
        /// </summary>
        /// <returns></returns>
        public async Task<List<RealtimeAccesMarket>> RealtimeAccess()
        {
            var request = new RestRequest("realtime_access", Method.GET);
            IRestResponse<List<RealtimeAccesMarket>> response = await Client.ExecuteTaskAsync<List<RealtimeAccesMarket>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#News-sources
        /// </summary>
        /// <returns></returns>
        public async Task<List<NewsSource>> NewsSources()
        {
            var request = new RestRequest("news_sources", Method.GET);
            IRestResponse<List<NewsSource>> response = await Client.ExecuteTaskAsync<List<NewsSource>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#News-search
        /// </summary>
        /// <param name="query">The query string if omitted or empty all news are returned for the time period and sources.</param>
        /// <param name="sourceIds">Comma separated list of news sources. If empty or omitted all news sources that the user has the right to see is used</param>
        /// <param name="count">Max number of items in the result</param>
        /// <param name="after">Specify a starting point for the search. If empty only news from today will be included in the search.</param>
        /// <returns></returns>
        public string SearchNews(string query = null, string[] sourceIds = null, int count = int.MaxValue, DateTime? after = null)
        {
            var request = new RestRequest("news_items", Method.GET);
            if (!string.IsNullOrEmpty(query))
                request.AddParameter("query", query);
            if (sourceIds != null && sourceIds.Any())
                request.AddParameter("sourceids", string.Join(",", sourceIds));
            if (count < int.MaxValue)
                request.AddParameter("count", count);
            if (after != null)
                request.AddParameter("after", after.ToString());
            IRestResponse restResponse = Client.Execute(request);
            return restResponse.Content;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#News-item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string NewsItem(string id)
        {
            var request = new RestRequest("news_items/" + id, Method.GET);
            IRestResponse restResponse = Client.Execute(request);
            return restResponse.Content;
        }
    }
}
