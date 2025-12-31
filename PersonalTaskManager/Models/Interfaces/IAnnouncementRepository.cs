using PersonalTaskManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalTaskManager.Models.Repositories
{
public interface IAnnouncementRepository
{
    Task<IEnumerable<Announcement>> GetAllAsync();
    Task CreateAsync(string title, string message);
}
}
