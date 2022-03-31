using Moq;
using System;
using System.IO;
using System.IO.Abstractions;
using Xunit;

namespace Census.Metering.Tests
{
    public class CensusMeterTests
    {
        public CensusMeter CreateDefaultCensusMeter()
        {
            return new CensusMeter();
        }

        #region Meter (IntegerValueTests)

        [Fact]
        public void Meter_InputIsNull_ReturnsException()
        {
            var censusMeter = CreateDefaultCensusMeter();

            Assert.Throws<ArgumentNullException>(() => censusMeter.Meter(null));
        }

        [Fact]
        public void Meter_SingleMeterNoValue_ReturnsOne()
        {
            var censusMeter = CreateDefaultCensusMeter();
            int result = censusMeter.Meter( "Hello World" );

            Assert.True(result == 1, "Single meter correctly returns 1");
        }

        [Fact]
        public void Meter_SameMeterNoValue_ReturnsIncrementedValue()
        {
            var censusMeter = CreateDefaultCensusMeter();

            censusMeter.Meter("Test");
            censusMeter.Meter("Test");
            var result = censusMeter.Meter("Test");
            Assert.True(result == 3, "3 meters of same name correctly returns 3");
        }

        [Fact]
        public void Meter_DifferentMeters_BothReturn1()
        {
            var censusMeter = CreateDefaultCensusMeter();

            var result1 = censusMeter.Meter("Test1");
            var result2 = censusMeter.Meter("Test2");
            Assert.True(result1 == 1 && result2 == 1, "Both meters should return 1");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        public void Meter_SingleMeterValue_ReturnsValueInput(int value)
        {
            var censusMeter = CreateDefaultCensusMeter();

            var result = censusMeter.Meter("Test", value);
            Assert.True(result == value, "Meter should return metered value");
        }

        [Fact]
        public void Meter_SameMeterValue_ReturnsCorrectValue()
        {
            var censusMeter = CreateDefaultCensusMeter();

            censusMeter.Meter("Test", 2);
            var result = censusMeter.Meter("Test", 2);
            Assert.True(result == 4, "Meter should return 4");
        }

        [Fact]
        public void Meter_DifferentMetersValue_ReturnsCorrectValue()
        {
            var censusMeter = CreateDefaultCensusMeter();

            censusMeter.Meter("Test1", 1);
            var result1 = censusMeter.Meter("Test1", 4);
            censusMeter.Meter("Test2", 5);
            var result2 = censusMeter.Meter("Test2", 10);
            Assert.True(result1 == 5 && result2 == 15, "Meters return expected values");
        }
        #endregion

        #region GetMeter (MemoryStorageTests)

        [Fact]
        public void GetMeter_InputIsNull_ReturnsException()
        {
            var censusMeter = CreateDefaultCensusMeter();

            Assert.Throws<ArgumentNullException>(() => censusMeter.GetMeter(null));
        }

        [Fact]
        public void GetMeter_SingleMeter_Returns1()
        {
            var censusMeter = CreateDefaultCensusMeter();

            censusMeter.Meter("Test");

            var result = censusMeter.GetMeter("Test");

            Assert.True(result == 1, "GetMeter returns 1");
        }

        [Fact]
        public void GetMeter_SameNameMetered_Returns3()
        {
            var censusMeter = CreateDefaultCensusMeter();

            censusMeter.Meter("Test");
            censusMeter.Meter("Test");
            censusMeter.Meter("Test");

            var result = censusMeter.GetMeter("Test");

            Assert.True(result == 3, "GetMeter returns 3");
        }

        [Fact]
        public void GetMeter_InvalidMeterName_ReturnsNull()
        {
            var censusMeter = CreateDefaultCensusMeter();

            var result = censusMeter.GetMeter("Test");

            Assert.Null(result);
        }

        [Fact]
        public void GetMeter_DifferentMeters_ReturnCorrectValues()
        {
            var censusMeter = CreateDefaultCensusMeter();

            censusMeter.Meter("Test1", 3);
            censusMeter.Meter("Test2", 12);

            var result1 = censusMeter.GetMeter("Test1");
            var result2 = censusMeter.GetMeter("Test2");

            Assert.True(result1 == 3 && result2 == 12, "GetMeter returns correct values for different meters");
        }

        #endregion

        #region SaveData (JsonTests)

        [Theory]
        [InlineData("Test1")]
        [InlineData("Test2")]
        public void SaveData_SingleMeterNoValue_StoresCorrectValue(string value)
        {
            var path = "../Meters.txt";

            var mockFileIO = new Mock<IFileSystem>();
            mockFileIO.Setup(t => t.File.Exists(path)).Returns(false);
            var censusMeter = new CensusMeter(mockFileIO.Object);

            censusMeter.Meter(value);
            censusMeter.SaveData(path);

            mockFileIO.Verify(f => f.File.WriteAllText(path, $"{{\"{value}\":1}}"), Times.Once);
        }

        [Fact]
        public void SaveData_DifferentMetersNoValue_StoresCorrectValues()
        {
            var path = "../Meters.json";

            var mockFileIO = new Mock<IFileSystem>();
            mockFileIO.Setup(t => t.File.Exists(path)).Returns(false);
            var censusMeter = new CensusMeter(mockFileIO.Object);

            censusMeter.Meter("Test1");
            censusMeter.Meter("Test2");
            censusMeter.SaveData(path);

            mockFileIO.Verify(f => f.File.WriteAllText(path, "{\"Test1\":1,\"Test2\":1}"), Times.Once);
        }

        [Fact]
        public void SaveData_SingleMeterDefaultClear_ClearsLocalData()
        {
            var path = "../Meters.json";

            var mockFileIO = new Mock<IFileSystem>();
            mockFileIO.Setup(t => t.File.Exists(path)).Returns(false);
            var censusMeter = new CensusMeter(mockFileIO.Object);

            censusMeter.Meter("Test");
            censusMeter.SaveData(path, true);
            var result = censusMeter.GetMeter("Test");

            mockFileIO.Verify(f => f.File.WriteAllText(path, "{\"Test\":1}"), Times.Once);
            Assert.Null(result);
        }

        [Fact]
        public void SaveData_SingleMeterNoLocalClear_LocalDataRemains()
        {
            var path = "../Meters.json";

            var mockFileIO = new Mock<IFileSystem>();
            mockFileIO.Setup(t => t.File.Exists(path)).Returns(false);
            var censusMeter = new CensusMeter(mockFileIO.Object);

            censusMeter.Meter("Test");
            censusMeter.SaveData(path);
            var result = censusMeter.GetMeter("Test");

            mockFileIO.Verify(f => f.File.WriteAllText(path, "{\"Test\":1}"), Times.Once);
            Assert.True(result == 1, "Local data isn't cleared");
        }

        [Fact]
        public void SaveData_SameMeterMultipleTimes_JsonValueUpdated()
        {
            var path = "Meters.json";

            var mockFileIO = new Mock<IFileSystem>();
            mockFileIO.Setup(t => t.File.WriteAllText(path, "{\"Test\":1}"));
            var censusMeter = new CensusMeter(mockFileIO.Object);

            censusMeter.Meter("Test", 4);
            censusMeter.SaveData(path);

            mockFileIO.Verify(f => f.File.WriteAllText(path, "{\"Test\":5}"), Times.Once);
        }

        #endregion
    }
}
