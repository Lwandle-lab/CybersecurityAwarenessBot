//========================================================
                       //TASK ITEM
//========================================================

namespace CybersecurityAwarenessBot.Models;

// The cybersecurity task is stored in the MySQL database.
public class TaskItem
{
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? ReminderDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }

    //The Read-only helpers used by the task list in the GUI.
    public string ReminderText => ReminderDate.HasValue ?
        ReminderDate.Value.ToString("dd MMM yyyy") : "None";
    public string StatusText => IsCompleted ? "Completed" : "Pending";
}