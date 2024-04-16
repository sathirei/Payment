namespace Payment.Infrastructure
{
    public class EncryptionKeyNotSetException : Exception
    {
        public EncryptionKeyNotSetException()
        {
        }

        public EncryptionKeyNotSetException(string message)
            : base(message)
        {
        }

        public EncryptionKeyNotSetException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
