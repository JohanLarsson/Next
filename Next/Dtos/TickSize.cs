namespace Next.Dtos
{
    public class TickSize
    {
        public double Tick { get; set; }
        public double Above { get; set; }
        public int Decimals { get; set; }

        public override string ToString()
        {
            return string.Format("Tick: {0}, Above: {1}, Decimals: {2}", Tick, Above, Decimals);
        }
    }
}