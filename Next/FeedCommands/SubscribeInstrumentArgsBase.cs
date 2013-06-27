using System;

namespace Next.FeedCommands
{
    public class SubscribeInstrumentArgsBase : IEquatable<SubscribeInstrumentArgsBase>
    {
        public SubscribeInstrumentArgsBase(InstrumentDescriptor instrument, string type)
        {
            m = instrument.MarketId;
            i = instrument.Identifier;
            t = type;
        }
        public string t { get; private set; }
        public string i { get; set; }
        public int m { get; set; }

        public bool Equals(SubscribeInstrumentArgsBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(t, other.t) && string.Equals(i, other.i) && m == other.m;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SubscribeInstrumentArgsBase) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (t != null ? t.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (i != null ? i.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ m;
                return hashCode;
            }
        }

        public static bool operator ==(SubscribeInstrumentArgsBase left, SubscribeInstrumentArgsBase right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SubscribeInstrumentArgsBase left, SubscribeInstrumentArgsBase right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("Type(t): {0}, Identification(i): {1}, Market(m): {2}", t, i, m);
        }
    }
}