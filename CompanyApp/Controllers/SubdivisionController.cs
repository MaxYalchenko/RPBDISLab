using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CompanyApp.Data;
using CompanyApp.Infrastructure;
using CompanyApp.Services;
using CompanyApp.Enums;
using Microsoft.EntityFrameworkCore.Infrastructure;
using X.PagedList;
using CompanyApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
namespace CompanyApp.Controllers
{
    public class SubdivisionController : Controller
    {
        private readonly Context _context;

        public SubdivisionController(Context context)
        {
            _context = context;
        }


        public IActionResult Index(SubdivisionSort sortState, string currentFilter, string searchSubdivisionName, int? page, bool reset)
        {
            if (reset)
            {
                HttpContext.Session.Remove("searchSubdivisionName");
                HttpContext.Session.Remove("sortStateSubdivision");
            }
            if (searchSubdivisionName != null)
            {
                page = 1;
                HttpContext.Session.SetString("searchSubdivisionName", searchSubdivisionName);
            }
            else if (HttpContext.Session.Keys.Contains("searchSubdivisionName"))
            {
                searchSubdivisionName = HttpContext.Session.GetString("searchSubdivisionName");
            }
            else
            {
                searchSubdivisionName = currentFilter;
            }
            if (sortState != SubdivisionSort.Default)
            {
                HttpContext.Session.Set("sortStateSubdivision", sortState);
            }
            else if (HttpContext.Session.Keys.Contains("sortStateSubdivision"))
            {
                sortState = HttpContext.Session.Get<SubdivisionSort>("sortStateSubdivision");
            }
            ViewBag.CurrentFilter = searchSubdivisionName;
            ViewBag.CurrentSort = sortState;
            IEnumerable<Subdivision> subdivisions = GetSubdivisions();
            subdivisions = Sort(subdivisions, sortState);
            subdivisions = Search(subdivisions, searchSubdivisionName);
            int pageNumber = page ?? 1;
            return View(subdivisions.ToPagedList(pageNumber, 20));
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subdivision = await _context.Subdivisions
                .FirstOrDefaultAsync(s => s.subdivisionId == id);
            if (subdivision == null)
            {
                return NotFound();
            }

            return View(subdivision);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("subdivisionId,subdivisionName,amountSubdivision")] Subdivision subdivision)
        {
            if (ModelState != null)
            {
                _context.Add(subdivision);
                await _context.SaveChangesAsync();
                _context.GetService<ICached<Subdivision>>().AddList("CachedSubdivision");
                return RedirectToAction(nameof(Index));
            }

            return View(subdivision);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subdivision = await _context.Subdivisions.FindAsync(id);
            if (subdivision == null)
            {
                return NotFound();
            }
            return View(subdivision);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("subdivisionId,subdivisonName,amountSubdivision")] Subdivision subdivision)
        {
            if (id != subdivision.subdivisionId)
            {
                return NotFound();
            }

            if (ModelState != null)
            {
                try
                {
                    _context.Update(subdivision);
                    await _context.SaveChangesAsync();
                    _context.GetService<ICached<Subdivision>>().AddList("CachedSubdivision");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubdivisionExists(subdivision.subdivisionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(subdivision);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subdivision = await _context.Subdivisions
                .FirstOrDefaultAsync(s => s.subdivisionId == id);
            if (subdivision == null)
            {
                return NotFound();
            }

            return View(subdivision);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _context.Subdivisions.Remove(await _context.Subdivisions.FindAsync(id));
            await _context.SaveChangesAsync();
            _context.GetService<ICached<Subdivision>>().AddList("CachedSubdivision");
            return RedirectToAction(nameof(Index));
        }
        private IEnumerable<Subdivision> Search(IEnumerable<Subdivision> subdivisions, string searchName)
        {
            if (!string.IsNullOrEmpty(searchName))
            {
                subdivisions = subdivisions.Where(s => s.subdivisionName.Contains(searchName));
            }
            return subdivisions;
        }
        private IEnumerable<Subdivision> Sort(IEnumerable<Subdivision> subdivisions, SubdivisionSort sortState)
        {
            ViewData["SubdivisionName"] = sortState == SubdivisionSort.NameAsc ? SubdivisionSort.NameDesc : SubdivisionSort.NameAsc;
            ViewData["SubdivisionAmount"] = sortState == SubdivisionSort.AmountAsc ? SubdivisionSort.AmountDesc : SubdivisionSort.AmountAsc;
            subdivisions = sortState switch
            {
                SubdivisionSort.NameAsc => subdivisions.OrderBy(s => s.subdivisionName),
                SubdivisionSort.NameDesc => subdivisions.OrderByDescending(s => s.subdivisionName),
                SubdivisionSort.AmountAsc => subdivisions.OrderBy(s => s.amountSubdivision),
                _ => subdivisions.OrderByDescending(s => s.amountSubdivision)
            };
            return subdivisions;
        }

        private IEnumerable<Subdivision> GetSubdivisions()
        {
            return _context.Subdivisions;
        }

        private bool SubdivisionExists(int id)
        {
            return _context.Subdivisions.Any(s => s.subdivisionId == id);
        }
    }
}
