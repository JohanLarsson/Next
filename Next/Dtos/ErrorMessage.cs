namespace Next.Dtos
{
    public class ErrorMessage
    {
        public string Error { get; set; }

        public override string ToString()
        {
            return string.Format("Error: {0}", Error);
        }
    }
}