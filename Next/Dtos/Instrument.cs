namespace Next.Dtos
{
    public class Instrument
    {
        public string Type { get; set; }
        public string MainMarketPrice { get; set; }
        public string Identifier { get; set; }
        public string Currency { get; set; }
        public string MainMarketId { get; set; }

        public override string ToString()
        {
            return string.Format("Type: {0}, MainMarketPrice: {1}, Identifier: {2}, Currency: {3}, MainMarketId: {4}", Type, MainMarketPrice, Identifier, Currency, MainMarketId);
        }

        protected bool Equals(Instrument other)
        {
            return string.Equals(Type, other.Type) && string.Equals(MainMarketPrice, other.MainMarketPrice) && string.Equals(Identifier, other.Identifier) && string.Equals(Currency, other.Currency) && string.Equals(MainMarketId, other.MainMarketId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Instrument) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (MainMarketPrice != null ? MainMarketPrice.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Identifier != null ? Identifier.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Currency != null ? Currency.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (MainMarketId != null ? MainMarketId.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Instrument left, Instrument right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Instrument left, Instrument right)
        {
            return !Equals(left, right);
        }
    }
}