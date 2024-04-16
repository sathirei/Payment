using Payment.ApiTests.Helper;
using Payment.Application.Dto.Source;
using Payment.Application.Dto;
using Payment.Domain.Constants;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;
using Newtonsoft.Json;

namespace Payment.ApiTests.Integration
{
    public class PaymentIntegrationTests : IClassFixture<PaymentApplicationFactory<Program>>
    {
        private readonly PaymentApplicationFactory<Program> _factory;
        private readonly HttpClient _httpClient;

        public PaymentIntegrationTests(PaymentApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
        }

        [Fact]
        public async Task POST_ShouldReturn_202_WithPaymentResult()
        {
            // Arrange
            var payment = new PaymentDto
            {
                Source = new PaymentSourceDto
                {
                    Type = PaymentSourceType.CreditCard,
                    ExpiryMonth = 02,
                    ExpiryYear = 2025,
                    Name = "John Doe",
                    Number = "1111222233334444"
                },
                Amount = 10000,
                MerchantId = "123",
                Currency = "GBP",
                Reference = "123_456",
                Type = PaymentType.OneTime
            };

            // Act
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("IdempotencyKey", Guid.NewGuid().ToString());
            var result = await _httpClient.PostAsJsonAsync("v1.0/Payments", payment);
            result.EnsureSuccessStatusCode();

            // Assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Accepted);
            var paymentResult = JsonConvert.DeserializeObject<PaymentResultDto>(
                await result.Content.ReadAsStringAsync());
            paymentResult!.Id.Should().NotBeEmpty();
            paymentResult!.Status.Should().Be(PaymentStatus.EXECUTING);
        }

        [Fact]
        public async Task GET_ShouldReturn_CreatedPayment()
        {
            // Arrange
            var payment = new PaymentDto
            {
                Source = new PaymentSourceDto
                {
                    Type = PaymentSourceType.CreditCard,
                    ExpiryMonth = 02,
                    ExpiryYear = 2025,
                    Name = "John Doe",
                    Number = "1111222233334444"
                },
                Amount = 10000,
                MerchantId = "123",
                Currency = "GBP",
                Reference = "123_456",
                Type = PaymentType.OneTime
            };

            // Act
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("IdempotencyKey", Guid.NewGuid().ToString());
            var result = await _httpClient.PostAsJsonAsync("v1.0/Payments", payment);
            result.EnsureSuccessStatusCode();
            var paymentResult = JsonConvert.DeserializeObject<PaymentResultDto>(
                await result.Content.ReadAsStringAsync());
            var paymentViewResult = await _httpClient.GetAsync($"v1.0/Payments/{paymentResult.Id}");
            var paymentView = JsonConvert.DeserializeObject<PaymentViewDto>(
                await paymentViewResult.Content.ReadAsStringAsync());

            // Assert  
            paymentView!.Id.Should().Be(paymentResult!.Id);
        }

        [Fact]
        public async Task POST_Twice_WithSameIdempotencyKey_ShouldReturn_SamePaymentResult()
        {
            // Arrange
            var payment = new PaymentDto
            {
                Source = new PaymentSourceDto
                {
                    Type = PaymentSourceType.CreditCard,
                    ExpiryMonth = 02,
                    ExpiryYear = 2025,
                    Name = "John Doe",
                    Number = "1111222233334444"
                },
                Amount = 10000,
                MerchantId = "123",
                Currency = "GBP",
                Reference = "123_456",
                Type = PaymentType.OneTime
            };
            var idempotencyKey = Guid.NewGuid().ToString();

            // Act
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("IdempotencyKey", idempotencyKey);
            var resultOne = await _httpClient.PostAsJsonAsync("v1.0/Payments", payment);
            var contentOne = await resultOne.Content.ReadAsStringAsync();
            var paymentResultOne = JsonConvert.DeserializeObject<PaymentResultDto>(contentOne);
            resultOne.EnsureSuccessStatusCode();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("IdempotencyKey", idempotencyKey);
            var resultTwo = await _httpClient.PostAsJsonAsync("v1.0/Payments", payment);
            var contentTwo = await resultTwo.Content.ReadAsStringAsync();
            var paymentResultTwo = JsonConvert.DeserializeObject<PaymentResultDto>(contentTwo);
            resultTwo.EnsureSuccessStatusCode();

            // Assert
            paymentResultOne.Should().BeEquivalentTo(paymentResultTwo);
        }
    }
}
