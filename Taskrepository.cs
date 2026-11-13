
using CybersecurityAwarenessBot.Models;
using MySqlConnector;

namespace CybersecurityAwarenessBot.Data;

// All database access for tasks lives here, so the rest of the app never writes SQL.
// Every query uses parameters (@name) to avoid SQL injection.
public class TaskRepository
{
    private readonly DatabaseSettings _settings;

    public TaskRepository(DatabaseSettings settings)
    {
        _settings = settings;
    }

    // Creates the database and the Tasks table if they don't exist yet.
    public async Task InitializeAsync()
    {
        string databaseName = _settings.Database.Replace("`", string.Empty);

        await using (MySqlConnection serverConnection = new(_settings.ServerConnectionString))
        {
            await serverConnection.OpenAsync();
            await using MySqlCommand createDatabase = new($"CREATE DATABASE IF NOT EXISTS `{databaseName}`;", serverConnection);
            await createDatabase.ExecuteNonQueryAsync();
        }

        await using (MySqlConnection connection = new(_settings.DatabaseConnectionString))
        {
            await connection.OpenAsync();
            const string sql = @"CREATE TABLE IF NOT EXISTS Tasks (
                                    TaskId INT AUTO_INCREMENT PRIMARY KEY,
                                    Title VARCHAR(150) NOT NULL,
                                    Description VARCHAR(500) NOT NULL,
                                    ReminderDate DATETIME NULL,
                                    IsCompleted TINYINT(1) NOT NULL DEFAULT 0,
                                    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
                                 );";
            await using MySqlCommand createTable = new(sql, connection);
            await createTable.ExecuteNonQueryAsync();
        }
    }

    public async Task<int> AddTaskAsync(string title, string description, DateTime? reminderDate)
    {
        await using MySqlConnection connection = new(_settings.DatabaseConnectionString);
        await connection.OpenAsync();

        const string sql = @"INSERT INTO Tasks (Title, Description, ReminderDate)
                             VALUES (@title, @description, @reminder);
                             SELECT LAST_INSERT_ID();";
        await using MySqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@title", title);
        command.Parameters.AddWithValue("@description", description);
        command.Parameters.AddWithValue("@reminder", (object?)reminderDate ?? DBNull.Value);

        object? result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        List<TaskItem> tasks = new();

        await using MySqlConnection connection = new(_settings.DatabaseConnectionString);
        await connection.OpenAsync();

        const string sql = @"SELECT TaskId, Title, Description, ReminderDate, IsCompleted, CreatedAt
                             FROM Tasks
                             ORDER BY IsCompleted ASC, CreatedAt DESC, TaskId DESC;";
        await using MySqlCommand command = new(sql, connection);
        await using MySqlDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            tasks.Add(ReadTask(reader));
        }

        return tasks;
    }

    public async Task<TaskItem?> GetTaskAsync(int taskId)
    {
        await using MySqlConnection connection = new(_settings.DatabaseConnectionString);
        await connection.OpenAsync();

        const string sql = @"SELECT TaskId, Title, Description, ReminderDate, IsCompleted, CreatedAt
                             FROM Tasks WHERE TaskId = @id;";
        await using MySqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@id", taskId);
        await using MySqlDataReader reader = await command.ExecuteReaderAsync();

        return await reader.ReadAsync() ? ReadTask(reader) : null;
    }

    public async Task<bool> SetReminderAsync(int taskId, DateTime reminderDate)
    {
        return await ExecuteAsync("UPDATE Tasks SET ReminderDate = @reminder WHERE TaskId = @id;",
            ("@reminder", reminderDate), ("@id", taskId));
    }

    public async Task<bool> MarkCompletedAsync(int taskId)
    {
        return await ExecuteAsync("UPDATE Tasks SET IsCompleted = 1 WHERE TaskId = @id;", ("@id", taskId));
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        return await ExecuteAsync("DELETE FROM Tasks WHERE TaskId = @id;", ("@id", taskId));
    }

    // Runs a non-query command with parameters and returns true if at least one row was affected.
    private async Task<bool> ExecuteAsync(string sql, params (string Name, object Value)[] parameters)
    {
        await using MySqlConnection connection = new(_settings.DatabaseConnectionString);
        await connection.OpenAsync();

        await using MySqlCommand command = new(sql, connection);
        foreach ((string name, object value) in parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }

        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    private static TaskItem ReadTask(MySqlDataReader reader)
    {
        int reminderOrdinal = reader.GetOrdinal("ReminderDate");

        return new TaskItem
        {
            TaskId = reader.GetInt32(reader.GetOrdinal("TaskId")),
            Title = reader.GetString(reader.GetOrdinal("Title")),
            Description = reader.GetString(reader.GetOrdinal("Description")),
            ReminderDate = reader.IsDBNull(reminderOrdinal) ? null : reader.GetDateTime(reminderOrdinal),
            IsCompleted = reader.GetBoolean(reader.GetOrdinal("IsCompleted")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
        };
    }
}