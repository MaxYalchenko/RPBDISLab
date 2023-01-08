using CompanyApp.Data;
using CompanyApp.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CompanyApp.Services
{
    public class CachedWorkpeople : ICached<Workpeople>
    {
        private readonly Context _context;
        private readonly IMemoryCache _memoryCache;

        public CachedWorkpeople(Context context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public void AddList(string cacheKey)
        {
            IEnumerable<Workpeople> workpeoples = _context.Workpeoples.ToList();
            if (workpeoples.Any())
            {
                _memoryCache.Set(cacheKey, workpeoples, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(256)));
            }
        }

        public IEnumerable<Workpeople> GetList()
        {
            return _context.Workpeoples.ToList();
        }

        public IEnumerable<Workpeople> GetList(string cacheKey)
        {
            IEnumerable<Workpeople> workpeoples;
            if (!_memoryCache.TryGetValue(cacheKey, out workpeoples))
            {
                workpeoples = _context.Workpeoples.ToList();
                if (workpeoples.Any())
                {
                    _memoryCache.Set(cacheKey, workpeoples, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(256)));
                }
            }
            return workpeoples;
        }
    }
}
