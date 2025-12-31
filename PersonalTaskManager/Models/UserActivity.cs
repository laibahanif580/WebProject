
namespace PersonalTaskManager.Models{
    
public class UserActivity
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public int TotalTasks { get; set; }
    public int Completed { get; set; }
    public int Pending { get; set; }
    public int InProgress { get; set; }
    public int Overdue { get; set; }
    public int HighPriority { get; set; }
}

}