using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Gymbokning.Controllers
{
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id == null)
                return NotFound();

            // Find the gym class by ID
            var gymClass = await _context.GymClasses
                .Include(gc => gc.AttendingMembers)
                .ThenInclude(aug => aug.ApplicationUser)
                .FirstOrDefaultAsync(gc => gc.Id == id);

            if (gymClass == null)
                return NotFound();

            // Get the currently logged-in user
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return NotFound();

            // Check if the user is already attending this gym class
            var existingBooking = gymClass.AttendingMembers
                .FirstOrDefault(aug => aug.ApplicationUserId == currentUser.Id);

            if (existingBooking != null)
            {
                // User is already booked, so remove the booking
                _context.ApplicationUserGymClasses.Remove(existingBooking);
            }
            else
            {
                // User is not booked, so add the booking
                var newBooking = new ApplicationUserGymClass
                {
                    ApplicationUserId = currentUser.Id,
                    GymClassId = gymClass.Id
                };
                await _context.ApplicationUserGymClasses.AddAsync(newBooking);
            }

            await _context.SaveChangesAsync();

            // Redirect back to the index or details page as appropriate
            return RedirectToAction(nameof(Index)); // or RedirectToAction(nameof(Details), new { id = gymClass.Id });
        }

        // GET: GymClasses
        public async Task<IActionResult> Index()
        {
            return View(await _context.GymClasses.ToListAsync());
        }

        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the gym class, including attending members and their associated users
            var gymClass = await _context.GymClasses
                .Include(gc => gc.AttendingMembers)
                    .ThenInclude(aug => aug.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // GET: GymClasses/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                // Convert Duration from string HH:mm to TimeSpan
                if (TimeSpan.TryParse(gymClass.Duration.ToString(), out TimeSpan duration))
                {
                    gymClass.Duration = duration;
                }
                else
                {
                    ModelState.AddModelError("Duration", "Invalid duration format.");
                    return View(gymClass);
                }

                // Check if Duration exceeds 24 hours
                if (gymClass.Duration.TotalHours > 24)
                {
                    ModelState.AddModelError("Duration", "Duration must be less than or equal to 24 hours.");
                    return View(gymClass);
                }

                _context.Add(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
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
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass != null)
            {
                _context.GymClasses.Remove(gymClass);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return _context.GymClasses.Any(e => e.Id == id);
        }
    }
}
