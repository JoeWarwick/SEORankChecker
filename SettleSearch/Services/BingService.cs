using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace SettleSearch.Services
{
    public class BingService : ISearchService
    {
        private readonly ILogger<BingService> logger;
        private readonly ISEOClientService client;
        private readonly IMemoryCache cache;

        private const string searchAddress = "https://bing.com/search?";
        private const int maxResults = 100;
        private const int cacheTimeoutMins = 60;

        public BingService(ILogger<BingService> logger, ISEOClientService client, IMemoryCache cache)
        {
            this.logger = logger;
            this.client = client;
            this.cache = cache;
        }

        public virtual async Task<string> FetchResults(string query)
        {
            var parms = new Dictionary<string, string> { { "q", query }, { "count", maxResults.ToString() }, { "format", "rss" } };
            if (cache.TryGetValue($"Bing:{query}", out string response))
                logger.LogInformation("BingService: Fetching Cached Data");
            else
                response = await cache.GetOrCreateAsync($"Bing:{query}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTimeoutMins);
                    logger.LogInformation("BingService: Fetching Live Data");
                    return await this.client.Fetch(new Uri(searchAddress), parms);
                });

            return response;
        }

        public List<SEOResult> ExtractSEOResults(string html, string query, string siteDomain)
        {
            var res = new List<SEOResult>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(html);
            var selectNodes = doc.DocumentElement.SelectNodes("//item");
            
            for (var i=0; i<selectNodes.Count; i++)
            {
                var node = selectNodes[i];
                res.Add(new SEOResult
                {
                    Position = i + 1,
                    Title = node.SelectSingleNode("./title").InnerText,
                    Url = node.SelectSingleNode("./link").InnerText,
                    Description = node.SelectSingleNode("./description").InnerText
                });
            }
            return res.FindAll(sr => sr.Title.Contains(siteDomain) || sr.Url.Contains(siteDomain) || sr.Description.Contains(siteDomain));
        }

    }
}
