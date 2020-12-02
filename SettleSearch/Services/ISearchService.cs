using System.Collections.Generic;
using System.Threading.Tasks;

namespace SettleSearch.Services
{
    public interface ISearchService
    {
        Task<string> FetchResults(string query);
        List<SEOResult> ExtractSEOResults(string html, string query, string siteDomain);
    }
}