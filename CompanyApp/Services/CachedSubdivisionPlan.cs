using CompanyApp.Data;
using CompanyApp.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CompanyApp.Services
{
    public class CachedSubdivisionPlan : ICached<SubdivisionPlan>
    {
        private readonly Context _context;
        private readonly IMemoryCache _memoryCache;

        public CachedSubdivisionPlan(Context context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public void AddList(string cacheKey)
        {
            IEnumerable<SubdivisionPlan> subdivisionPlans = _context.SubdivisionPlans.ToList();
            if (subdivisionPlans.Any())
            {
                _memoryCache.Set(cacheKey, subdivisionPlans, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(256)));
            }
        }

        public IEnumerable<SubdivisionPlan> GetList()
        {
            return _context.SubdivisionPlans.ToList();
        }

        public IEnumerable<SubdivisionPlan> GetList(string cacheKey)
        {
            IEnumerable<SubdivisionPlan> subdivisionPlans;
            if (!_memoryCache.TryGetValue(cacheKey, out subdivisionPlans))
            {
                subdivisionPlans = _context.SubdivisionPlans.ToList();
                if (subdivisionPlans.Any())
                {
                    _memoryCache.Set(cacheKey, subdivisionPlans, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(256)));
                }
            }
            return subdivisionPlans;
        }
    }
}

