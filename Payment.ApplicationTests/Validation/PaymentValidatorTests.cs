using Xunit;
using Payment.Application.Dto;
using FluentAssertions;
using Payment.Application.Dto.Source;
using Payment.Domain;
using Payment.Domain.Constants;

namespace Payment.Application.Validation.Tests
{
    public class PaymentValidatorTests
    {
        [Fact()]
        public void Validate_ShouldReturnError_WhenSourceIsNull()
        {
            // Arrange
            var payment = new PaymentDto { Source = null };

            // Act
            var sut = new PaymentValidator();
            var result = sut.Validate(payment);
            var error = result.Errors
               .Single(x => x.PropertyName == nameof(payment.Source)
               && x.ErrorCode == "NotNullValidator");

            // Assert
            error.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            error.ErrorMessage.Should().Be("'Source' must not be empty.");
        }

        [Theory()]
        [InlineData(null)]
        [InlineData("")]
        public void Validate_ShouldReturnError_WhenMerchantId_IsNullOrEmpty(string merchantId)
        {
            // Arrange
            var payment = new PaymentDto { MerchantId = merchantId };

            // Act
            var sut = new PaymentValidator();
            var result = sut.Validate(payment);
            var error = result.Errors
               .Single(x => x.PropertyName == nameof(payment.MerchantId));

            // Assert
            error.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            error.ErrorMessage.Should().Be("'MerchantId' should not be null or empty.");
        }

        [Theory()]
        [InlineData(null)]
        [InlineData("")]
        public void Validate_ShouldReturnError_WhenReference_IsNullOrEmpty(string reference)
        {
            // Arrange
            var payment = new PaymentDto { Reference = reference };

            // Act
            var sut = new PaymentValidator();
            var result = sut.Validate(payment);

            var error = result.Errors
              .Single(x => x.PropertyName == nameof(payment.Reference));

            // Assert
            error.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            error.ErrorMessage.Should().Be("'Reference' should not be null or empty.");
        }

        [Theory()]
        [InlineData(null)]
        [InlineData("")]
        public void Validate_ShouldReturnError_WhenCurrency_IsNotValidISOCode(string currency)
        {
            // Arrange
            var payment = new PaymentDto { Currency = currency };

            // Act
            var sut = new PaymentValidator();
            var result = sut.Validate(payment);
            var error = result.Errors
               .Single(x => x.PropertyName == nameof(payment.Currency));

            // Assert
            error.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            error.ErrorMessage.Should().Be("Currency should be a valid ISO 4217 code.");
        }

        [Fact()]
        public void Validate_ShouldReturnError_WhenAmountIsNegative()
        {
            // Arrange
            var payment = new PaymentDto { Amount = -1 };

            // Act
            var sut = new PaymentValidator();
            var result = sut.Validate(payment);
            var error = result.Errors
               .Single(x => x.PropertyName == nameof(payment.Amount));

            // Assert
            error.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            error.ErrorMessage.Should().Be("Payment amount must be greater than 0.");
        }

        [Fact()]
        public void Validate_ShouldReturnError_WhenPaymentSource_IsExpired()
        {
            // Arrange
            var dateTime = new DateTime(2024, 04, 15);
            SystemTime.Set(dateTime);
            var payment = new PaymentDto
            {
                Source = new PaymentSourceDto
                {
                    Type = PaymentSourceType.CreditCard,
                    ExpiryMonth = 02,
                    ExpiryYear = 2023
                }
            };

            // Act
            var sut = new PaymentValidator();
            var result = sut.Validate(payment);
            var error = result.Errors
               .Single(x => x.PropertyName == nameof(payment.Source)
               && x.ErrorCode == "PredicateValidator");

            // Assert
            error.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            error.ErrorMessage.Should().Be("Credit card has already expired.");
        }

        [Fact()]
        public void Validate_ShouldReturnError_WhenPaymentType_IsNotSupported()
        {
            // Arrange
            var payment = new PaymentDto { Type = PaymentType.Recurring };

            // Act
            var sut = new PaymentValidator();
            var result = sut.Validate(payment);
            var error = result.Errors
               .Single(x => x.PropertyName == nameof(payment.Type)
               && x.ErrorCode == "PredicateValidator");

            // Assert
            error.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            error.ErrorMessage.Should().Be("Payment type is currently not supported.");
        }


        [Theory]
        [InlineData(0)]
        [InlineData(13)]
        [InlineData(-13)]
        public void Validate_ShouldReturnError_WhenExpiryMonthIsNotValid(int expiryMonth)
        {
            // Arrange
            var payment = new PaymentDto
            {
                Source = new PaymentSourceDto
                {
                    Type = PaymentSourceType.CreditCard,
                    ExpiryMonth = expiryMonth,
                    ExpiryYear = 2023
                }
            };

            // Act
            var sut = new PaymentValidator();
            var result = sut.Validate(payment);
            var error = result.Errors
               .Single(x => x.PropertyName == "Source.ExpiryMonth"
               && x.ErrorCode == "PredicateValidator");

            // Assert
            error.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            error.ErrorMessage.Should().Be("Expiry month should be between 1 and 12 (January to December).");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(13)]
        [InlineData(999)]
        [InlineData(10001)]
        [InlineData(00123)]
        public void Validate_ShouldReturnError_WhenExpiryIsNotFourDigit(int expiryYear)
        {
            // Arrange
            var payment = new PaymentDto
            {
                Source = new PaymentSourceDto
                {
                    Type = PaymentSourceType.CreditCard,
                    ExpiryMonth = 01,
                    ExpiryYear = expiryYear
                }
            };

            // Act
            var sut = new PaymentValidator();
            var result = sut.Validate(payment);
            var error = result.Errors
               .Single(x => x.PropertyName == "Source.ExpiryYear"
               && x.ErrorCode == "PredicateValidator");

            // Assert
            error.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            error.ErrorMessage.Should().Be("Year should be in 4 digit format e.g. 1999.");
        }

        [Fact()]
        public void Validate_Should_Succeed()
        {
            // Arrange
            var dateTime = new DateTime(2024, 04, 15);
            SystemTime.Set(dateTime);
            var payment = new PaymentDto
            {
                Source = new PaymentSourceDto
                {
                    Type = PaymentSourceType.CreditCard,
                    ExpiryMonth = 02,
                    ExpiryYear = 2025
                },
                Amount = 10000,
                MerchantId = "123",
                Currency = "GBP",
                Reference = "123_456",
                Type = PaymentType.OneTime
            };

            // Act
            var sut = new PaymentValidator();
            var result = sut.Validate(payment);

            // Assert
            result.Errors.Should().BeEmpty();
            result.IsValid.Should().BeTrue();
        }
    }
}