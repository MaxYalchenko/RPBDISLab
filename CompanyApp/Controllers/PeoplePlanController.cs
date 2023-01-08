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
    public class PeoplePlanController : Controller
    {
        private readonly Context _context;

        public PeoplePlanController(Context context)
        {
            _context = context;
        }

        public IActionResult Index(PeoplePlanSort sortState, string currentFilter, string searchPeoplePlanName, int? page, bool reset)
        {
            if (reset)
            {
                HttpContext.Session.Remove("searchPeoplePlanName");
                HttpContext.Session.Remove("sortStatePeoplePlan");
            }
            if (searchPeoplePlanName != null)
            {
                page = 1;
                HttpContext.Session.SetString("searchPeoplePlanName", searchPeoplePlanName);
            }
            else if (HttpContext.Session.Keys.Contains("searchPeoplePlanName"))
            {
                searchPeoplePlanName = HttpContext.Session.GetString("searchPeoplePlanName");
            }
            else
            {
                searchPeoplePlanName = currentFilter;
            }
            if (sortState != PeoplePlanSort.Default)
            {
                HttpContext.Session.Set("sortStatePeoplePlan", sortState);
            }
            else if (HttpContext.Session.Keys.Contains("sortStatePeoplePlan"))
            {
                sortState = HttpContext.Session.Get<PeoplePlanSort>("sortStatePeoplePlan");
            }
            ViewBag.CurrentFilter = searchPeoplePlanName;
            ViewBag.CurrentSort = sortState;
            IEnumerable<PeoplePlanView> peoplePlans = GetPeoplePlans();
            peoplePlans = Sort(peoplePlans, sortState);
            peoplePlans = Search(peoplePlans, searchPeoplePlanName);
            int pageNumber = page ?? 1;
            return View(peoplePlans);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var peoplePlan = await _context.PeoplePlans
                .FirstOrDefaultAsync(f => f.peoplePlanId == id);
            if (peoplePlan == null)
            {
                return NotFound();
            }

            var workpeople = _context.Workpeoples.FirstOrDefault(f => f.workpeopleId.Equals(peoplePlan.workPeopleId));
            var peoplePlanView = new PeoplePlanView()
            {
                peoplePlanId = peoplePlan.peoplePlanId,
                peoplePlanDate = peoplePlan.peoplePlanDate,
                peoplePlanIndex = peoplePlan.peoplePlanIndex,
                workpeople = workpeople.peopleName
            };
            return View(peoplePlanView);
        }

        public IActionResult Create()
        {
            var workPeoples = new SelectList(_context.Workpeoples.AsNoTracking().AsEnumerable(), "workpeopleId", "peopleName");
            ViewData["workPeoples"] = workPeoples;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("peoplePlanId,peoplePlanDate,peoplePlanIndex,workpeopleId")]  */PeoplePlan peoplePlan)
        {
            //if (ModelState != null)
            //{
                _context.Add(peoplePlan);
                await _context.SaveChangesAsync();
                _context.GetService<ICached<PeoplePlan>>().AddList("CachedPeoplePlan");
                return RedirectToAction(nameof(Index));    
                
            //}
            return View(peoplePlan);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var workPeoples = new SelectList(_context.Workpeoples.AsNoTracking().AsEnumerable(), "workpeopleId", "peopleName");
            ViewData["workPeoples"] = workPeoples;

            var peoplePlan = await _context.PeoplePlans.FindAsync(id);
            if (peoplePlan == null)
            {
                return NotFound();
            }
            return View(peoplePlan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, /*[Bind("peoplePlanId,peoplePlanDate,peoplePlanIndex,workpeopleId")] */PeoplePlan peoplePlan)
        {
            //if (id != peoplePlan.peoplePlanId)
            //{
            //    return NotFound();
            //}

            //if (ModelState.IsValid)
            //{
                //try
                //{
                    _context.PeoplePlans.Update(peoplePlan);
                    await _context.SaveChangesAsync();
                    _context.GetService<ICached<PeoplePlan>>().AddList("CachedPeoplePlan");
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

            var peoplePlan = await _context.PeoplePlans
                .FirstOrDefaultAsync(f => f.peoplePlanId == id);
            if (peoplePlan == null)
            {
                return NotFound();
            }

            var workpeople = _context.Workpeoples.FirstOrDefault(s => s.workpeopleId.Equals(peoplePlan.workPeopleId));
            var peoplePlanView = new PeoplePlanView()
            {
                peoplePlanId = peoplePlan.peoplePlanId,
                peoplePlanDate = peoplePlan.peoplePlanDate,
                peoplePlanIndex = peoplePlan.peoplePlanIndex,
                workpeople = workpeople.peopleName
            };
            return View(peoplePlanView);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _context.PeoplePlans.Remove(await _context.PeoplePlans.FindAsync(id));
            await _context.SaveChangesAsync();
            _context.GetService<ICached<PeoplePlan>>().AddList("CachedPeoplePlan");
            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<PeoplePlanView> Search(IEnumerable<PeoplePlanView> peoplePlans, string searchName)
        {
            if (!string.IsNullOrEmpty(searchName))
            {
                peoplePlans = peoplePlans.Where(w => w.workpeople.Contains(searchName));
            }
            return peoplePlans;
        }

        private IEnumerable<PeoplePlanView> Sort(IEnumerable<PeoplePlanView> peoplePlans, PeoplePlanSort sortState)
        {
            ViewData["PeoplePlanDate"] = sortState == PeoplePlanSort.DateAsc ? PeoplePlanSort.DateDesc : PeoplePlanSort.DateAsc;
            ViewData["PeoplePlanIndex"] = sortState == PeoplePlanSort.IndexAsc ? PeoplePlanSort.IndexDesc : PeoplePlanSort.IndexAsc;
            ViewData["PeoplePlanWorkpeople"] = sortState == PeoplePlanSort.WorkpeopleAsc ? PeoplePlanSort.WorkpeopleDesc : PeoplePlanSort.WorkpeopleAsc;
            peoplePlans = sortState switch
            {
                PeoplePlanSort.DateAsc => peoplePlans.OrderBy(w => w.peoplePlanDate),
                PeoplePlanSort.DateDesc => peoplePlans.OrderByDescending(w => w.peoplePlanDate),
                PeoplePlanSort.IndexAsc => peoplePlans.OrderBy(w => w.peoplePlanIndex),
                PeoplePlanSort.IndexDesc => peoplePlans.OrderByDescending(w => w.peoplePlanIndex),
                PeoplePlanSort.WorkpeopleAsc => peoplePlans.OrderBy(w => w.workpeople),
                _ => peoplePlans.OrderByDescending(w => w.workpeople)
            };
            return peoplePlans;
        }

        private IEnumerable<PeoplePlanView> GetPeoplePlans()
        {
            return from w in _context.PeoplePlans
                   join s in _context.Workpeoples
                   on w.workPeopleId equals s.workpeopleId
                   select new PeoplePlanView()
                   {
                       peoplePlanId = w.peoplePlanId,
                       peoplePlanDate = w.peoplePlanDate,
                       peoplePlanIndex = w.peoplePlanIndex,
                       workpeople = s.peopleName
                   };
        }

        private bool PeoplePlanExists(int id)
        {
            return _context.PeoplePlans.Any(w => w.peoplePlanId == id);
        }
    }
}
