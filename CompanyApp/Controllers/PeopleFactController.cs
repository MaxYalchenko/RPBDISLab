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
    public class PeopleFactController : Controller
    {
        private readonly Context _context;

        public PeopleFactController(Context context)
        {
            _context = context;
        }
        public IActionResult Index(PeopleFactSort sortState, string currentFilter, string searchPeopleFactName, int? page, bool reset)
        {
            if (reset)
            {
                HttpContext.Session.Remove("searchPeopleFactName");
                HttpContext.Session.Remove("sortStatePeopleFact");
            }
            if (searchPeopleFactName != null)
            {
                page = 1;
                HttpContext.Session.SetString("searchPeopleFactName", searchPeopleFactName);
            }
            else if (HttpContext.Session.Keys.Contains("searchPeopleFactName"))
            {
                searchPeopleFactName = HttpContext.Session.GetString("searchPeopleFactName");
            }
            else
            {
                searchPeopleFactName = currentFilter;
            }
            if (sortState != PeopleFactSort.Default)
            {
                HttpContext.Session.Set("sortStatePeopleFact", sortState);
            }
            else if (HttpContext.Session.Keys.Contains("sortStatePeopleFact"))
            {
                sortState = HttpContext.Session.Get<PeopleFactSort>("sortStatePeopleFact");
            }
            ViewBag.CurrentFilter = searchPeopleFactName;
            ViewBag.CurrentSort = sortState;
            IEnumerable<PeopleFactView> peopleFacts = GetPeopleFacts();
            peopleFacts = Sort(peopleFacts, sortState);
            peopleFacts = Search(peopleFacts, searchPeopleFactName);
            int pageNumber = page ?? 1;
            return View(peopleFacts.ToPagedList(pageNumber, 20));
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var peopleFact = await _context.PeopleFacts
                .FirstOrDefaultAsync(f => f.peopleFactId == id);
            if (peopleFact == null)
            {
                return NotFound();
            }
            var workpeople = _context.Workpeoples.FirstOrDefault(f => f.workpeopleId.Equals(peopleFact.workPeopleId));
            var peopleFactView = new PeopleFactView()
            {
                peopleFactId = peopleFact.peopleFactId,
                peopleFactDate = peopleFact.peopleFactDate,
                peopleFactIndex = peopleFact.peopleFactIndex,
                workpeople = workpeople.peopleName
            };
            return View(peopleFactView);
        }
        public IActionResult Create()
        {
            var workPeoples = new SelectList(_context.Workpeoples.AsNoTracking().AsEnumerable(), "workpeopleId", "peopleName");
            ViewData["workPeoples"] = workPeoples;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PeopleFact peopleFact)
        {
            _context.Add(peopleFact);
            await _context.SaveChangesAsync();
            _context.GetService<ICached<PeopleFact>>().AddList("CachedPeopleFact");
            return RedirectToAction(nameof(Index));
            //return View(peopleFact);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var workPeoples = new SelectList(_context.Workpeoples.AsNoTracking().AsEnumerable(), "workpeopleId", "peopleName");
            ViewData["workPeoples"] = workPeoples;
            var peopleFact = await _context.PeopleFacts.FindAsync(id);
            if (peopleFact == null)
            {
                return NotFound();
            }
            return View(peopleFact);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PeopleFact peopleFact)
        {
            _context.Update(peopleFact);
            await _context.SaveChangesAsync();
            _context.GetService<ICached<PeopleFact>>().AddList("CachedPeopleFact");
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var peopleFact = await _context.PeopleFacts
                .FirstOrDefaultAsync(f => f.peopleFactId == id);
            if (peopleFact == null)
            {
                return NotFound();
            }
            var workpeople = _context.Workpeoples.FirstOrDefault(s => s.workpeopleId.Equals(peopleFact.workPeopleId));
            var peopleFactView = new PeopleFactView()
            {
                peopleFactId = peopleFact.peopleFactId,
                peopleFactDate = peopleFact.peopleFactDate,
                peopleFactIndex = peopleFact.peopleFactIndex,
                workpeople = workpeople.peopleName
            };
            return View(peopleFactView);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _context.PeopleFacts.Remove(await _context.PeopleFacts.FindAsync(id));
            await _context.SaveChangesAsync();
            _context.GetService<ICached<PeopleFact>>().AddList("CachedPeopleFact");
            return RedirectToAction(nameof(Index));
        }
        private IEnumerable<PeopleFactView> Search(IEnumerable<PeopleFactView> peopleFacts, string searchName)
        {
            if (!string.IsNullOrEmpty(searchName))
            {
                peopleFacts = peopleFacts.Where(w => w.workpeople.Contains(searchName));
            }
            return peopleFacts;
        }
        private IEnumerable<PeopleFactView> Sort(IEnumerable<PeopleFactView> peopleFacts, PeopleFactSort sortState)
        {
            ViewData["PeopleFactDate"] = sortState == PeopleFactSort.DateAsc ? PeopleFactSort.DateDesc : PeopleFactSort.DateAsc;
            ViewData["PeopleFactIndex"] = sortState == PeopleFactSort.IndexAsc ? PeopleFactSort.IndexDesc : PeopleFactSort.IndexAsc;
            ViewData["PeopleFactWorkpeople"] = sortState == PeopleFactSort.WorkpeopleAsc ? PeopleFactSort.WorkpeopleDesc : PeopleFactSort.WorkpeopleAsc;
            peopleFacts = sortState switch
            {
                PeopleFactSort.DateAsc => peopleFacts.OrderBy(w => w.peopleFactDate),
                PeopleFactSort.DateDesc => peopleFacts.OrderByDescending(w => w.peopleFactDate),
                PeopleFactSort.IndexAsc => peopleFacts.OrderBy(w => w.peopleFactIndex),
                PeopleFactSort.IndexDesc => peopleFacts.OrderByDescending(w => w.peopleFactIndex),
                PeopleFactSort.WorkpeopleAsc => peopleFacts.OrderBy(w => w.workpeople),
                _ => peopleFacts.OrderByDescending(w => w.workpeople)
            };
            return peopleFacts;
        }
        private IEnumerable<PeopleFactView> GetPeopleFacts()
        {
            return from w in _context.PeopleFacts
                   join s in _context.Workpeoples
                   on w.workPeopleId equals s.workpeopleId
                   select new PeopleFactView()
                   {
                       peopleFactId = w.peopleFactId,
                       peopleFactDate = w.peopleFactDate,
                       peopleFactIndex = w.peopleFactIndex,
                       workpeople = s.peopleName
                   };
        }
        private bool PeopleFactExists(int id)
        {
            return _context.PeopleFacts.Any(w => w.peopleFactId == id);
        }
    }
}
