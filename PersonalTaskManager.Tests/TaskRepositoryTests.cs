using Xunit;
using Moq;
using Dapper;
using System.Data;
using PersonalTaskManager.Data;
using PersonalTaskManager.Models;
using PersonalTaskManager.Models.Repositories;
using Moq.Dapper;

namespace PersonalTaskManager.Tests
{
    public class TaskRepositoryTests
    {
        private readonly Mock<IDapperContext> _contextMock;
        private readonly Mock<IDbConnection> _connectionMock;
        private readonly TaskRepository _repository;

        public TaskRepositoryTests()
        {
            _contextMock = new Mock<IDapperContext>();
            _connectionMock = new Mock<IDbConnection>();

            _contextMock
                .Setup(c => c.CreateConnection())
                .Returns(_connectionMock.Object);

            _repository = new TaskRepository(_contextMock.Object);
        }

        #region GetTaskByIdAsync

        [Fact]
        public async Task GetTaskByIdAsync_Returns_Task_When_Task_Exists()
        {
            // Arrange
            var task = new UserTask { TaskId = 10, Title = "Unit Test Task" };

            _connectionMock
                .SetupDapperAsync(c =>
                    c.QuerySingleOrDefaultAsync<UserTask>(
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        null,
                        null,
                        null))
                .ReturnsAsync(task);

            // Act
            var result = await _repository.GetTaskByIdAsync(10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result!.TaskId);
            Assert.Equal("Unit Test Task", result.Title);
        }

        [Fact]
        public async Task GetTaskByIdAsync_Returns_Null_When_Task_Not_Found()
        {
            _connectionMock
                .SetupDapperAsync(c =>
                    c.QuerySingleOrDefaultAsync<UserTask>(
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        null,
                        null,
                        null))
                .ReturnsAsync((UserTask?)null);

            var result = await _repository.GetTaskByIdAsync(99);

            Assert.Null(result);
        }

        #endregion

        #region AddTaskAsync

        [Fact]
        public async Task AddTaskAsync_Executes_Insert()
        {
            var task = new UserTask { Title = "New Task", UserId = "user1" };

            _connectionMock
                .SetupDapperAsync(c =>
                    c.ExecuteAsync(
                        It.IsAny<string>(),
                        task,
                        null,
                        null,
                        null))
                .ReturnsAsync(1);

            await _repository.AddTaskAsync(task);

            _connectionMock.Verify();
        }

        #endregion

        #region UpdateTaskAsync

        [Fact]
        public async Task UpdateTaskAsync_Executes_Update_Command()
        {
            var task = new UserTask
            {
                TaskId = 5,
                Title = "Updated Task",
                UserId = "user1"
            };

            _connectionMock
                .SetupDapperAsync(c =>
                    c.ExecuteAsync(
                        It.IsAny<string>(),
                        task,
                        null,
                        null,
                        null))
                .ReturnsAsync(1);

            await _repository.UpdateTaskAsync(task);

            _connectionMock.Verify();
        }

        #endregion

        #region DeleteTaskAsync

        [Fact]
        public async Task DeleteTaskAsync_Executes_Delete_Command()
        {
            _connectionMock
                .SetupDapperAsync(c =>
                    c.ExecuteAsync(
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        null,
                        null,
                        null))
                .ReturnsAsync(1);

            await _repository.DeleteTaskAsync(5);

            _connectionMock.Verify();
        }

        #endregion

        #region Optional: GetTasksAsync

        [Fact]
        public async Task GetTasksAsync_Returns_List_Of_Tasks()
        {
            var tasks = new List<UserTask>
            {
                new() { TaskId = 1, Title = "Task 1", UserId = "user1" },
                new() { TaskId = 2, Title = "Task 2", UserId = "user1" }
            };

            _connectionMock
                .SetupDapperAsync(c =>
                    c.QueryAsync<UserTask>(
                        It.IsAny<string>(),
                        It.IsAny<object>(),
                        null,
                        null,
                        null))
                .ReturnsAsync(tasks);

            var result = await _repository.GetTasksAsync("user1");

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        #endregion
    }
}
