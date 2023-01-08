using CompanyApp.Data;
using CompanyApp.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CompanyApp.Services
{
    public class CachedSubdivisionFact : ICached<SubdivisionFact>
    {
        private readonly Context _context;
        private readonly IMemoryCache _memoryCache;

        public CachedSubdivisionFact(Context context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public void AddList(string cacheKey)
        {
            IEnumerable<SubdivisionFact> subdivisionFacts = _context.SubdivisionFacts.ToList();
            if (subdivisionFacts.Any())
            {
                _memoryCache.Set(cacheKey, subdivisionFacts, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(256)));
            }
        }

        public IEnumerable<SubdivisionFact> GetList()
        {
            return _context.SubdivisionFacts.ToList();
        }

        public IEnumerable<SubdivisionFact> GetList(string cacheKey)
        {
            IEnumerable<SubdivisionFact> subdivisionFacts;
            if (!_memoryCache.TryGetValue(cacheKey, out subdivisionFacts))
            {
                subdivisionFacts = _context.SubdivisionFacts.ToList();
                if (subdivisionFacts.Any())
                {
                    _memoryCache.Set(cacheKey, subdivisionFacts, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(256)));
                }
            }
            return subdivisionFacts;
        }
    }
}

