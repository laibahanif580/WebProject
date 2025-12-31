using System.Data;
using Dapper;
using PersonalTaskManager.Data;
using PersonalTaskManager.Models;
using PersonalTaskManager.Models.Repositories;

namespace PersonalTaskManager.Models.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IDapperContext _context;

        public TaskRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserTask>> GetTasksAsync(
            string userId,
            string? category = null,
            string? priority = null,
            string? searchTerm = null,
            IDbTransaction? transaction = null,
            IDbConnection? connection = null)
        {
            var conn = connection ?? _context.CreateConnection();

            string sql = "SELECT * FROM UserTasks WHERE UserId = @UserId";

            if (!string.IsNullOrEmpty(category))
                sql += " AND Category = @Category";

            if (!string.IsNullOrEmpty(priority))
                sql += " AND Priority = @Priority";
            if (!string.IsNullOrEmpty(searchTerm))
                sql += " AND (Title LIKE @SearchTerm OR Description LIKE @SearchTerm)";

            // Wrap searchTerm with % for partial match
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("UserId", userId);
            if (!string.IsNullOrEmpty(category))
                dynamicParams.Add("Category", category);
            if (!string.IsNullOrEmpty(priority))
                dynamicParams.Add("Priority", priority);
            if (!string.IsNullOrEmpty(searchTerm))
                dynamicParams.Add("SearchTerm", $"%{searchTerm}%");
                
           return await conn.QueryAsync<UserTask>(sql, dynamicParams, transaction: transaction);

        }

        public async Task<UserTask?> GetTaskByIdAsync(int taskId, IDbTransaction? transaction = null, IDbConnection? connection = null)
        {
            var conn = connection ?? _context.CreateConnection();

            return await conn.QuerySingleOrDefaultAsync<UserTask>(
                "SELECT * FROM UserTasks WHERE TaskId = @TaskId",
                new { TaskId = taskId },
                transaction: transaction);
        }

        public async Task AddTaskAsync(UserTask task, IDbTransaction? transaction = null, IDbConnection? connection = null)
        {
            var conn = connection ?? _context.CreateConnection();

            string sql = @"
                INSERT INTO UserTasks 
                (UserId, Title, Description, Category, Status, Priority, DueDate)
                VALUES 
                (@UserId, @Title, @Description, @Category, @Status, @Priority, @DueDate)";

            await conn.ExecuteAsync(sql, task, transaction: transaction);
        }

        public async Task UpdateTaskAsync(UserTask task, IDbTransaction? transaction = null, IDbConnection? connection = null)
        {
            var conn = connection ?? _context.CreateConnection();

            string sql = @"
                UPDATE UserTasks
                SET Title = @Title,
                    Description = @Description,
                    Category = @Category,
                    Status = @Status,
                    Priority = @Priority,
                    DueDate = @DueDate
                WHERE TaskId = @TaskId";

            await conn.ExecuteAsync(sql, task, transaction: transaction);
        }

        public async Task DeleteTaskAsync(int taskId, IDbTransaction? transaction = null, IDbConnection? connection = null)
        {
            var conn = connection ?? _context.CreateConnection();

            await conn.ExecuteAsync(
                "DELETE FROM UserTasks WHERE TaskId = @TaskId",
                new { TaskId = taskId },
                transaction: transaction);
        }
    }
}

