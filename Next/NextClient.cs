using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Next.Dtos;
using RestSharp;

namespace Next
{
    public class NextClient
    {
        private const string _login = "login";

        private readonly RestClient _client;
        private readonly ApiInfo _apiInfo;
        private SessionInfo _session;

        public NextClient(ApiVersion apiVersion)
        {
            if (apiVersion == ApiVersion.Test)
                _apiInfo = Properties.Settings.Default.TestApiInfo;
            else
                throw new NotImplementedException("message");

            _client = new RestClient(_apiInfo.BaseUrl);
        }

        public SessionInfo Session
        {
            get { return _session; }
            private set
            {
                _session = value;
                _client.Authenticator = value == null ? null : new HttpBasicAuthenticator(value.SessionKey, value.SessionKey);
            }
        }

        private RestClient Client
        {
            get { return _client; }
        }

        public async Task<ServiceStatus> ServiceStatus()
        {
            IRestResponse<ServiceStatus> response = await Client.ExecuteTaskAsync<ServiceStatus>(new RestRequest(Method.GET));
            return response.Data;
        }

        private Timer _touchTimer;
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
            Session = response.Data.SessionKey == null 
                ? null 
                : response.Data;

            OnLoggedInChanged();
            return Session!=null;
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
            if (Session == null) {
                Debug.WriteLine("NextClient.Logout() called with Sessoin == null.  Not logged in.");
                return true;
            }

            var resource = string.Format("{0}/{1}", _login, Session.SessionKey);
            var restRequest = new RestRequest(resource, Method.DELETE);
            
            IRestResponse<LoggedInStatus> response = await Client.ExecuteTaskAsync<LoggedInStatus>(restRequest);
            if (response.Data.IsLoggedIn)
                return false; //This is probably an exception
            
