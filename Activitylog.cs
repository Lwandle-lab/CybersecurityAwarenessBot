
namespace CybersecurityAwarenessBot.Services;

// One recorded action.
public class ActivityEntry
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Description { get; set; } = string.Empty;

    public string Display => $"{Timestamp:dd MMM HH:mm}   {Description}";
}

// Task 4: keeps a list of the actions the bot has taken, with timestamps.
public class ActivityLog
{
    private readonly List<ActivityEntry> _entries = new();

    // Raised whenever an entry is added so the GUI can refresh the log panel.
    public event Action? Changed;

    public int Count => _entries.Count;

    public void Add(string description)
    {
        _entries.Add(new ActivityEntry { Timestamp = DateTime.Now, Description = description });
        Changed?.Invoke();
    }

    // Returns the most recent entries, newest first.
    public List<ActivityEntry> Recent(int count)
    {
        return _entries.AsEnumerable().Reverse().Take(count).ToList();
    }
}