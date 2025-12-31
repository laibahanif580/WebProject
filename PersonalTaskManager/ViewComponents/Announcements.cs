using Microsoft.AspNetCore.Mvc;
using PersonalTaskManager.Models.Repositories;

namespace PersonalTaskManager.ViewComponents
{
    public class Announcements : ViewComponent
    {

        private readonly IAnnouncementRepository _repo;

        public Announcements(IAnnouncementRepository repo)
        {
            _repo = repo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var announcements = await _repo.GetAllAsync();
            // Console.WriteLine("\n\n\nDEBUG: Ann Count = " + announcements.Count());
            return View(announcements); // Pass data to the ViewComponent view
        }
    }
}
