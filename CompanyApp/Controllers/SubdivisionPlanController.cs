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
    public class SubdivisionPlanController : Controller
    {
        private readonly Context _context;

        public SubdivisionPlanController(Context context)
        {
            _context = context;
        }

        public IActionResult Index(SubdivisionPlanSort sortState, string currentFilter, string searchSubdivisionPlanName, int? page, bool reset)
        {
            if (reset)
            {
                HttpContext.Session.Remove("searchSubdivisionPlanName");
                HttpContext.Session.Remove("sortStateSubdivisionPlan");
            }
            if (searchSubdivisionPlanName != null)
            {
                page = 1;
                HttpContext.Session.SetString("searchSubdivisionPlanName", searchSubdivisionPlanName);
            }
            else if (HttpContext.Session.Keys.Contains("searchSubdivisionPlanName"))
            {
                searchSubdivisionPlanName = HttpContext.Session.GetString("searchSubdivisionPlanName");
            }
            else
            {
                searchSubdivisionPlanName = currentFilter;
            }
            if (sortState != SubdivisionPlanSort.Default)
            {
                HttpContext.Session.Set("sortStateSubdivisionPlan", sortState);
            }
            else if (HttpContext.Session.Keys.Contains("sortStateSubdivisionPlan"))
            {
                sortState = HttpContext.Session.Get<SubdivisionPlanSort>("sortStateSubdivisionPlan");
            }
            ViewBag.CurrentFilter = searchSubdivisionPlanName;
            ViewBag.CurrentSort = sortState;
            IEnumerable<SubdivisionPlanView> subdivisionPlans = GetSubdivisionPlans();
            subdivisionPlans = Sort(subdivisionPlans, sortState);
            subdivisionPlans = Search(subdivisionPlans, searchSubdivisionPlanName);
            int pageNumber = page ?? 1;
            return View(subdivisionPlans.ToPagedList(pageNumber, 20));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subdivisionPlan = await _context.SubdivisionPlans
                .FirstOrDefaultAsync(f => f.subdivisionPlanId == id);
            if (subdivisionPlan == null)
            {
                return NotFound();
            }

            var subdivision = _context.Subdivisions.FirstOrDefault(f => f.subdivisionId.Equals(subdivisionPlan.subdivisionId));
            var subdivisionPlanView = new SubdivisionPlanView()
            {
                subdivisionPlanId = subdivisionPlan.subdivisionPlanId,
                subdivisionPlanDate = subdivisionPlan.subdivisionPlanDate,
                subdivisionPlanIndex = subdivisionPlan.subdivisionPlanIndex,
                subdivision = subdivision.subdivisionName
            };
            return View(subdivisionPlanView);
        }

        public IActionResult Create()
        {
            var subdivisions = new SelectList(_context.Subdivisions.AsNoTracking().AsEnumerable(), "subdivisionId", "subdivisionName");
            ViewData["subdivisions"] = subdivisions;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("peoplePlanId,peoplePlanDate,peoplePlanIndex,workpeopleId")]  */ SubdivisionPlan subdivisionPlan)
        {
            //if (ModelState != null)
            //{
            _context.Add(subdivisionPlan);
                await _context.SaveChangesAsync();
                _context.GetService<ICached<SubdivisionPlan>>().AddList("CachedSubdivisionPlan");
                return RedirectToAction(nameof(Index));

            //}
            return View(subdivisionPlan);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subdivisions = new SelectList(_context.Subdivisions.AsNoTracking().AsEnumerable(), "subdivisionId", "subdivisionName");
            ViewData["subdivisions"] = subdivisions;

            var subdivisionPlan = await _context.SubdivisionPlans.FindAsync(id);
            if (subdivisionPlan == null)
            {
                return NotFound();
            }
            return View(subdivisionPlan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,/*[Bind("peoplePlanId,peoplePlanDate,peoplePlanIndex,workpeopleId")] */ SubdivisionPlan subdivisionPlan)
        {
            //if (id != peoplePlan.peoplePlanId)
            //{
            //    return NotFound();
            //}

            //if (ModelState.IsValid)
            //{
            //try
            //{
      
            _context.SubdivisionPlans.Update(subdivisionPlan);
            await _context.SaveChangesAsync();
            _context.GetService<ICached<SubdivisionPlan>>().AddList("CachedSubdivisionPlan");
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!PeoplePlanExists(peoplePlan.peoplePlanId))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}
            return RedirectToAction(nameof(Index));
            //}
            //return View(peoplePlan);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subdivisionPlan = await _context.SubdivisionPlans
                .FirstOrDefaultAsync(f => f.subdivisionPlanId == id);
            if (subdivisionPlan == null)
            {
                return NotFound();
            }

            var subdivision = _context.Subdivisions.FirstOrDefault(s => s.subdivisionId.Equals(subdivisionPlan.subdivisionId));
            var subdivisionPlanView = new SubdivisionPlanView()
            {
                subdivisionPlanId = subdivisionPlan.subdivisionPlanId,
                subdivisionPlanDate = subdivisionPlan.subdivisionPlanDate,
                subdivisionPlanIndex = subdivisionPlan.subdivisionPlanIndex,
                subdivision = subdivision.subdivisionName
            };
            return View(subdivisionPlanView);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _context.SubdivisionPlans.Remove(await _context.SubdivisionPlans.FindAsync(id));
            await _context.SaveChangesAsync();
            _context.GetService<ICached<SubdivisionPlan>>().AddList("CachedSubdivisionPlan");
            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<SubdivisionPlanView> Search(IEnumerable<SubdivisionPlanView> subdivisionPlans, string searchName)
        {
            if (!string.IsNullOrEmpty(searchName))
            {
                subdivisionPlans = subdivisionPlans.Where(w => w.subdivision.Contains(searchName));
            }
            return subdivisionPlans;
        }

        private IEnumerable<SubdivisionPlanView> Sort(IEnumerable<SubdivisionPlanView> subdivisionPlans, SubdivisionPlanSort sortState)
        {
            ViewData["SubdivisionPlanDate"] = sortState == SubdivisionPlanSort.DateAsc ? SubdivisionPlanSort.DateDesc : SubdivisionPlanSort.DateAsc;
            ViewData["SubdivisionPlanIndex"] = sortState == SubdivisionPlanSort.IndexAsc ? SubdivisionPlanSort.IndexDesc : SubdivisionPlanSort.IndexAsc;
            ViewData["SubdivisionPlanSubdivision"] = sortState == SubdivisionPlanSort.SubdivisionAsc ? SubdivisionPlanSort.SubdivisionDesc : SubdivisionPlanSort.SubdivisionAsc;
            subdivisionPlans = sortState switch
            {
                SubdivisionPlanSort.DateAsc => subdivisionPlans.OrderBy(w => w.subdivisionPlanDate),
                SubdivisionPlanSort.DateDesc => subdivisionPlans.OrderByDescending(w => w.subdivisionPlanDate),
                SubdivisionPlanSort.IndexAsc => subdivisionPlans.OrderBy(w => w.subdivisionPlanIndex),
                SubdivisionPlanSort.IndexDesc => subdivisionPlans.OrderByDescending(w => w.subdivisionPlanIndex),
                SubdivisionPlanSort.SubdivisionAsc => subdivisionPlans.OrderBy(w => w.subdivision),
                _ => subdivisionPlans.OrderByDescending(w => w.subdivision)
            };
            return subdivisionPlans;
        }

        private IEnumerable<SubdivisionPlanView> GetSubdivisionPlans()
        {
            return from w in _context.SubdivisionPlans
                   join s in _context.Subdivisions
                   on w.subdivisionId equals s.subdivisionId
                   select new SubdivisionPlanView()
                   {
                       subdivisionPlanId = w.subdivisionPlanId,
                       subdivisionPlanDate = w.subdivisionPlanDate,
                       subdivisionPlanIndex = w.subdivisionPlanIndex,
                       subdivision = s.subdivisionName
                   };
        }

        private bool SubdivisionPlanExists(int id)
        {
            return _context.SubdivisionPlans.Any(w => w.subdivisionPlanId == id);
        }
    }
}
