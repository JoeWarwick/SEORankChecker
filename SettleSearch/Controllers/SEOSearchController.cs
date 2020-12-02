using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SettleSearch.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SettleSearch.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SEOSearchController : ControllerBase
    {
        private readonly ILogger<SEOSearchController> _logger;
        private readonly ISearchService _googleService;
        private readonly ISearchService _bingService;

        public SEOSearchController(ILogger<SEOSearchController> logger, GoogleService googleService, BingService bingService)
        {
            _logger = logger;
            _googleService = googleService;
            _bingService = bingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SEOResult>>> Get(string type, string term, string uri)
        {
            Enum.TryParse(type, out SearchEngineType setype);

            switch (setype)
            {
                case SearchEngineType.Google:
                    _logger.LogInformation("Google Service was requested.");
                    var html = await _googleService.FetchResults(term);
                    return _googleService.ExtractSEOResults(html, term, uri);
                case SearchEngineType.Bing:
                    _logger.LogInformation("Bing Service was requested.");
                    var rss = await _bingService.FetchResults(term);
                    return _bingService.ExtractSEOResults(rss, term, uri);
                default:
                    _logger.LogWarning($"Unknown Service Type requested: {type}");
                    return new NotFoundResult();                    
            }
        }
    }
}
