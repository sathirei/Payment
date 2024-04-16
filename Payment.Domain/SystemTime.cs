namespace Payment.Domain
{
    public static class SystemTime
    {
        [ThreadStatic]
        private static DateTime? _customDateTime;

        public static DateTime Now()
        {
            return _customDateTime ?? DateTime.UtcNow;
        }

        public static DateTimeOffset OffsetNow()
        {
            return ((DateTimeOffset?)_customDateTime) ?? DateTimeOffset.UtcNow;
        }

        public static void Set(DateTime customDateTime)
        {
            _customDateTime = customDateTime;
        }

        public static void Reset()
        {
            _customDateTime = null;
        }
    }
}