            Session = null;
            OnLoggedInChanged();
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

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Get-accounts
        /// </summary>
        /// <returns></returns>
        public async Task<List<Account>> Accounts()
        {
            var request = new RestRequest("accounts", Method.GET);
            IRestResponse<List<Account>> executeTaskAsync = await Client.ExecuteTaskAsync<List<Account>>(request);
            return executeTaskAsync.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Get-account-summary
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<AccountSummary> AccountSummary(Account account)
        {
            var request = new RestRequest("accounts/" + account.Id, Method.GET);
            IRestResponse<AccountSummary> response = await Client.ExecuteTaskAsync<AccountSummary>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Get-account-ledgers
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<List<Ledger>> AccountLedgers(Account account)
        {
            var request = new RestRequest(string.Format("accounts/{0}/ledgers", account.Id), Method.GET);
            IRestResponse<List<Ledger>> response = await Client.ExecuteTaskAsync<List<Ledger>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Get-account-positions
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<List<Position>> AccountPositions(Account account)
        {
            var request = new RestRequest(string.Format("accounts/{0}/positions", account.Id), Method.GET);
            IRestResponse<List<Position>> response = await Client.ExecuteTaskAsync<List<Position>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Exchange-orders
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<List<OrderStatus>> AccountOrders(Account account)
        {
            var request = new RestRequest(string.Format("accounts/{0}/orders", account.Id), Method.GET);
            IRestResponse<List<OrderStatus>> response = await Client.ExecuteTaskAsync<List<OrderStatus>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Exchange-trades
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<List<Trade>> AccountTrades(Account account)
        {
            var request = new RestRequest(string.Format("accounts/{0}/trades", account.Id), Method.GET);
            IRestResponse<List<Trade>> response = await Client.ExecuteTaskAsync<List<Trade>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Instrument-search
        /// </summary>
        /// <param name="query">The search string to use. Example: ERI, VOL</param>
        /// <param name="type">The type of the instrument. Example: A, O, WNT, FND, FUT</param>
        /// <param name="country">The country to search in. Example: SE, DK, NO, FI, DE, US, CA</param>
        /// <returns></returns>
        public async Task<List<InstrumentMatch>> InstrumentSearch(string query = null, string type = null, string country = null)
        {
            var request = new RestRequest("instruments", Method.GET);
            if (!string.IsNullOrEmpty(query))
                request.AddParameter("query", query);
            if (type != null)
                request.AddParameter("type", type);
            if (country != null)
                request.AddParameter("country", country);
            IRestResponse<List<InstrumentMatch>> response = await Client.ExecuteTaskAsync<List<InstrumentMatch>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Instrument-lookup
        /// </summary>
        /// <param name="identifier">Nordnet instrument identifier, example: 101</param>
        /// <param name="marketId">Nordnet market identifier, example: 11</param>
        /// <returns></returns>
        public async Task<InstrumentMatch> InstrumentSearch(string identifier, int marketId)
        {
            var request = new RestRequest("instruments", Method.GET);
            request.AddParameter("identifier", identifier);
            request.AddParameter("marketID", marketId);
            IRestResponse<InstrumentMatch> response = await Client.ExecuteTaskAsync<InstrumentMatch>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Multiple-instrument-lookup
        /// </summary>
        /// <returns></returns>
        public async Task<List<InstrumentMatch>> InstrumentSearch(InstrumentDescriptor[] descriptors)
        {
            var request = new RestRequest("instruments", Method.GET);
            var sb = new StringBuilder();
            foreach (var descriptor in descriptors)
            {
                sb.AppendFormat("{0},{1}", descriptor.MarketId, descriptor.Identifier);
                if (descriptor != descriptors.Last())
                    sb.Append(";");
            }
            request.AddParameter("list", sb.ToString());
            IRestResponse<List<InstrumentMatch>> response = await Client.ExecuteTaskAsync<List<InstrumentMatch>>(request);
            return response.Data;
        }

        public async Task<InstrumentMatch> InstrumentSearch(InstrumentItem instrumentItem)
        {
            return await InstrumentSearch(instrumentItem.Identifier, instrumentItem.MarketID);
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Chart-data
        /// </summary>
        /// <param name="identifier">Nordnet instrument identifier, example: 101</param>
        /// <param name="marketId">Nordnet market identifier, example: 11</param>
        /// <returns></returns>
        public async Task<List<Tick>> ChartData(string identifier, int marketId)
        {
            var request = new RestRequest("chart_data", Method.GET);
            request.AddParameter("identifier", identifier);
            request.AddParameter("marketID", marketId);
            IRestResponse<List<Tick>> response = await Client.ExecuteTaskAsync<List<Tick>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Get-lists
        /// </summary>
        /// <returns></returns>
        public async Task<List<InstrumentList>> Lists()
        {
            var request = new RestRequest("lists", Method.GET);
            IRestResponse<List<InstrumentList>> response = await Client.ExecuteTaskAsync<List<InstrumentList>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Get-list-items
        /// </summary>
        /// <returns></returns>
        public async Task<List<InstrumentItem>> ListItems(string listId)
        {
            var request = new RestRequest(string.Format("lists/{0}", listId), Method.GET);
            IRestResponse<List<InstrumentItem>> response = await Client.ExecuteTaskAsync<List<InstrumentItem>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Get-markets
        /// </summary>
        /// <returns></returns>
        public async Task<List<Market>> Markets()
        {
            var request = new RestRequest("markets", Method.GET);
            IRestResponse<List<Market>> response = await Client.ExecuteTaskAsync<List<Market>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Get-valid-trading-days
        /// </summary>
        /// <returns></returns>
        public async Task<List<TradingDay>> TradingDays(int marketId)
        {
            var request = new RestRequest(string.Format("markets/{0}/trading_days", marketId), Method.GET);
            IRestResponse<List<TradingDay>> response = await Client.ExecuteTaskAsync<List<TradingDay>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Get-indices
        /// </summary>
        /// <returns></returns>
        public async Task<List<Index>> Indices()
        {
            var request = new RestRequest("indices", Method.GET);
            IRestResponse<List<Index>> response = await Client.ExecuteTaskAsync<List<Index>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Get-ticksize-table
        /// </summary>
        /// <returns></returns>
        public async Task<List<TickSize>> TickSizes(string instrumentId)
        {
            var request = new RestRequest("ticksizes/" + instrumentId, Method.GET);
            IRestResponse<List<TickSize>> response = await Client.ExecuteTaskAsync<List<TickSize>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Derivatives
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> Countries(string derivativeType)
        {
            var request = new RestRequest("derivatives/" + derivativeType, Method.GET);
            IRestResponse<List<string>> response = await Client.ExecuteTaskAsync<List<string>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Derivatives
        /// </summary>
        /// <returns></returns>
        public async Task<List<InstrumentItem>> Underlyings(string derivativeType, string country)
        {
            var request = new RestRequest(string.Format("derivatives/{0}/underlyings/{1}", derivativeType, country), Method.GET);
            IRestResponse<List<InstrumentItem>> response = await Client.ExecuteTaskAsync<List<InstrumentItem>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Derivatives
        /// </summary>
        /// <returns></returns>
        public async Task<List<Derivative>> Derivatives(string derivativeType)
        {
            var request = new RestRequest(string.Format("derivatives/{0}/derivatives", derivativeType), Method.GET);
            IRestResponse<List<Derivative>> response = await Client.ExecuteTaskAsync<List<Derivative>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Related-markets
        /// </summary>
        /// <returns></returns>
        public async Task<List<RelatedMarket>> RelatedMarkets(int marketId, string identifier)
        {
            var request = new RestRequest("related_markets", Method.GET);
            request.AddParameter("marketID", marketId);
            request.AddParameter("identifier", identifier);
            IRestResponse<List<RelatedMarket>> response = await Client.ExecuteTaskAsync<List<RelatedMarket>>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Order-entry
        /// </summary>
        /// <returns></returns>
        public async Task<OrderStatus> PlaceOrder(OrderBuilder order)
        {
            var request = new RestRequest(string.Format("accounts/{0}/orders", order.Account.Id), Method.POST);
            request.AddParameter("identifier", order.Instrument.Identifier);
            request.AddParameter("marketID", order.Instrument.MainMarketId);
            request.AddParameter("price", order.Price);
            request.AddParameter("volume", order.Volume);
            request.AddParameter("side", order.Side);
            request.AddParameter("currency", order.Currency);
            if (order.ValidUntil.HasValue && !order.SmartOrder)
                request.AddParameter("valid_until", order.ValidUntil.Value.ToString("yyyy-MM-dd"));
            if (order.OpenVolume != 0)
                request.AddParameter("open_volume", order.OpenVolume);
            request.AddParameter("smart_order", order.SmartOrder);

            IRestResponse<OrderStatus> response = await Client.ExecuteTaskAsync<OrderStatus>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Order-modify
        /// </summary>
        /// <returns></returns>
        public async Task<OrderStatus> ModifyOrder(string account, string orderId, decimal newPrice, uint newVolume)
        {
            var request = new RestRequest(string.Format("accounts/{0}/orders/{1}", account, orderId), Method.PUT);
            request.AddParameter("price", newPrice);
            request.AddParameter("volume", newVolume);
            IRestResponse<OrderStatus> response = await Client.ExecuteTaskAsync<OrderStatus>(request);
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Order-delete
        /// </summary>
        /// <returns></returns>
        public async Task<OrderStatus> DeleteOrder(string account, string orderId)
        {
            var request = new RestRequest(string.Format("accounts/{0}/orders/{1}", account, orderId), Method.DELETE);
            IRestResponse<OrderStatus> response = await Client.ExecuteTaskAsync<OrderStatus>(request);
            return response.Data;
        }

        public event EventHandler<bool> LoggedInChanged;

        protected virtual void OnLoggedInChanged()
        {
            EventHandler<bool> handler = LoggedInChanged;
            if (handler != null) handler(this, this.Session!=null);
            if (Session == null && _touchTimer != null)
            {
                _touchTimer.Dispose();
                _touchTimer = null;
            }
            else
            {
                _touchTimer = new Timer((o) => Touch(), null, TimeSpan.FromSeconds(Session.ExpiresIn - 10), TimeSpan.FromSeconds(Session.ExpiresIn - 10));
            }
        }
    }
}
