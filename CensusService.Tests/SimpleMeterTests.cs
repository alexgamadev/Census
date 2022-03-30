using Moq;
using System;
using System.IO;
using System.IO.Abstractions;
using Xunit;

namespace CensusService.Tests
{
    public class SimpleMeterTests
    {
        public SimpleMeter CreateDefaultSimpleMeter()
        {
            return new SimpleMeter();
        }

        #region Meter (IntegerValueTests)

        [Fact]
        public void Meter_InputIsNull_ReturnsException()
        {
            var simpleMeter = CreateDefaultSimpleMeter();

            Assert.Throws<ArgumentNullException>(() => simpleMeter.Meter(null));
        }

        [Fact]
        public void Meter_SingleMeterNoValue_ReturnsOne()
        {
            var simpleMeter = CreateDefaultSimpleMeter();
            int result = simpleMeter.Meter( "Hello World" );

            Assert.True(result == 1, "Single meter correctly returns 1");
        }

        [Fact]
        public void Meter_SameMeterNoValue_ReturnsIncrementedValue()
        {
            var simpleMeter = CreateDefaultSimpleMeter();

            simpleMeter.Meter("Test");
            simpleMeter.Meter("Test");
            var result = simpleMeter.Meter("Test");
            Assert.True(result == 3, "3 meters of same name correctly returns 3");
        }

        [Fact]
        public void Meter_DifferentMeters_BothReturn1()
        {
            var simpleMeter = CreateDefaultSimpleMeter();

            var result1 = simpleMeter.Meter("Test1");
            var result2 = simpleMeter.Meter("Test2");
            Assert.True(result1 == 1 && result2 == 1, "Both meters should return 1");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        public void Meter_SingleMeterValue_ReturnsValueInput(int value)
        {
            var simpleMeter = CreateDefaultSimpleMeter();

            var result = simpleMeter.Meter("Test", value);
            Assert.True(result == value, "Meter should return metered value");
        }

        [Fact]
        public void Meter_SameMeterValue_ReturnsCorrectValue()
        {
            var simpleMeter = CreateDefaultSimpleMeter();

            simpleMeter.Meter("Test", 2);
            var result = simpleMeter.Meter("Test", 2);
            Assert.True(result == 4, "Meter should return 4");
        }

        [Fact]
        public void Meter_DifferentMetersValue_ReturnsCorrectValue()
        {
            var simpleMeter = CreateDefaultSimpleMeter();

            simpleMeter.Meter("Test1", 1);
            var result1 = simpleMeter.Meter("Test1", 4);
            simpleMeter.Meter("Test2", 5);
            var result2 = simpleMeter.Meter("Test2", 10);
            Assert.True(result1 == 5 && result2 == 15, "Meters return expected values");
        }
        #endregion

        #region GetMeter (MemoryStorageTests)

        [Fact]
        public void GetMeter_InputIsNull_ReturnsException()
        {
            var simpleMeter = CreateDefaultSimpleMeter();

            Assert.Throws<ArgumentNullException>(() => simpleMeter.GetMeter(null));
        }

        [Fact]
        public void GetMeter_SingleMeter_Returns1()
        {
            var simpleMeter = CreateDefaultSimpleMeter();

            simpleMeter.Meter("Test");

            var result = simpleMeter.GetMeter("Test");

            Assert.True(result == 1, "GetMeter returns 1");
        }

        [Fact]
        public void GetMeter_SameNameMetered_Returns3()
        {
            var simpleMeter = CreateDefaultSimpleMeter();

            simpleMeter.Meter("Test");
            simpleMeter.Meter("Test");
            simpleMeter.Meter("Test");

            var result = simpleMeter.GetMeter("Test");

            Assert.True(result == 3, "GetMeter returns 3");
        }

        [Fact]
        public void GetMeter_InvalidMeterName_ReturnsNull()
        {
            var simpleMeter = CreateDefaultSimpleMeter();

            var result = simpleMeter.GetMeter("Test");

            Assert.Null(result);
        }

        [Fact]
        public void GetMeter_DifferentMeters_ReturnCorrectValues()
        {
            var simpleMeter = CreateDefaultSimpleMeter();

            simpleMeter.Meter("Test1", 3);
            simpleMeter.Meter("Test2", 12);

            var result1 = simpleMeter.GetMeter("Test1");
            var result2 = simpleMeter.GetMeter("Test2");

            Assert.True(result1 == 3 && result2 == 12, "GetMeter returns correct values for different meters");
        }

        #endregion

        #region SaveData (FileSystemTests)

        [Theory]
        [InlineData("Test1")]
        [InlineData("Test2")]
        public void SaveData_SingleMeterNoValue_StoresCorrectValue(string value)
        {
            var path = "../Meters.txt";

            var mockFileIO = new Mock<IFileSystem>();
            mockFileIO.Setup(t => t.File.Exists(path)).Returns(false);
            var simpleMeter = new SimpleMeter(mockFileIO.Object);

            simpleMeter.Meter(value);
            simpleMeter.SaveData(path);

            mockFileIO.Verify(f => f.File.WriteAllText(path, $"{value}:1;"), Times.Once);
        }

        [Fact]
        public void SaveData_DifferentMetersNoValue_StoresCorrectValues()
        {
            var path = "../Meters.txt";

            var mockFileIO = new Mock<IFileSystem>();
            mockFileIO.Setup(t => t.File.Exists(path)).Returns(false);
            var simpleMeter = new SimpleMeter(mockFileIO.Object);

            simpleMeter.Meter("Test1");
            simpleMeter.Meter("Test2");
            simpleMeter.SaveData(path);

            mockFileIO.Verify(f => f.File.WriteAllText(path, "Test1:1;Test2:1;"), Times.Once);
        }

        [Fact]
        public void SaveData_SingleMeterDefaultClear_ClearsLocalData()
        {
            var path = "../Meters.txt";

            var mockFileIO = new Mock<IFileSystem>();
            mockFileIO.Setup(t => t.File.Exists(path)).Returns(false);
            var simpleMeter = new SimpleMeter(mockFileIO.Object);

            simpleMeter.Meter("Test");
            simpleMeter.SaveData(path);
            var result = simpleMeter.GetMeter("Test");

            mockFileIO.Verify(f => f.File.WriteAllText(path, "Test:1;"), Times.Once);
            Assert.Null(result);
        }

        [Fact]
        public void SaveData_SingleMeterNoLocalClear_LocalDataRemains()
        {
            var path = "../Meters.txt";

            var mockFileIO = new Mock<IFileSystem>();
            mockFileIO.Setup(t => t.File.Exists(path)).Returns(false);
            var simpleMeter = new SimpleMeter(mockFileIO.Object);

            simpleMeter.Meter("Test");
            simpleMeter.SaveData(path, false);
            var result = simpleMeter.GetMeter("Test");

            mockFileIO.Verify(f => f.File.WriteAllText(path, "Test:1;"), Times.Once);
            Assert.True(result == 1, "Local data isn't cleared");
        }

        #endregion
    }
}
