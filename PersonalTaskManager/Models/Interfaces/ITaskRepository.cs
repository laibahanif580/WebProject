using PersonalTaskManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;

namespace PersonalTaskManager.Models.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<UserTask>> GetTasksAsync(
            string userId,
            string? category = null,
            string? priority = null,
            string? searchTerm=null,
            IDbTransaction? transaction = null,
            IDbConnection? connection = null);

        Task<UserTask?> GetTaskByIdAsync(int taskId, IDbTransaction? transaction = null, IDbConnection? connection = null);

        Task AddTaskAsync(UserTask task, IDbTransaction? transaction = null, IDbConnection? connection = null);

        Task UpdateTaskAsync(UserTask task, IDbTransaction? transaction = null, IDbConnection? connection = null);

        Task DeleteTaskAsync(int taskId, IDbTransaction? transaction = null, IDbConnection? connection = null);
    }
}
