using Moq;
using SnmpSharpNet;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Scorpio.Instrumentation.Ubiquiti.Tests
{
    public class UbiquitiSnmpTests
    {
        [Fact]
        public async Task UbiquitiResponseFilteringTest()
        {
            // Arrange
            var sntpServiceMock = new Mock<ISnmpService>();
            Dictionary<Oid, AsnType> mockResponse = new Dictionary<Oid, AsnType>
            {
                { new Oid("1.3.6.1.4.1.41112.1.4.1.1.6.1"), new Integer32(161)  },
                { new Oid("1.3.6.1.4.1.41112.1.4.1.1.4.1"), new Integer32(141)  },
                { new Oid("1.3.6.1.4.1.41112.1.4.1.1.4.22"), new Integer32(99322)  },
                { new Oid("1.3.6.1.4.1.41112.1.4.1.1.4.24"), new Integer32(41241)  }
            };

            var ubi = new UbiquitiStatsProvider(sntpServiceMock.Object);
            var filterDict = new Dictionary<string, PhysicalProperty>
            {
                { "1.3.6.1.4.1.41112.1.4.1.1.6.1", new PhysicalProperty { Magnitude = "signalPower", Unit = "dBm" } },
                { "1.3.6.1.4.1.41112.1.4.1.1.4.1", new PhysicalProperty { Magnitude = "frequency", Unit = "MHz" } },
            };

            ubi.ResponseFilter = filterDict;
            sntpServiceMock.Setup(x => x.Walk(It.IsAny<SnmpVersion>(), It.IsAny<string>())).Returns(mockResponse);


            // Act
            var stats = await ubi.GetStatsAsync();


            // Assert
            Assert.Equal(2, stats.Count);
            Assert.True(stats.First().Key.Equals("signalPower"));
            Assert.Contains("161 [dBm]", stats.First().Value);
        }
    }
}
