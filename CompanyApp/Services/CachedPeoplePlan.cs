using CompanyApp.Data;
using CompanyApp.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CompanyApp.Services
{
    public class CachedPeoplePlan : ICached<PeoplePlan>
    {
        private readonly Context _context;
        private readonly IMemoryCache _memoryCache;

        public CachedPeoplePlan(Context context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public void AddList(string cacheKey)
        {
            IEnumerable<PeoplePlan> peoplePlans = _context.PeoplePlans.ToList();
            if (peoplePlans.Any())
            {
                _memoryCache.Set(cacheKey, peoplePlans, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(256)));
            }
        }

        public IEnumerable<PeoplePlan> GetList()
        {
            return _context.PeoplePlans.ToList();
        }

        public IEnumerable<PeoplePlan> GetList(string cacheKey)
        {
            IEnumerable<PeoplePlan> peoplePlans;
            if (!_memoryCache.TryGetValue(cacheKey, out peoplePlans))
            {
                peoplePlans = _context.PeoplePlans.ToList();
                if (peoplePlans.Any())
                {
                    _memoryCache.Set(cacheKey, peoplePlans, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(256)));
                }
            }
            return peoplePlans;
        }
    }
}

