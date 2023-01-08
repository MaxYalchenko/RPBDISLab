using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyApp.Data;
using CompanyApp.Infrastructure;
using CompanyApp.Services;
using CompanyApp.Enums;
using Microsoft.EntityFrameworkCore.Infrastructure;
using X.PagedList;
using CompanyApp.Models;
using CompanyApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace CompanyApp.Controllers
{
    public class SubdivisionFactController : Controller
    {
        private readonly Context _context;

        public SubdivisionFactController(Context context)
        {
            _context = context;
        }
        public IActionResult Index(SubdivisionFactSort sortState, string currentFilter, string searchSubdivisionFactName, int? page, bool reset)
        {
            if (reset)
            {
                HttpContext.Session.Remove("searchSubdivisionFactName");
                HttpContext.Session.Remove("sortStateSubdivisionFact");
            }
            if (searchSubdivisionFactName != null)
            {
                page = 1;
                HttpContext.Session.SetString("searchSubdivisionFactName", searchSubdivisionFactName);
            }
            else if (HttpContext.Session.Keys.Contains("searchSubdivisionFactName"))
            {
                searchSubdivisionFactName = HttpContext.Session.GetString("searchSubdivisionFactName");
            }
            else
            {
                searchSubdivisionFactName = currentFilter;
            }
            if (sortState != SubdivisionFactSort.Default)
            {
                HttpContext.Session.Set("sortStateSubdivisionFact", sortState);
            }
            else if (HttpContext.Session.Keys.Contains("sortStateSubdivisionFact"))
            {
                sortState = HttpContext.Session.Get<SubdivisionFactSort>("sortStateSubdivisionFact");
            }
            ViewBag.CurrentFilter = searchSubdivisionFactName;
            ViewBag.CurrentSort = sortState;
            IEnumerable<SubdivisionFactView> subdivisionFacts = GetSubdivisionFacts();
            subdivisionFacts = Sort(subdivisionFacts, sortState);
            subdivisionFacts = Search(subdivisionFacts, searchSubdivisionFactName);
            int pageNumber = page ?? 1;
            return View(subdivisionFacts.ToPagedList(pageNumber, 20));
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subdivisionFact = await _context.SubdivisionFacts
                .FirstOrDefaultAsync(f => f.subdivisionFactId == id);
            if (subdivisionFact == null)
            {
                return NotFound();
            }
            var subdivision = _context.Subdivisions.FirstOrDefault(f => f.subdivisionId.Equals(subdivisionFact.subdivisionId));
            var subdivisionFactView = new SubdivisionFactView()
            {
                subdivisionFactId = subdivisionFact.subdivisionFactId,
                subdivisionFactDate = subdivisionFact.subdivisionFactDate,
                subdivisionFactIndex = subdivisionFact.subdivisionFactIndex,
                subdivision = subdivision.subdivisionName
            };
            return View(subdivisionFactView);
        }
        public IActionResult Create()
        {
            var subdivisions = new SelectList(_context.Subdivisions.AsNoTracking().AsEnumerable(), "subdivisionId", "subdivisionName");
            ViewData["subdivisions"] = subdivisions;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubdivisionFact subdivisionFact)
        {
                _context.Add(subdivisionFact);
                await _context.SaveChangesAsync();
                _context.GetService<ICached<SubdivisionFact>>().AddList("CachedSubdivisionFact");
                return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subdivisions = new SelectList(_context.Subdivisions.AsNoTracking().AsEnumerable(), "subdivisionId", "subdivisionName");
            ViewData["subdivisions"] = subdivisions;
            var subdivisionFact = await _context.SubdivisionFacts.FindAsync(id);
            if (subdivisionFact == null)
            {
                return NotFound();
            }
            return View(subdivisionFact);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubdivisionFact subdivisionFact)
        {
                    _context.Update(subdivisionFact);
                    await _context.SaveChangesAsync();
                    _context.GetService<ICached<SubdivisionFact>>().AddList("CachedSubdivisionFact");
                return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subdivisionFact = await _context.SubdivisionFacts
                .FirstOrDefaultAsync(f => f.subdivisionFactId == id);
            if (subdivisionFact == null)
            {
                return NotFound();
            }
            var subdivision = _context.Subdivisions.FirstOrDefault(s => s.subdivisionId.Equals(subdivisionFact.subdivisionId));
            var subdivisionFactView = new SubdivisionFactView()
            {
                subdivisionFactId = subdivisionFact.subdivisionFactId,
                subdivisionFactDate = subdivisionFact.subdivisionFactDate,
                subdivisionFactIndex = subdivisionFact.subdivisionFactIndex,
                subdivision = subdivision.subdivisionName
            };
            return View(subdivisionFactView);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _context.SubdivisionFacts.Remove(await _context.SubdivisionFacts.FindAsync(id));
            await _context.SaveChangesAsync();
            _context.GetService<ICached<SubdivisionFact>>().AddList("CachedSubdivisionFact");
            return RedirectToAction(nameof(Index));
        }
        private IEnumerable<SubdivisionFactView> Search(IEnumerable<SubdivisionFactView> subdivisionFacts, string searchName)
        {
            if (!string.IsNullOrEmpty(searchName))
            {
                subdivisionFacts = subdivisionFacts.Where(w => w.subdivision.Contains(searchName));
            }
            return subdivisionFacts;
        }
        private IEnumerable<SubdivisionFactView> Sort(IEnumerable<SubdivisionFactView> subdivisionFacts, SubdivisionFactSort sortState)
        {
            ViewData["SubdivisionFactDate"] = sortState == SubdivisionFactSort.DateAsc ? SubdivisionFactSort.DateDesc : SubdivisionFactSort.DateAsc;
            ViewData["SubdivisionFactIndex"] = sortState == SubdivisionFactSort.IndexAsc ? SubdivisionFactSort.IndexDesc : SubdivisionFactSort.IndexAsc;
            ViewData["SubdivisionFactSubdivision"] = sortState == SubdivisionFactSort.SubdivisionAsc ? SubdivisionFactSort.SubdivisionDesc : SubdivisionFactSort.SubdivisionAsc;
            subdivisionFacts = sortState switch
            {
                SubdivisionFactSort.DateAsc => subdivisionFacts.OrderBy(w => w.subdivisionFactDate),
                SubdivisionFactSort.DateDesc => subdivisionFacts.OrderByDescending(w => w.subdivisionFactDate),
                SubdivisionFactSort.IndexAsc => subdivisionFacts.OrderBy(w => w.subdivisionFactIndex),
                SubdivisionFactSort.IndexDesc => subdivisionFacts.OrderByDescending(w => w.subdivisionFactIndex),
                SubdivisionFactSort.SubdivisionAsc => subdivisionFacts.OrderBy(w => w.subdivision),
                _ => subdivisionFacts.OrderByDescending(w => w.subdivision)
            };
            return subdivisionFacts;
        }
        private IEnumerable<SubdivisionFactView> GetSubdivisionFacts()
        {
            return from w in _context.SubdivisionFacts
                   join s in _context.Subdivisions
                   on w.subdivisionId equals s.subdivisionId
                   select new SubdivisionFactView()
                   {
                       subdivisionFactId = w.subdivisionFactId,
                       subdivisionFactDate = w.subdivisionFactDate,
                       subdivisionFactIndex = w.subdivisionFactIndex,
                       subdivision = s.subdivisionName
                   };
        }
        private bool SubdivisionFactExists(int id)
        {
            return _context.SubdivisionFacts.Any(w => w.subdivisionFactId == id);
        }
    }
}
