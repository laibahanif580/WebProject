using Xunit;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using PersonalTaskManager.Models;
using PersonalTaskManager.Models.Repositories;
using PersonalTaskManager.Data;
using System.Data;
using Microsoft.Data.Sqlite;

[Collection("SqliteDatabase")]
public class TaskRepositoryIntegrationTests
{
    private readonly SqliteTestDapperContext _context;
    private readonly TaskRepository _repository;

    public TaskRepositoryIntegrationTests(SqliteDatabaseFixture fixture)
    {
        _context = new SqliteTestDapperContext(fixture.ConnectionString);
        _repository = new TaskRepository(_context);
    }
    [Fact]
    public async Task GetTasksAsync_ShouldReturnAllTasksForUser()
    {
        using var conn = _context.CreateConnection();
        using var transaction = conn.BeginTransaction();

        var task1 = new UserTask { UserId = "user2", Title = "Task 1" };
        var task2 = new UserTask { UserId = "user2", Title = "Task 2" };

        await _repository.AddTaskAsync(task1, transaction, conn);
        await _repository.AddTaskAsync(task2, transaction, conn);

        var tasks = (await _repository.GetTasksAsync("user2", transaction: transaction, connection: conn)).ToList();

        Assert.Equal(2, tasks.Count);

        transaction.Rollback();
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnCorrectTask()
    {
        using var conn = _context.CreateConnection();
        using var transaction = conn.BeginTransaction();

        var task = new UserTask { UserId = "user3", Title = "Single Task" };
        await _repository.AddTaskAsync(task, transaction, conn);

        var inserted = (await _repository.GetTasksAsync("user3", transaction: transaction, connection: conn)).First();

        var fetched = await _repository.GetTaskByIdAsync(inserted.TaskId, transaction, conn);

        Assert.NotNull(fetched);
        Assert.Equal("Single Task", fetched!.Title);

        transaction.Rollback();
    }

    [Fact]
    public async Task AddTaskAsync_ShouldInsertTask()
    {
        using var conn = _context.CreateConnection();
        using var transaction = conn.BeginTransaction();

        var task = new UserTask
        {
            UserId = "user1",
            Title = "Test Task",
            Status = "Pending"
        };

        await _repository.AddTaskAsync(task, transaction, conn);
        var tasks = await _repository.GetTasksAsync("user1", transaction: transaction, connection: conn);

        Assert.Single(tasks);

        transaction.Rollback();
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldModifyTask()
    {
        using var conn = _context.CreateConnection();
        using var transaction = conn.BeginTransaction();

        var task = new UserTask
        {
            UserId = "user1",
            Title = "Old",
            Status = "Pending"
        };

        await _repository.AddTaskAsync(task, transaction, conn);
        var inserted = (await _repository.GetTasksAsync("user1", transaction: transaction, connection: conn)).First();
        inserted.Title = "Updated";

        await _repository.UpdateTaskAsync(inserted, transaction, conn);
        var updated = await _repository.GetTaskByIdAsync(inserted.TaskId, transaction, conn);

        Assert.Equal("Updated", updated!.Title);

        transaction.Rollback();
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldRemoveTask()
    {
        using var conn = _context.CreateConnection();
        using var transaction = conn.BeginTransaction();

        var task = new UserTask
        {
            UserId = "user1",
            Title = "Delete Me"
        };

        await _repository.AddTaskAsync(task, transaction, conn);
        var inserted = (await _repository.GetTasksAsync("user1", transaction: transaction, connection: conn)).First();

        await _repository.DeleteTaskAsync(inserted.TaskId, transaction, conn);
        var result = await _repository.GetTaskByIdAsync(inserted.TaskId, transaction, conn);

        Assert.Null(result);

        transaction.Rollback();
    }
}

