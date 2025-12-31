using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalTaskManager.Models;

namespace PersonalTaskManager.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        //setups the identity's database conn and options bcz identity stores the users,
        // ResolveEventArgs ,claims and other authen/author data
        // LINKS IDENTITY WITH EF CORE
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
