using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SettleSearch.Services
{
    public class HttpClientService : ISEOClientService
    {
        public async Task<string> Fetch(Uri uri, Dictionary<string,string> parameters)
        {
            var client = new HttpClient();
            var address = $"{uri}" + string.Join('&', parameters.Select(kvp => $"{kvp.Key.ToLower()}={kvp.Value}"));
            var request = new HttpRequestMessage(HttpMethod.Get, address);
            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
