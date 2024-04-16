namespace Payment.Application
{
    public static class StringExtensions
    {
        public static string MaskedLast(this string source, char maskValue, int count)
        {
            if (count > source?.Length)
            {
                return source;
            }
            var masked = new string(maskValue, source!.Length - count);
            var nonMasked = source.Substring(source.Length - count, count);
            return  masked + nonMasked;
        }
    }
}
