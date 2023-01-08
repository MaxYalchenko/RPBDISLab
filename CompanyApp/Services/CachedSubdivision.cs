using CompanyApp.Data;
using CompanyApp.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CompanyApp.Services
{
    public class CachedSubdivision : ICached<Subdivision>
    {
        private readonly Context _context;
        private readonly IMemoryCache _memoryCache;

        public CachedSubdivision(Context context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public void AddList(string cacheKey)
        {
            IEnumerable<Subdivision> subdivisions = _context.Subdivisions.ToList();
            if (subdivisions.Any())
            {
                _memoryCache.Set(cacheKey, subdivisions, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(256)));
            }
        }

        public IEnumerable<Subdivision> GetList()
        {
            return _context.Subdivisions.ToList();
        }

        public IEnumerable<Subdivision> GetList(string cacheKey)
        {
            IEnumerable<Subdivision> subdivisions;
            if (!_memoryCache.TryGetValue(cacheKey, out subdivisions))
            {
                subdivisions = _context.Subdivisions.ToList();
                if (subdivisions.Any())
                {
                    _memoryCache.Set(cacheKey, subdivisions, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(256)));
                }
            }
            return subdivisions;
        }
    }
}
