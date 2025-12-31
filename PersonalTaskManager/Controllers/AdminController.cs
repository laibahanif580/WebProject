using Microsoft.AspNetCore.Authorization;
using PersonalTaskManager.Models;
using Microsoft.AspNetCore.Identity;
using PersonalTaskManager.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PersonalTaskManager.Hubs;
using Microsoft.AspNetCore.SignalR;


namespace PersonalTaskManager.Controllers
{
    [Authorize(Policy = "AdminOnly")]  // Only admin can access
    public class AdminController : Controller
    {
        private readonly IAnnouncementRepository _repo;
        private readonly ITaskRepository _taskRepository;

        private readonly UserManager<IdentityUser> _userManager;
        public AdminController(IAnnouncementRepository repo, ITaskRepository taskRepository, UserManager<IdentityUser> userManager)
        {
            _taskRepository = taskRepository;
            _repo = repo;
            _userManager = userManager;
        }
        // /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var allUsers = _userManager.Users.ToList();

            int totalUsers = 0;
            int pendingApprovals = 0;
            int pendingTasks = 0;
            int overdueTasks = 0;

            foreach (var user in allUsers)
            {
                var claims = await _userManager.GetClaimsAsync(user);

                // Exclude Admins
                if (!claims.Any(c => c.Type == "TaskPermission" && c.Value == "Admin"))
                {
                    totalUsers++;

                    // Example: track pending approval via claim (if you have one)
                    if (claims.Any(c => c.Type == "TaskPermission" && c.Value == "Viewer"))
                        pendingApprovals++;

                    // Get user tasks dynamically
                    var tasks = await _taskRepository.GetTasksAsync(user.Id);

                    pendingTasks += tasks.Count(t => t.Status == "Pending" || t.Status == "In-Progress");
                    overdueTasks += tasks.Count(t =>
                        (t.Status == "Pending" || t.Status == "In-Progress") &&
                        t.DueDate.HasValue &&
                        t.DueDate.Value < DateTime.UtcNow
                    );
                }
            }

            ViewBag.TotalUsers = totalUsers;
            ViewBag.PendingApprovals = pendingApprovals;
            ViewBag.PendingTasks = pendingTasks;
            ViewBag.OverdueTasks = overdueTasks;

            return View();
        }
        public async Task<IActionResult> Users()
        {
            var allUsers = _userManager.Users.ToList();
            var users = new List<IdentityUser>();
            var userClaimsDict = new Dictionary<string, bool>();

            foreach (var user in allUsers)
            {
                var claim = await _userManager.GetClaimsAsync(user);

                // Check if user has Admin claim
                if (claim.Any(c => c.Type == "TaskPermission" && c.Value == "Admin"))
                {
                    continue; // skip Admins
                }
                users.Add(user);

                var claims = await _userManager.GetClaimsAsync(user);
                // Check if user already has Editor claim
                bool isEditor = claims.Any(c => c.Type == "TaskPermission" && c.Value == "Editor");
                userClaimsDict[user.Id] = isEditor;
            }

            ViewBag.UserClaims = userClaimsDict;
            return View(users);
        }
        public async Task<IActionResult> UserActivity()
        {
            var allUsers = _userManager.Users.ToList();
            var userActivities = new List<dynamic>();

            foreach (var user in allUsers)
            {
                var claims = await _userManager.GetClaimsAsync(user);

                // Skip Admins
                if (claims.Any(c => c.Type == "TaskPermission" &&(c.Value == "Admin" || c.Value == "Viewer")))
                    continue;

                // Get tasks for this user
                var tasks = await _taskRepository.GetTasksAsync(user.Id); // ✅ await the async method

                var activity = new UserActivity
                {
                    UserId = user.Id,
                    Email = user.Email,
                    TotalTasks = tasks.Count(),
                    Completed = tasks.Count(t => t.Status == "Completed"),
                    Pending = tasks.Count(t => t.Status == "Pending"),
                    InProgress = tasks.Count(t => t.Status == "In-Progress"),
                    Overdue = tasks.Count(t => t.DueDate < DateTime.Now && t.Status != "Completed"),
                    HighPriority = tasks.Count(t => t.Priority == "High")
                };


                userActivities.Add(activity);
            }

            return View(userActivities);
        }

        public IActionResult AllAnnouncements()
        {
            return View();
        }


        public IActionResult Announcements()
        {
            return View(); // View just renders <vc:announcements>
        }
        [HttpPost]
        public async Task<IActionResult> CreateAnnouncement(string title, string message)
        {
            await _repo.CreateAsync(title, message);
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> MakeEditor(string id)
        {
            if (id == null) return Json(new { success = false, message = "User ID not provided" });

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return Json(new { success = false, message = "User not found" });

            var claims = await _userManager.GetClaimsAsync(user);
            var existingClaim = claims.FirstOrDefault(c => c.Type == "TaskPermission");

            if (existingClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, existingClaim);
            }

            await _userManager.AddClaimAsync(user, new Claim("TaskPermission", "Editor"));

            return Json(new { success = true, userId = user.Id });
        }
    }
}
