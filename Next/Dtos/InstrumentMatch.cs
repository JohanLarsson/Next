namespace Next.Dtos
{
    public class InstrumentMatch : Instrument
    {
        public string Longname { get; set; }
        public int MarketID { get; set; }
        public string Country { get; set; }
        public string Shortname { get; set; }
        public string Marketname { get; set; }
        public string IsinCode { get; set; }

        public override string ToString()
        {
            return string.Format("Type: {0}, Longname: {1}, marketID: {2}, Country: {3}, Shortname: {4}, Marketname: {5}, IsinCode: {6}, Identifier: {7}, Currency: {8}", Type, Longname, MarketID, Country, Shortname, Marketname, IsinCode, Identifier, Currency);
        }

        protected bool Equals(InstrumentMatch other)
        {
            return base.Equals(other) && string.Equals(Longname, other.Longname) && string.Equals(Country, other.Country) && string.Equals(Shortname, other.Shortname) && string.Equals(Marketname, other.Marketname) && string.Equals(IsinCode, other.IsinCode);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((InstrumentMatch) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (Longname != null ? Longname.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Country != null ? Country.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Shortname != null ? Shortname.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Marketname != null ? Marketname.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (IsinCode != null ? IsinCode.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(InstrumentMatch left, InstrumentMatch right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(InstrumentMatch left, InstrumentMatch right)
        {
            return !Equals(left, right);
        }
    }
}