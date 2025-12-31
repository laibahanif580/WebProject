using Microsoft.AspNetCore.Authorization;  //attribute [Authorize]
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonalTaskManager.Helpers;
using PersonalTaskManager.Models;
using PersonalTaskManager.Models.Repositories;

namespace PersonalTaskManager.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskRepository _taskRepo;
        private readonly UserManager<IdentityUser> _userManager;

        public TaskController(ITaskRepository taskRepo, UserManager<IdentityUser> userManager)
        {
            _taskRepo = taskRepo;
            _userManager = userManager;
        }

        [Authorize(Policy = "TaskAccess")]
        // public async Task<IActionResult> Index(string? category, string? priority,string? searchTerm)
        // {
        //     try
        //     {
        //         var user = await _userManager.GetUserAsync(User);
        //         var tasks = await _taskRepo.GetTasksAsync(user.Id, category, priority,searchTerm);
        //         // Pass the searchTerm to the view to persist it in the search box
        //         ViewBag.SearchTerm = searchTerm;

        //         return PartialView("_TaskListPartial", tasks); // new partial view

        //         // return View(tasks);
        //     }
        //     catch (Exception ex)
        //     {
        //         FileLogger.LogError(ex, "Error loading tasks for Index view");
        //         ViewBag.Message = "Unable to load tasks. Please try again later.";
        //         return View();
        //     }
        // }
        [Authorize(Policy = "TaskAccess")]
        public async Task<IActionResult> Index(string? searchTerm, string? category, string? priority)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var tasks = await _taskRepo.GetTasksAsync(user.Id, category, priority, searchTerm);

                // Keep selected filters/search term for view
                ViewBag.SelectedCategory = category;
                ViewBag.SelectedPriority = priority;
                ViewBag.SearchTerm = searchTerm;

                // Check if the request is AJAX
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    // Return only the partial view for dynamic search
                    return PartialView("_TaskListPartial", tasks);
                }

                // Return full page on normal request
                return View(tasks);
            }
            catch (Exception ex)
            {
                FileLogger.LogError(ex, "Error loading tasks for Index view");
                ViewBag.Message = "Unable to load tasks. Please try again later.";
                return View();
            }
        }

        [Authorize(Policy = "TaskEditor")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Policy = "TaskEditor")]
        [HttpPost]
        [ValidateAntiForgeryToken]  // to protect agiant cross-site-request-forgery
        // typically for actions mofifying data and chceks if the req made from original 
        //application not third party
        public async Task<IActionResult> Create(UserTask task)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Invalid task data submitted");

                var user = await _userManager.GetUserAsync(User);
                task.UserId = user.Id;

                await _taskRepo.AddTaskAsync(task);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                FileLogger.LogError(ex, $"Error creating task '{task?.Title}'");
                ViewBag.Message = "An unexpected error occurred while creating the task.";
                return View(task);
            }
        }

        [Authorize(Policy = "TaskEditor")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var task = await _taskRepo.GetTaskByIdAsync(id);
                if (task == null) return NotFound();
                return View(task);
            }
            catch (Exception ex)
            {
                FileLogger.LogError(ex, $"Error loading task {id} for editing");
                ViewBag.Message = "Unable to load task. Please try again later.";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = "TaskEditor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserTask task)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Invalid task data submitted for edit");

                var existingTask = await _taskRepo.GetTaskByIdAsync(task.TaskId);
                if (existingTask == null) return NotFound();  //return 404 not found response

                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.Category = task.Category;
                existingTask.Status = task.Status;
                existingTask.Priority = task.Priority;
                existingTask.DueDate = task.DueDate;

                await _taskRepo.UpdateTaskAsync(existingTask);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                FileLogger.LogError(ex, $"Error editing task '{task?.Title}'");
                ViewBag.Message = "An unexpected error occurred while updating the task.";
                return View(task);
            }
        }

        [Authorize(Policy = "TaskEditor")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var task = await _taskRepo.GetTaskByIdAsync(id);
                if (task == null) return NotFound();
                return View(task);
            }
            catch (Exception ex)
            {
                FileLogger.LogError(ex, $"Error loading task details for {id}");
                ViewBag.Message = "Unable to load task details.";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = "TaskEditor")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var task = await _taskRepo.GetTaskByIdAsync(id);
                if (task == null) return NotFound();
                return View(task);
            }
            catch (Exception ex)
            {
                FileLogger.LogError(ex, $"Error loading task {id} for deletion");
                ViewBag.Message = "Unable to load task for deletion.";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = "TaskEditor")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _taskRepo.DeleteTaskAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                FileLogger.LogError(ex, $"Error deleting task {id}");
                ViewBag.Message = "Unable to delete task. Please try again later.";
                return RedirectToAction("Index");
            }
        }
    }
}
