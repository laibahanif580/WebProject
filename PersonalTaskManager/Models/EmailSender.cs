using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace PersonalTaskManager.Models
{
    public class EmailSender : IEmailSender
    {
        // Exact signature required by IEmailSender
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask; // placeholder, does nothing
        }
    }
}