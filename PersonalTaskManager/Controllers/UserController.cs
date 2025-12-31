using Microsoft.AspNetCore.Authorization;  //Authorize attributte
using PersonalTaskManager.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PersonalTaskManager.Controllers
{
    [Authorize(Policy = "TaskAccess")]  // Only admin can access
    public class UserController : Controller
    {
        // GET: /User/Dashboard
        private readonly IAnnouncementRepository _repo;
        private readonly ITaskRepository _taskRepo;


        public UserController(IAnnouncementRepository repo, ITaskRepository taskRepo)
        {
            _repo = repo;
            _taskRepo = taskRepo;
        }
        public IActionResult Announcements()
        {
            return View(); // View just renders <vc:announcements>
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // get logged-in user id

            var allTasks = await _taskRepo.GetTasksAsync(userId);
            var workTasksList = await _taskRepo.GetTasksAsync(userId, category: "Work");
            var personalTasksList = await _taskRepo.GetTasksAsync(userId, category: "Personal");

            // Count the tasks
            var pendingTasks = allTasks.Count(t => t.Status == "Pending");
            var completedTasks = allTasks.Count(t => t.Status == "Completed");
            var workTasks = workTasksList.Count();
            var personalTasks = personalTasksList.Count();
            var taskSummary = new[]
            {
            new { Name = "Pending", Icon = "bi-hourglass-split", Count = pendingTasks, BgColor = "rgba(255,255,255,0.8)" },
            new { Name = "Completed", Icon = "bi-check2-circle", Count = completedTasks, BgColor = "rgba(255,255,255,0.8)" },
            new { Name = "Work", Icon = "bi-briefcase", Count = workTasks, BgColor = "rgba(255,255,255,0.8)" },
            new { Name = "Personal", Icon = "bi-person", Count = personalTasks, BgColor = "rgba(255,255,255,0.8)" }
        };

            ViewData["TaskSummary"] = taskSummary;
            return View();
        }

    }
}
