using Microsoft.AspNetCore.Http;  //IFormFile

namespace PersonalTaskManager.Models
{
    public class UserTask
    {
        public int TaskId { get; set; }
        public string? UserId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public string? Category { get; set; } // Work or Personal

        public string? Status { get; set; } // Pending, In-Progress, Completed

        public string? Priority { get; set; } // Low, Medium, High

        public DateTime? DueDate { get; set; } = DateTime.Now;
    }
}