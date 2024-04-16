using Crypto.AES;

namespace Payment.Infrastructure
{
    public static class Encryption
    {
        private static string Key { get; set; }
        public static string Encrypt(string dataToEncrypt)
        {
            ThrowIfKeyIsNull();

            if (string.IsNullOrEmpty(dataToEncrypt) || string.IsNullOrWhiteSpace(dataToEncrypt))
            {
                return dataToEncrypt;
            }

            using AES aes = new(Key);
            return aes.Encrypt(dataToEncrypt);
        }

        public static string Decrypt(string dataToDecrypt)
        {
            ThrowIfKeyIsNull();

            if (string.IsNullOrEmpty(dataToDecrypt) || string.IsNullOrWhiteSpace(dataToDecrypt))
            {
                return dataToDecrypt;
            }
            using AES aes = new(Key);
            return aes.Decrypt(dataToDecrypt);
        }

        private static void ThrowIfKeyIsNull()
        {
            if (string.IsNullOrEmpty(Key))
            {
                throw new EncryptionKeyNotSetException("Encryption key has not been set.");
            }
        }

        public static void SetEncryptionKey(string key)
        {
            Key = key;
        }
    }
}
