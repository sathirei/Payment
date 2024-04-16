using FluentAssertions;

namespace Payment.Domain.Source.Tests
{
    public class CreditCardPaymentSourceTests
    {
        [Fact()]
        public void CreditCardPaymentSource_Should_SetFields()
        {
            // Arrange
            var cardNumber = "1111222233334444";
            var expiryMonth = 11; // November
            var expiryYear = 2024;
            var name = "John Doe";

            // Act
            var sut = new CreditCardPaymentSource(
                cardNumber,
                expiryMonth,
                expiryYear,
                name);

            // Assert
            sut.Number.Should().Be(cardNumber);
            sut.ExpiryMonth.Should().Be(expiryMonth);
            sut.ExpiryYear.Should().Be(expiryYear);
            sut.Name.Should().Be(name);
        }
    }
}