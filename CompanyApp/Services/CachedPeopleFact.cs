using CompanyApp.Data;
using CompanyApp.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CompanyApp.Services
{
    public class CachedPeopleFact : ICached<PeopleFact>
    {
        private readonly Context _context;
        private readonly IMemoryCache _memoryCache;

        public CachedPeopleFact(Context context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public void AddList(string cacheKey)
        {
            IEnumerable<PeopleFact> peopleFacts = _context.PeopleFacts.ToList();
            if (peopleFacts.Any())
            {
                _memoryCache.Set(cacheKey, peopleFacts, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(256)));
            }
        }

        public IEnumerable<PeopleFact> GetList()
        {
            return _context.PeopleFacts.ToList();
        }

        public IEnumerable<PeopleFact> GetList(string cacheKey)
        {
            IEnumerable<PeopleFact> peopleFacts;
            if (!_memoryCache.TryGetValue(cacheKey, out peopleFacts))
            {
                peopleFacts = _context.PeopleFacts.ToList();
                if (peopleFacts.Any())
                {
                    _memoryCache.Set(cacheKey, peopleFacts, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(256)));
                }
            }
            return peopleFacts;
        }
    }
}

