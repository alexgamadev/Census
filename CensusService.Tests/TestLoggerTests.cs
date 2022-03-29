using Xunit;

namespace CensusService.Tests
{
    public class TestLoggerTests
    {
        public TestLogger CreateDefaultTestLogger()
        {
            return new TestLogger();
        }

        [Theory]
        [InlineData("Hello World")]
        [InlineData("Test")]
        public void TestLogger_InputIsString_ReturnSameString(string value)
        {
            var testLogger = CreateDefaultTestLogger();
            string result = testLogger.SimpleLog(value);

            Assert.True(result == value, "Logger did not return exact string");
        }
    }
}