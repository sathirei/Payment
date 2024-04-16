using FluentAssertions;
using Payment.Domain;
using Xunit;

namespace Payment.Application.BusinessRules.Tests
{
    public class CreditCardShouldNotBeExpiredTests
    {
        [Fact()]
        public void CreditCardShouldNotBeExpired_ShouldSetCodeAndMessage()
        {
            // Arrange

            // Act
            var sut = new CreditCardShouldNotBeExpired(2025, 01);

            // Assert
            sut.Code.Should().Be("credit-card-expired");
            sut.BrokenRuleMessage.Should()
                .Be("Credit card has already expired.");
        }

        [Fact()]
        public void IsValid_ShouldReturnTrue_WhenCreditCardIsNotExpired()
        {
            // Arrange
            var dateTime = new DateTime(2024, 04, 01);
            SystemTime.Set(dateTime);

            // Act
            var sut = new CreditCardShouldNotBeExpired(2025, 01);

            // Assert
            sut.IsValid().Should().BeTrue();
        }

        [Fact()]
        public void IsValid_ShouldReturnTrue_WhenCreditCardYearAndMonthAreSameAsCurrent()
        {
            // Arrange
            var dateTime = new DateTime(2024, 04, 01);
            SystemTime.Set(dateTime);

            // Act
            var sut = new CreditCardShouldNotBeExpired(2024, 04);

            // Assert
            sut.IsValid().Should().BeTrue();
        }

        [Fact()]
        public void IsValid_ShouldReturnFalse_WhenCreditIsExpired()
        {
            // Arrange
            var dateTime = new DateTime(2024, 04, 01);
            SystemTime.Set(dateTime);

            // Act
            var sut = new CreditCardShouldNotBeExpired(2023, 04);

            // Assert
            sut.IsValid().Should().BeFalse();
        }
    }
}