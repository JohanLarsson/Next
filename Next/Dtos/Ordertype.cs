namespace Next.Dtos
{
    public class Ordertype
    {
        public string Text { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return string.Format("Text: {0}, Type: {1}", Text, Type);
        }
    }
}