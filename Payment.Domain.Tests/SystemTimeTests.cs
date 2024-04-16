using FluentAssertions;

namespace Payment.Domain.Tests
{
    public class SystemTimeTests
    {
        [Fact()]
        public void Now_ShouldReturn_DateTimeThatHasBeenSet()
        {
            // Arrange
            var dateTime = new DateTime(2024, 4, 15);
            SystemTime.Set(dateTime);

            // Act
            var nowFirst = SystemTime.Now();
            var nowSecond = SystemTime.Now();

            // Asset
            nowFirst.Should().Be(nowSecond);
        }

        [Fact()]
        public void Now_ShouldReturn_ANewDate()
        {
            // Arrange
            SystemTime.Reset();

            // Act
            var nowFirst = SystemTime.Now();
            var nowSecond = SystemTime.Now();

            // Asset
            nowFirst.Should().NotBe(nowSecond);
        }

        [Fact()]
        public void OffsetNow_ShouldReturn_OffsetThatHasBeenSet()
        {
            // Arrange
            var dateTime = new DateTime(2024, 4, 15);
            SystemTime.Set(dateTime);

            // Act
            var offsetNowFirst = SystemTime.OffsetNow();
            var offsetNowSecond = SystemTime.OffsetNow();

            // Asset
            offsetNowFirst.Should().Be(offsetNowSecond);
        }

        [Fact()]
        public void OffsetNow_ShouldReturn_ANewOffset()
        {
            // Arrange
            SystemTime.Reset();

            // Act
            var offsetNowFirst = SystemTime.OffsetNow();
            var offsetNowSecond = SystemTime.OffsetNow();

            // Asset
            offsetNowFirst.Should().NotBe(offsetNowSecond);
        }
    }
}