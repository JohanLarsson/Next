using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Next.Dtos
{
    public class InstrumentDescriptor : IEquatable<InstrumentDescriptor>
    {
        public InstrumentDescriptor()
        {
            
        }
        public InstrumentDescriptor(string marketId,string identifier)
        {
            Identifier = identifier;
            MarketId = marketId;
        }

        public InstrumentDescriptor(int marketId, string identifier)
        {
            Identifier = identifier;
            MarketId = marketId.ToString(CultureInfo.InvariantCulture);
        }

        public string Identifier { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Never)]
        public string MarketID
        {
            get { return MarketId; }
            set { MarketId = value; }
        }

        public string MarketId {get; set; }

        public override string ToString()
        {
            return string.Format("MarketId: {1}, Identifier: {0}", Identifier, MarketId);
        }

        public bool Equals(InstrumentDescriptor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Identifier, other.Identifier) && string.Equals(MarketId, other.MarketId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((InstrumentDescriptor) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Identifier != null ? Identifier.GetHashCode() : 0)*397) ^ (MarketId != null ? MarketId.GetHashCode() : 0);
            }
        }

        public static bool operator ==(InstrumentDescriptor left, InstrumentDescriptor right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(InstrumentDescriptor left, InstrumentDescriptor right)
        {
            return !Equals(left, right);
        }
    }
}