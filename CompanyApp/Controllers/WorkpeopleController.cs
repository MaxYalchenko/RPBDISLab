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
    public class WorkpeopleController : Controller
    {
        private readonly Context _context;

        public WorkpeopleController(Context context)
        {
            _context = context;
        }

        public IActionResult Index(WorkpeopleSort sortState, string currentFilter, string searchWorkpeopleName, int? page, bool reset)
        {
            if (reset)
            {
                HttpContext.Session.Remove("searchWorkpeopleName");
                HttpContext.Session.Remove("sortStateWorkpeople");
            }
            if (searchWorkpeopleName != null)
            {
                page = 1;
                HttpContext.Session.SetString("searchWorkpeopleName", searchWorkpeopleName);
            }
            else if (HttpContext.Session.Keys.Contains("searchWorkpeopleName"))
            {
                searchWorkpeopleName = HttpContext.Session.GetString("searchWorkpeopleName");
            }
            else
            {
                searchWorkpeopleName = currentFilter;
            }
            if (sortState != WorkpeopleSort.Default)
            {
                HttpContext.Session.Set("sortStateWorkpeople", sortState);
            }
            else if (HttpContext.Session.Keys.Contains("sortStateWorkpeople"))
            {
                sortState = HttpContext.Session.Get<WorkpeopleSort>("sortStateWorkpeople");
            }
            ViewBag.CurrentFilter = searchWorkpeopleName;
            ViewBag.CurrentSort = sortState;
            IEnumerable<WorkpeopleView> workpeoples = GetWorkPeoples();
            workpeoples = Sort(workpeoples, sortState);
            workpeoples = Search(workpeoples, searchWorkpeopleName);
            int pageNumber = page ?? 1;
            return View(workpeoples.ToPagedList(pageNumber, 20));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workpeople = await _context.Workpeoples
                .FirstOrDefaultAsync(w => w.workpeopleId == id);
            if (workpeople == null)
            {
                return NotFound();
            }

            var subdivision = _context.Subdivisions.FirstOrDefault(s => s.subdivisionId.Equals(workpeople.subdivisionId));
            var workpeopleView = new WorkpeopleView()
            {
                workpeopleId = workpeople.workpeopleId,
                peopleName = workpeople.peopleName,
                amountPeople = workpeople.amountPeople,
                Achievements = workpeople.Achievements,
                subdivision = subdivision.subdivisionName
            };
            return View(workpeopleView);
        }

        public IActionResult Create()
        {
            var subdivisions = new SelectList(_context.Subdivisions.AsNoTracking().AsEnumerable(), "subdivisionId", "subdivisionName");

            ViewData["subdivisions"] = subdivisions;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("workpeopleId,peopleName,amountPeople,subdivisionId,Achievements")]*/  Workpeople workpeople)
        {
            //if (ModelState != null)
            //{
        

            _context.Add(workpeople);
                await _context.SaveChangesAsync();
                _context.GetService<ICached<Workpeople>>().AddList("CachedWorkpeople");
                return RedirectToAction(nameof(Index));
                
            //}
            return View(workpeople);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workpeople = await _context.Workpeoples.FindAsync(id);
            if (workpeople == null)
            {
                return NotFound();
            }

            var subdivisions = new SelectList(_context.Subdivisions.AsNoTracking().AsEnumerable(), "subdivisionId", "subdivisionName");
            ViewData["subdivisions"] = subdivisions;
            return View(workpeople);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("workpeopleId,peopleName,amountPeople,Achievements,subdivisionId")] Workpeople workpeople)
        {
            //if (id != workpeople.workpeopleId)
            //{
            //    return NotFound();
            //}

            //if (ModelState != null)
            //{
            //    try
            //    {
                    _context.Workpeoples.Update(workpeople);
                    await _context.SaveChangesAsync();
                    _context.GetService<ICached<Workpeople>>().AddList("CachedWorkpeople");
                //}
                //catch (DbUpdateConcurrencyException)
                //{
                //    if (!WorkpeopleExists(workpeople.workpeopleId))
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
            //return View(workpeople);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workpeople = await _context.Workpeoples
                .FirstOrDefaultAsync(w => w.workpeopleId == id);
            if (workpeople == null)
            {
                return NotFound();
            }

            var subdivision = _context.Subdivisions.FirstOrDefault(s => s.subdivisionId.Equals(workpeople.subdivisionId));
            var workpeopleView = new WorkpeopleView()
            {
                workpeopleId = workpeople.workpeopleId,
                peopleName = workpeople.peopleName,
                amountPeople = workpeople.amountPeople,
                Achievements = workpeople.Achievements,
                subdivision = subdivision.subdivisionName
            };
            return View(workpeopleView);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _context.Workpeoples.Remove(await _context.Workpeoples.FindAsync(id));
            await _context.SaveChangesAsync();
            _context.GetService<ICached<Workpeople>>().AddList("CachedWorkpeople");
            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<WorkpeopleView> Search(IEnumerable<WorkpeopleView> workpeoples, string searchName)
        {
            if (!string.IsNullOrEmpty(searchName))
            {
                workpeoples = workpeoples.Where(w => w.peopleName.Contains(searchName));
            }
            return workpeoples;
        }

        private IEnumerable<WorkpeopleView> Sort(IEnumerable<WorkpeopleView> workpeoples, WorkpeopleSort sortState)
        {
            ViewData["WorkpeopleName"] = sortState == WorkpeopleSort.PeopleNameAsc ? WorkpeopleSort.PeopleNameDesc : WorkpeopleSort.PeopleNameAsc;
            ViewData["WorkpeopleAmount"] = sortState == WorkpeopleSort.AmountPeopleAsc ? WorkpeopleSort.AmountPeopleDesc : WorkpeopleSort.AmountPeopleAsc;
            ViewData["WorkpeopleSubdivision"] = sortState == WorkpeopleSort.SubdivisionAsc ? WorkpeopleSort.SubdivisionDesc : WorkpeopleSort.SubdivisionAsc;
            ViewData["WorkpeopleAchievments"] = sortState == WorkpeopleSort.AchievmentsAsc ? WorkpeopleSort.AchievmentsDesc : WorkpeopleSort.AchievmentsAsc;
            workpeoples = sortState switch
            {
                WorkpeopleSort.PeopleNameAsc => workpeoples.OrderBy(w => w.peopleName),
                WorkpeopleSort.PeopleNameDesc => workpeoples.OrderByDescending(w => w.peopleName),
                WorkpeopleSort.AmountPeopleAsc => workpeoples.OrderBy(w => w.amountPeople),
                WorkpeopleSort.AmountPeopleDesc => workpeoples.OrderByDescending(w => w.amountPeople),
                WorkpeopleSort.SubdivisionAsc => workpeoples.OrderBy(w => w.subdivision),
                WorkpeopleSort.SubdivisionDesc => workpeoples.OrderByDescending(w => w.subdivision),
                WorkpeopleSort.AchievmentsAsc => workpeoples.OrderBy(w => w.Achievements),
                _ => workpeoples.OrderByDescending(w => w.Achievements)
            };
            return workpeoples;
        }

        private IEnumerable<WorkpeopleView> GetWorkPeoples()
        {
            return from w in _context.Workpeoples
                   join s in _context.Subdivisions
                   on w.subdivisionId equals s.subdivisionId
                   select new WorkpeopleView()
                   {
                       workpeopleId = w.workpeopleId,
                       peopleName = w.peopleName,
                       amountPeople = w.amountPeople,
                       Achievements = w.Achievements,
                       subdivision = s.subdivisionName
                   };
        }

        private bool WorkpeopleExists(int id)
        {
            return _context.Workpeoples.Any(w => w.workpeopleId == id);
        }
    }
}
