using Xunit;
using Payment.Application.Dto.Source;
using Payment.Application.Dto;
using Payment.Domain.Constants;
using FluentAssertions;
using Payment.Domain.Source;
using Payment.Application.Tests;

namespace Payment.Application.Mapper.Tests
{
    public class PaymentMapperTests
    {
        [Fact()]
        public void Map_ShouldReturn_PaymentDomain()
        {
            // Arrange
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
            var paymentDomain = PaymentMapper.Map(payment);

            // Assert
            AssertPropertiesAreSame(payment, paymentDomain!);
        }

        [Fact()]
        public void Map_ShouldReturn_Null_WhenDtoIsNull()
        {
            // Arrange

            // Act
            var paymentDomain = PaymentMapper.Map(default(PaymentDto));

            // Assert
            paymentDomain.Should().BeNull();
        }

        [Fact()]
        public void Map_ShouldReturn_Null_WhenDomainIsNull()
        {
            // Arrange

            // Act
            var paymentViewDto = PaymentMapper.Map(default(Domain.Payment));

            // Assert
            paymentViewDto.Should().BeNull();
        }

        [Fact()]
        public void Map_ShouldReturn_PaymentViewDto()
        {
            // Arrange
            var payment = Fixture.NewPayment(PaymentType.OneTime);

            // Act
            var paymentViewDto = PaymentMapper.Map(payment);

            // Assert
            AssertProperties(payment, paymentViewDto!);
        }

        private static void AssertProperties(Domain.Payment payment, PaymentViewDto paymentViewDto)
        {
            paymentViewDto.Id.Should().Be(payment.Id);
            paymentViewDto.Currency.Should().Be(payment.Currency);
            paymentViewDto.Status.Should().Be(payment.Status);
            paymentViewDto.Amount.Should().Be(payment.Amount);
            paymentViewDto.CreatedDateTime.Should().Be(payment.CreatedDateTime);
            paymentViewDto.LastChangedDateTime.Should().Be(payment.LastChangedDateTime);
            paymentViewDto.MerchantId.Should().Be(payment.MerchantId);
            paymentViewDto.Reference.Should().Be(payment.Reference);
            paymentViewDto.Response.Should().Be(payment.Response);
            var sourceCardNumber = ((CreditCardPaymentSource)payment.Source).Number;
            paymentViewDto.Source.MaskedCardNumber.Should()
                .Be(sourceCardNumber.MaskedLast('x', 4));
        }

        private static void AssertPropertiesAreSame(PaymentDto payment, Domain.Payment paymentDomain)
        {
            paymentDomain.Currency.Should().Be(payment.Currency);
            paymentDomain.Amount.Should().Be(payment.Amount);
            paymentDomain.MerchantId.Should().Be(payment.MerchantId);
            paymentDomain.Reference.Should().Be(payment.Reference);
            paymentDomain.Type.Should().Be(payment.Type);
            paymentDomain.Id.Should().NotBeEmpty();
            paymentDomain.Plan.Should().BeNull();
            paymentDomain.Source.Should().NotBeNull();
            paymentDomain.Source.Should().BeAssignableTo(typeof(CreditCardPaymentSource));
            var creditCardSource = paymentDomain.Source as CreditCardPaymentSource;
            creditCardSource!.Number.Should().Be(payment.Source.Number);
            creditCardSource.ExpiryYear.Should().Be(payment.Source.ExpiryYear);
            creditCardSource.ExpiryMonth.Should().Be(payment.Source.ExpiryMonth);
            creditCardSource.Name.Should().Be(payment.Source.Name);
        }
    }
}