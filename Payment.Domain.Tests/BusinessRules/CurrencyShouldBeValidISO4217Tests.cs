using FluentAssertions;
using ISO._4217;
using Payment.Domain.BusinessRules;

namespace Payment.Domain.Tests.BusinessRules
{
    public class CurrencyShouldBeValidISO4217Tests
    {
        [Fact()]
        public void CurrencyShouldBeValidISO4217_ShouldSetCodeAndMessage()
        {
            // Arrange

            // Act
            var sut = new CurrencyShouldBeValidISO4217("");

            // Assert
            sut.Code.Should().Be("currency-should-be-valid-iso-4217");
            sut.BrokenRuleMessage.Should()
                .Be("Currency should be a valid ISO 4217 code.");
        }

        [Fact()]
        public void IsValid_ShouldReturnTrue_WhenCurrencyISOIsValid()
        {
            // Arrange
            var currencies = CurrencyCodesResolver.Codes
                .Where(currency => !string.IsNullOrWhiteSpace(currency.Code));

            // Act
            foreach (var isoCurrencyCode in currencies)
            {
                var sut = new CurrencyShouldBeValidISO4217(isoCurrencyCode.Code);

                // Assert
                sut.IsValid().Should().BeTrue();
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" \t")]
        [InlineData("INVALID")]
        public void IsValid_ShouldReturnFalse_WhenCurrencyISOIsValid(string isoCurrencyCode)
        {
            // Arrange

            // Act
            var sut = new CurrencyShouldBeValidISO4217(isoCurrencyCode);

            // Assert
            sut.IsValid().Should().BeFalse();
        }
    }
}
