//=======================================================
                    //DATABASESETTINGS
//=======================================================
using System.Text.Json;
using MySqlConnector;
using System.IO;

namespace CybersecurityAwarenessBot.Data;

// Reads the MySQL connection details from appsettings.local.json (if present) or appsettings.json.
// Keep real passwords in appsettings.local.json, which is listed in .gitignore and never committed.
public class DatabaseSettings
{
    public string Server { get; set; } = "localhost";
    public uint Port { get; set; } = 3306;
    public string User { get; set; } = "root";
    public string Password { get; set; } = string.Empty;
    public string Database { get; set; } = "cybersecurity_chatbot";

    public static DatabaseSettings Load()
    {
        foreach (string fileName in new[] { "appsettings.local.json", "appsettings.json" })
        {
            string path = Path.Combine(AppContext.BaseDirectory, fileName);
            if (!File.Exists(path))
            {
                continue;
            }

            try
            {
                using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path));
                if (!document.RootElement.TryGetProperty("MySql", out JsonElement section))
                {
                    continue;
                }

                DatabaseSettings settings = new();
                if (section.TryGetProperty("Server", out JsonElement server))
                {
                    settings.Server = server.GetString() ?? settings.Server;
                }
                if (section.TryGetProperty("Port", out JsonElement port) && port.TryGetUInt32(out uint portValue))
                {
                    settings.Port = portValue;
                }
                if (section.TryGetProperty("User", out JsonElement user))
                {
                    settings.User = user.GetString() ?? settings.User;
                }
                if (section.TryGetProperty("Password", out JsonElement password))
                {
                    settings.Password = password.GetString() ?? string.Empty;
                }
                if (section.TryGetProperty("Database", out JsonElement database))
                {
                    settings.Database = database.GetString() ?? settings.Database;
                }

                return settings;
            }
            catch (Exception)
            {
                // A malformed settings file should not crash the app; fall back to the next file or defaults.
            }
        }

        return new DatabaseSettings();
    }

    // Connection to the MySQL server itself (used once to create the database if it doesn't exist).
    public string ServerConnectionString => new MySqlConnectionStringBuilder
    {
        Server = Server,
        Port = Port,
        UserID = User,
        Password = Password
    }.ConnectionString;

    // Connection to the chatbot database.
    public string DatabaseConnectionString => new MySqlConnectionStringBuilder
    {
        Server = Server,
        Port = Port,
        UserID = User,
        Password = Password,
        Database = Database
    }.ConnectionString;
}