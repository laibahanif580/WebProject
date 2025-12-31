using Microsoft.Data.Sqlite;
using Dapper;
using System;

public class SqliteDatabaseFixture : IDisposable
{
    public string ConnectionString { get; }
    private readonly SqliteConnection _anchorConnection;

    public SqliteDatabaseFixture()
    {
        ConnectionString = "Data Source=file:memdb1?mode=memory&cache=shared";
        _anchorConnection = new SqliteConnection(ConnectionString);
        _anchorConnection.Open();

        _anchorConnection.Execute(@"
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

    public void Dispose()
    {
        _anchorConnection.Dispose();
    }
}
