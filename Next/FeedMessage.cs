using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Next.Dtos;
using RestSharp;
using RestSharp.Deserializers;

namespace Next
{
    public class PublicFeedMessage
    {
        private static Regex priceTickRegex = new Regex(string.Format(@"""cmd"" ?: ?""{0}""", FeedCommands.SubscribeType.Price),
            RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        public static bool IsPriceTick(string json)
        {
            return priceTickRegex.IsMatch(json);
        }

        public static PriceTick Deserialize(string json)
        {
            JsonDeserializer deserializer = new JsonDeserializer();
            PublicFeedMessage<PriceTick> publicFeedMessage = deserializer.Deserialize<PublicFeedMessage<PriceTick>>(new FakeResponse(json));
            PriceTick priceTick = publicFeedMessage.args;
            priceTick.Instrument = new InstrumentDescriptor(priceTick.m, priceTick.i);
            return priceTick;
        }
    }

    internal class FakeResponse : IRestResponse
    {
        public FakeResponse(string content)
        {
            Content = content;
        }

        public IRestRequest Request { get; set; }
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
        public string ContentEncoding { get; set; }
        public string Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public byte[] RawBytes { get; set; }
        public Uri ResponseUri { get; set; }
        public string Server { get; set; }
        public IList<RestResponseCookie> Cookies { get; private set; }
        public IList<Parameter> Headers { get; private set; }
        public ResponseStatus ResponseStatus { get; set; }
        public string ErrorMessage { get; set; }
        public Exception ErrorException { get; set; }
    }

    public class PublicFeedMessage<T>
    {
        public string cmd { get; set; }

        public T args { get; set; }
    }

    public class PriceTick
    {
        public InstrumentDescriptor Instrument { get; set; }
        public string i { get; set; }
        public int m { get; set; }
        public string t { get; set; }
        public DateTime trade_timestamp { get; set; }
        public DateTime tick_timestamp { get; set; }
        public decimal bid { get; set; }
        public int bid_volume { get; set; }
        public decimal ask { get; set; }
        public int ask_volume { get; set; }
        public decimal close { get; set; }
        public decimal high { get; set; }
        public decimal last { get; set; }
        public int last_volume { get; set; }
        public int lot_size { get; set; }
        public decimal low { get; set; }
        public decimal open { get; set; }
        public int turnover { get; set; }
        public int turnover_volume { get; set; }
    }
}
