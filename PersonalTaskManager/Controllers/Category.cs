using Microsoft.AspNetCore.Mvc;
using PersonalTaskManager.Models;
using Microsoft.AspNetCore.Authorization;

namespace PersonalTaskManager.Controllers
{
    [Authorize(Policy="TaskAccess")]  
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
