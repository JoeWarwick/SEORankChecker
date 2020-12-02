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
    public class GoogleServiceTest
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

            var hdoc = new HtmlAgilityPack.HtmlDocument();
            hdoc.Load("TestAssets\\GoogleSearchResult.html");

            var googleServiceMock = new Mock<GoogleService>(It.IsAny<ILogger<GoogleService>>(), It.IsAny<ISEOClientService>(), memoryCache);
            googleServiceMock
                    .Setup(bs => bs.FetchResults("e-settlements"))
                    .ReturnsAsync(hdoc.Text);

            var expected = new List<SEOResult>
            {
                {
                    new SEOResult
                    {
                        Position = 2,
                        Title = "Sympli: Making e-Settlements Simple",
                        Url = "/url?q=https://www.sympli.com.au/&amp;sa=U&amp;ved=2ahUKEwjVue_Xl67tAhVHfMAKHRAPA9UQFjAMegQILhAB&amp;usg=AOvVaw2IhKVbTuIalIywK7K-FXvL",
                        Description="\r\n                    \r\n                        \r\n                            \r\n                                \r\n                                    \r\n                                        Meet Sympli - the new Electronic Settlements provider built by users for users. We're here to make e-Settlements simple. Find out more.\r\n                                        e-Settlement Services · About Us · Our Team · Contact Us\r\n                                    \r\n                                \r\n                            \r\n                        \r\n                    \r\n                "
                    }
                }                
            };

            // act
            var controller = new SEOSearchController(logger, googleServiceMock.Object, null);
            var actionResult = await controller.Get("Google", "e-settlements", "sympli.com.au");

            // assert
            Assert.IsNotNull(actionResult);
            var result = actionResult.Value;
            expected.Should().BeEquivalentTo(result);
        }
    }
}
