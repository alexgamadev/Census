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
        public void Meter_InputIsString_ReturnsOne()
        {
            var SimpleMeter = CreateDefaultSimpleMeter();
            int result = SimpleMeter.Meter( "Hello World" );

            Assert.True(result == 1, "Single meter correctly returns 1");
        }

        [Fact]
        public void Meter_InputIsNull_ReturnsException()
        {
            var SimpleMeter = CreateDefaultSimpleMeter();

            Assert.Throws<ArgumentNullException>(() => SimpleMeter.Meter(null));
        }

        [Fact]
        public void Meter_SameMeterMultipleTimes_ReturnsExpectedResult()
        {
            var SimpleMeter = CreateDefaultSimpleMeter();

            SimpleMeter.Meter("Test");
            SimpleMeter.Meter("Test");
            var result = SimpleMeter.Meter("Test");
            Assert.True(result == 3, "3 meters of same name correctly returns 3");
        }

        [Fact]
        public void Meter_DifferentMeters_Return1()
        {
            var SimpleMeter = CreateDefaultSimpleMeter();

            var result1 = SimpleMeter.Meter("Test");
            var result2 = SimpleMeter.Meter("Test2");
            Assert.True(result1 == 1 && result2 == 1, "Both meters should return 1");
        }

        [Fact]
        public void Meter_ValueOf2_Returns2()
        {
            var SimpleMeter = CreateDefaultSimpleMeter();

            var result = SimpleMeter.Meter("Test", 2);
            Assert.True(result == 2, "Meter should return 2");
        }

        [Fact]
        public void Meter_SameMeterValueOf2_Returns4()
        {
            var SimpleMeter = CreateDefaultSimpleMeter();

            SimpleMeter.Meter("Test", 2);
            var result = SimpleMeter.Meter("Test", 2);
            Assert.True(result == 4, "Meter should return 4");
        }
    }
}
