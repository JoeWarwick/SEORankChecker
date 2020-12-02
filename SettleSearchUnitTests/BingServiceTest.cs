using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FluentAssertions;
using SettleSearch.Services;
using System.Xml;
using System.Threading.Tasks;
using SettleSearch.Controllers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace SettleSearchUnitTests
{
    [TestClass]
    public class BingServiceTest
    {
        [TestMethod]
        public async Task TestExtract()
        {
            // arrange
            var services = new ServiceCollection();
            services.AddMemoryCache();
            services.AddLogging();
            var serviceProvider = services.BuildServiceProvider();

            var memoryCache = serviceProvider.GetService<IMemoryCache>();
            var logger = serviceProvider.GetService<ILogger<SEOSearchController>>();

            var xdoc = new XmlDocument();
            xdoc.Load("TestAssets\\BingSearchResult.xml");

            var bingServiceMock = new Mock<BingService>(It.IsAny<ILogger<BingService>>(), It.IsAny<ISEOClientService>(), memoryCache);
            bingServiceMock
                    .Setup(bs => bs.FetchResults("e-settlements"))
                    .ReturnsAsync(xdoc.OuterXml);

            var expected = new List<SEOResult>
            {
                {
                    new SEOResult
                    {
                        Position = 11,
                        Title = "Making e-Settlements Simple | Sympli",
                        Url = "https://www.sympli.com.au/",
                        Description="An e-settlements service built by users, for users. We’ve spoken with thousands of lawyers and conveyancers from across the industry to identify what you want to see in an e-settlements service. The result is an efficient and user-friendly platform shaped by your feedback."
                    }
                },
                {
                    new SEOResult
                    {
                        Position = 12,
                        Title = "e-Settlement Services | Sympli - Making e-Settlements Simple",
                        Url = "https://www.sympli.com.au/e-settlement-services/",
                        Description="e-Settlement Services Designed by users, for users. You’ve told us what you need to conduct e-Settlements with confidence and our highly experienced team have taken your feedback to develop a service that’s efficient, easy to use, reliable and secure."
                    }
                }
                
            };

            // act
            var controller = new SEOSearchController(logger, null, bingServiceMock.Object);
            var actionResult = await controller.Get("Bing", "e-settlements", "sympli.com.au");

            // assert
            Assert.IsNotNull(actionResult);
            var result = actionResult.Value;
            expected.Should().BeEquivalentTo(result);
        }
    }
}
