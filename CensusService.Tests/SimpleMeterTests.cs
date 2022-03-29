using System;
using Xunit;

namespace CensusService.Tests
{
    public class SimpleMeterTests
    {
        public SimpleMeter CreateDefaultSimpleMeter()
        {
            return new SimpleMeter();
        }

        [Fact]
        public void Meter_InputIsNull_ReturnsException()
        {
            var SimpleMeter = CreateDefaultSimpleMeter();

            Assert.Throws<ArgumentNullException>(() => SimpleMeter.Meter(null));
        }

        [Fact]
        public void Meter_SingleMeterNoValue_ReturnsOne()
        {
            var SimpleMeter = CreateDefaultSimpleMeter();
            int result = SimpleMeter.Meter( "Hello World" );

            Assert.True(result == 1, "Single meter correctly returns 1");
        }

        [Fact]
        public void Meter_SameMeterNoValue_ReturnsIncrementedValue()
        {
            var SimpleMeter = CreateDefaultSimpleMeter();

            SimpleMeter.Meter("Test");
            SimpleMeter.Meter("Test");
            var result = SimpleMeter.Meter("Test");
            Assert.True(result == 3, "3 meters of same name correctly returns 3");
        }

        [Fact]
        public void Meter_DifferentMeters_BothReturn1()
        {
            var SimpleMeter = CreateDefaultSimpleMeter();

            var result1 = SimpleMeter.Meter("Test");
            var result2 = SimpleMeter.Meter("Test2");
            Assert.True(result1 == 1 && result2 == 1, "Both meters should return 1");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        public void Meter_SingleMeterValue_ReturnsValueInput(int value)
        {
            var SimpleMeter = CreateDefaultSimpleMeter();

            var result = SimpleMeter.Meter("Test", value);
            Assert.True(result == value, "Meter should return metered value");
        }

        [Fact]
        public void Meter_SameMeterValue_ReturnsIncrementedValueInput()
        {
            var SimpleMeter = CreateDefaultSimpleMeter();

            SimpleMeter.Meter("Test", 2);
            var result = SimpleMeter.Meter("Test", 2);
            Assert.True(result == 4, "Meter should return 4");
        }
    }
}
