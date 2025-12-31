using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;
using PersonalTaskManager.Data;
using Microsoft.Data.Sqlite;

namespace PersonalTaskManager.Data
{
    public class SqliteTestDapperContext : IDapperContext, IDisposable
    {
        private readonly SqliteConnection _connection;

        public SqliteTestDapperContext(string connectionString)
        {
            _connection = new SqliteConnection(connectionString);
            _connection.Open();

            // Create schema once
            _connection.Execute(@"
                CREATE TABLE IF NOT EXISTS UserTasks (
                    TaskId INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId TEXT NOT NULL,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    Category TEXT,
                    Status TEXT,
                    Priority TEXT,
                    DueDate TEXT
                );
            ");
        }

        public IDbConnection CreateConnection() => _connection;

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}


