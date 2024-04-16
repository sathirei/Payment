using FluentAssertions;

namespace Payment.Infrastructure.Tests
{
    public class EncryptionTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Decrypt_ShouldThrow_EncryptionKeyIsNullOrEmpty(string key)
        {
            // Arrange
            Encryption.SetEncryptionKey(key);

            // Act
            Action action = () => Encryption.Decrypt("Test");

            // Assert
            action.Should().Throw<EncryptionKeyNotSetException>()
                .WithMessage("Encryption key has not been set.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Encrypt_ShouldThrow_EncryptionKeyNotSetException(String key)
        {
            // Arrange
            Encryption.SetEncryptionKey(key);

            // Act
            Action action = () => Encryption.Encrypt("Test");

            // Assert
            action.Should().Throw<EncryptionKeyNotSetException>()
                .WithMessage("Encryption key has not been set.");
        }

        [Fact()]
        public void Decrypt_ShouldReturn_OriginalValue()
        {
            // Arrange
            var encryptionKey = "123";
            Encryption.SetEncryptionKey(encryptionKey);

            var sensitiveData = "1111222233334444";

            // Act
            var encryptedValue = Encryption.Encrypt(sensitiveData);
            var decryptedValue = Encryption.Decrypt(encryptedValue);

            // Assert
            decryptedValue.Should().Be(sensitiveData);
        }

        [Fact()]
        public void Encrypt_ShouldReturnEncryptedValue_DifferentFromOriginal()
        {
            // Arrange
            var encryptionKey = "12345678901234567890123456789012";
            Encryption.SetEncryptionKey(encryptionKey);

            var sensitiveData = "1111222233334444";

            // Act
            var encryptedValue = Encryption.Encrypt(sensitiveData);

            // Assert
            encryptedValue.Should().NotBe(sensitiveData);

        }

        [Theory()]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public void Encrypt_ShouldReturnEncryptedValue_SameAsOriginal(string input)
        {
            // Arrange
            var encryptionKey = "12345678901234567890123456789012";
            Encryption.SetEncryptionKey(encryptionKey);

            // Act
            var encryptedValue = Encryption.Encrypt(input);

            // Assert
            encryptedValue.Should().Be(input);
        }

        [Theory()]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public void Decrypt_ShouldReturnEncryptedValue_SameAsOriginal(string input)
        {
            // Arrange
            var encryptionKey = "12345678901234567890123456789012";
            Encryption.SetEncryptionKey(encryptionKey);

            // Act
            var decryptedValue = Encryption.Decrypt(input);

            // Assert
            decryptedValue.Should().Be(input);

        }
    }
}
