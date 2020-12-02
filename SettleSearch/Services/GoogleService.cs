using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SettleSearch.Services
{
    public class GoogleService : ISearchService
    {
        private readonly ILogger<GoogleService> logger;
        private readonly ISEOClientService client;
        private readonly IMemoryCache cache;

        private const string searchAddress = "https://google.com/search?";
        private const int maxResults = 100;
        private const int cacheTimeoutMins = 60;

        public GoogleService(ILogger<GoogleService> logger, ISEOClientService client, IMemoryCache cache)
        {
            this.logger = logger;
            this.client = client;
            this.cache = cache;
        }

        public virtual async Task<string> FetchResults(string query)
        {
            var parms = new Dictionary<string, string> { { "q", query }, { "num", maxResults.ToString() } };
            if (cache.TryGetValue($"Google:{query}", out string response))
                logger.LogInformation("GoogleService: Fetching Cached Data");
            else
                response = await cache.GetOrCreateAsync($"Google:{query}", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTimeoutMins);
                    logger.LogInformation("GoogleService: Fetching Live Data");
                    return await this.client.Fetch(new Uri(searchAddress), parms);
                });

            return response;
        }

        public List<SEOResult> ExtractSEOResults(string html, string query, string siteDomain)
        {
            var res = new List<SEOResult>();
            // As parsing the HTML with regex is convoluted I will be using HtmlAgilityPack as confirmed by Wayne.

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var selectNodes = doc.DocumentNode.SelectNodes("//div/a/h3");
            
            for (var i=0; i<selectNodes.Count; i++)
            {
                var node = selectNodes[i];
                res.Add(new SEOResult
                {
                    Position = i + 1,
                    Title = string.Join(" | ", node.ChildNodes.Select(c => c.InnerText)),
                    Url = node.ParentNode.GetAttributeValue<string>("href", "#notfound"),
                    Description = node.ParentNode.ParentNode.SelectSingleNode("following-sibling::div[2]")?.InnerText
                });
            }
            return res.FindAll(sr => sr.Title.Contains(siteDomain) || sr.Url.Contains(siteDomain) || sr.Description.Contains(siteDomain));
        }

    }
}
