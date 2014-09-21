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
    using System.Globalization;

    public class NextClient : IDisposable
    {
        /// <summary>
        /// Client using the testserver
        /// </summary>
        public static NextClient TestClient = new NextClient(Properties.Settings.Default.TestApiInfo);
        private static readonly CachedSearch<List<InstrumentList>> _cachedLists = new CachedSearch<List<InstrumentList>>();
        private static readonly CachedSearch<string, List<InstrumentItem>> _cachedListsItems = new CachedSearch<string, List<InstrumentItem>>();
        private static readonly CachedSearch<List<Market>> _cachedMarkets = new CachedSearch<List<Market>>();
        private static readonly CachedSearch<List<Index>> _cachedIndices = new CachedSearch<List<Index>>();
        private static readonly CachedSearch<List<TickSize>> _cachedTickSizes = new CachedSearch<List<TickSize>>();
  
        private Timer _touchTimer;
        private const string _login = "login";
        private readonly RestClient _client;
        private readonly ApiInfo _apiInfo;
        private SessionInfo _session;

        private NextClient(ApiInfo apiInfo)
        {
            _apiInfo = apiInfo;
            _client = new RestClient(_apiInfo.BaseUrl);
            PrivateFeed = new PrivateFeed(this, c => c.Session.PrivateFeed);
            PublicFeed = new PublicFeed(this, c => c.Session.PublicFeed);
        }

        public event EventHandler<bool> LoggedInChanged;

        public PrivateFeed PrivateFeed { get; private set; }

        public PublicFeed PublicFeed { get; private set; }

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
            request.AddParameter("auth", Encrypt(username, password, _apiInfo.PublicKey));
            IRestResponse<SessionInfo> response = await Client.ExecuteTaskAsync<SessionInfo>(request);
            if (response.Data.SessionKey != null)
            {
                Session = response.Data;
                var logins = new Task[2];
                logins[0] = PublicFeed.Login();
                logins[1] = PrivateFeed.Login();
                await Task.WhenAll(logins);
            }
            else
            {
                Session = null;
            }

            OnLoggedInChanged();
            return Session != null;
        }

        public static string Encrypt(string username, string password, RSAParameters rsaParameters)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            //RSAParameters parameters = NextClient.PublicKey;

            // Set the public key
            rsa.ImportParameters(rsaParameters);

            // Create timestamp (Unix timestamp in milliseconds)
            string timestamp = DateTime.UtcNow.ToUnixTimeStamp().ToString();

            // Base64 encode each field and concatenate with ":" 
            string encoded = string.Join(":", new[] { username, password, timestamp }.Select(s => s.ToBase64()));

            // Encrypt
            byte[] encrypted = rsa.Encrypt(Encoding.UTF8.GetBytes(encoded), false);

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
            {
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
            ResetTouchTimer();
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
            ResetTouchTimer();
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
        public async Task<List<NewsItem>> SearchNews(string query = null, string[] sourceIds = null, int count = int.MaxValue, DateTime? after = null)
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
            IRestResponse<List<NewsItem>> response = await Client.ExecuteTaskAsync<List<NewsItem>>(request);
            ResetTouchTimer();
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#News-item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<NewsItem> NewsItem(string id)
        {
            var request = new RestRequest("news_items/" + id, Method.GET);
            IRestResponse<NewsItem> restResponse = await Client.ExecuteTaskAsync<NewsItem>(request);
            ResetTouchTimer();
            return restResponse.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Get-accounts
        /// </summary>
        /// <returns></returns>
        public async Task<List<Account>> Accounts()
        {
            var request = new RestRequest("accounts", Method.GET);
            IRestResponse<List<Account>> executeTaskAsync = await Client.ExecuteTaskAsync<List<Account>>(request);
            ResetTouchTimer();
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
            ResetTouchTimer();
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
            ResetTouchTimer();
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
            ResetTouchTimer();
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
            ResetTouchTimer();
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
            ResetTouchTimer();
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
            ResetTouchTimer();
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Instrument-lookup
        /// </summary>
        /// <param name="identifier">Nordnet instrument identifier, example: 101</param>
        /// <param name="marketId">Nordnet market identifier, example: 11</param>
        /// <returns></returns>
        public async Task<InstrumentMatch> InstrumentSearch(string identifier, string marketId)
        {
            var request = new RestRequest("instruments", Method.GET);
            request.AddParameter("identifier", identifier);
            request.AddParameter("marketID", marketId);
            IRestResponse<InstrumentMatch> response = await Client.ExecuteTaskAsync<InstrumentMatch>(request);
            ResetTouchTimer();
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
            ResetTouchTimer();
            return response.Data;
        }

        public async Task<InstrumentMatch> InstrumentSearch(InstrumentDescriptor instrumentItem)
        {
            return await InstrumentSearch(instrumentItem.Identifier, instrumentItem.MarketId.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Chart-data
        /// </summary>
        /// <param name="identifier">Nordnet instrument identifier, example: 101</param>
        /// <param name="marketId">Nordnet market identifier, example: 11</param>
        /// <returns></returns>
        public async Task<List<Tick>> ChartData(string identifier, string marketId)
        {
            var request = new RestRequest("chart_data", Method.GET);
            request.AddParameter("identifier", identifier);
            request.AddParameter("marketID", marketId);
            IRestResponse<List<Tick>> response = await Client.ExecuteTaskAsync<List<Tick>>(request);
            ResetTouchTimer();
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Get-lists
        /// </summary>
        /// <returns></returns>
        public async Task<List<InstrumentList>> Lists()
        {
            if (_cachedLists.IsCached)
                return _cachedLists.Cache;
            var request = new RestRequest("lists", Method.GET);
            IRestResponse<List<InstrumentList>> response = await Client.ExecuteTaskAsync<List<InstrumentList>>(request);
            ResetTouchTimer();
            _cachedLists.Cache = response.Data;
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Get-list-items
        /// </summary>
        /// <returns></returns>
        public async Task<List<InstrumentItem>> ListItems(string listId)
        {
            if (_cachedListsItems.IsCached(listId))
                return _cachedListsItems[listId];
            var request = new RestRequest(string.Format("lists/{0}", listId), Method.GET);
            IRestResponse<List<InstrumentItem>> response = await Client.ExecuteTaskAsync<List<InstrumentItem>>(request);
            ResetTouchTimer();
            _cachedListsItems[listId] = response.Data;
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Get-markets
        /// </summary>
        /// <returns></returns>
        public async Task<List<Market>> Markets()
        {
            if (_cachedMarkets.IsCached)
                return _cachedMarkets.Cache;
            var request = new RestRequest("markets", Method.GET);
            IRestResponse<List<Market>> response = await Client.ExecuteTaskAsync<List<Market>>(request);
            ResetTouchTimer();
            _cachedMarkets.Cache = response.Data;
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
            ResetTouchTimer();
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Get-indices
        /// </summary>
        /// <returns></returns>
        public async Task<List<Index>> Indices()
        {
            if (_cachedIndices.IsCached)
                return _cachedIndices.Cache;
            var request = new RestRequest("indices", Method.GET);
            IRestResponse<List<Index>> response = await Client.ExecuteTaskAsync<List<Index>>(request);
            ResetTouchTimer();
            _cachedIndices.Cache = response.Data;
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Get-ticksize-table
        /// </summary>
        /// <returns></returns>
        public async Task<List<TickSize>> TickSizes(string instrumentId)
        {
            if (_cachedTickSizes.IsCached)
                return _cachedTickSizes.Cache;
            var request = new RestRequest("ticksizes/" + instrumentId, Method.GET);
            IRestResponse<List<TickSize>> response = await Client.ExecuteTaskAsync<List<TickSize>>(request);
            ResetTouchTimer();
            _cachedTickSizes.Cache = response.Data;
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
            ResetTouchTimer();
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
            ResetTouchTimer();
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
            ResetTouchTimer();
            return response.Data;
        }

        /// <summary>
        /// https://api.test.nordnet.se/projects/api/wiki/REST_API_documentation#Related-markets
        /// </summary>
        /// <returns></returns>
        public async Task<List<RelatedMarket>> RelatedMarkets(string marketId, string identifier)
        {
            var request = new RestRequest("related_markets", Method.GET);
            request.AddParameter("marketID", marketId);
            request.AddParameter("identifier", identifier);
            IRestResponse<List<RelatedMarket>> response = await Client.ExecuteTaskAsync<List<RelatedMarket>>(request);
            ResetTouchTimer();
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
            ResetTouchTimer();
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
            ResetTouchTimer();
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
            ResetTouchTimer();
            return response.Data;
        }

        protected virtual void OnLoggedInChanged()
        {
            EventHandler<bool> handler = LoggedInChanged;
            if (handler != null) handler(this, Session != null);
            if (Session == null && _touchTimer != null)
            {
                _touchTimer.Dispose();
                _touchTimer = null;
            }
            else
            {
                ResetTouchTimer();
            }
        }

        public void Dispose()
        {
            if (PrivateFeed != null)
                PrivateFeed.Dispose();
            if (PublicFeed != null)
                PublicFeed.Dispose();
            Logout();
        }

        private void ResetTouchTimer()
        {
            if (_touchTimer != null)
            {
                _touchTimer.Dispose();
            }
            var time = TimeSpan.FromSeconds(Session.ExpiresIn - 10);
            _touchTimer = new Timer(
                _ => Touch(),
                null,
                time,
                time);
        }
    }
}
