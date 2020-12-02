using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SettleSearch.Services
{
    public interface ISEOClientService
    {
        Task<string> Fetch(Uri url, Dictionary<string,string> parameters);
    }
}