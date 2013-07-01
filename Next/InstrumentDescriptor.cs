using System;

namespace Next
{
    public class InstrumentDescriptor : IEquatable<InstrumentDescriptor>
    {
        public InstrumentDescriptor( int marketId,string identifier)
        {
            Identifier = identifier;
            MarketId = marketId;
        }

        public string Identifier { get; set; }

        public int MarketId {get; set; }

        public override string ToString()
        {
            return string.Format("MarketId: {1}, Identifier: {0}", Identifier, MarketId);
        }

        public bool Equals(InstrumentDescriptor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Identifier, other.Identifier) && MarketId == other.MarketId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((InstrumentDescriptor) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Identifier != null ? Identifier.GetHashCode() : 0)*397) ^ MarketId;
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