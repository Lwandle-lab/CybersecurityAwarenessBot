//===================================================================
                        //CHATCONTROLLER
//===================================================================

using System.Text;
using System.Text.RegularExpressions;
using CybersecurityAwarenessBot.Data;
using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services;

public enum ConversationState
{
    AwaitingName,
    None,
    AwaitingTaskTitle,
    AwaitingReminderChoice,
    AwaitingReminderTime
}

// The "brain" of the chatbot. The GUI sends every message here and shows the reply.
// It keeps the conversation going (for example "Would you like a reminder?" then "Yes, in 3 days")
// and ties together the task assistant, quiz, NLP engine, activity log and the Part 1/2 amd responses.
public class ChatController
{
    public const string Greeting =
        "Hello! Welcome to the Cybersecurity Awareness Bot. I'm here to help you stay safe online. Before we start, what's your name?";

    private readonly UserProfile _user = new();
    private readonly TaskRepository _tasks;
    private readonly ActivityLog _log;
    private readonly ResponseHandler _responses = new();
    private readonly NlpEngine _nlp = new();

    private ConversationState _state = ConversationState.AwaitingName;
    private int _pendingTaskId;
    private string _pendingTaskTitle = string.Empty;
    private bool _pendingWantsReminder;
    private Timeframe? _pendingTimeframe;

    // Raised so the GUI can refresh the task list or switch to a panel.
    public event Action? TasksChanged;
    public event Action? QuizRequested;
    public event Action<string>? OpenPanel;

    public ChatController(TaskRepository tasks, ActivityLog log)
    {
        _tasks = tasks;
        _log = log;
    }

    public string UserName => _user.Name;

    // ---------- The Main entry point ----------

    public Task<string> ProcessAsync(string input)
    {
        return GuardAsync(() => ProcessInternalAsync(input));
    }

    private async Task<string> ProcessInternalAsync(string input)
    {
        string text = input.Trim();
        if (text.Length == 0)
        {
            return "I didn't catch that. Could you type something?";
        }

        if (_state == ConversationState.AwaitingName)
        {
            return HandleName(text);
        }

        // Follow-up answers to a question the bot just asked.

        string? followUp = _state switch
        {
            ConversationState.AwaitingTaskTitle => await HandleTaskTitleAsync(text),
            ConversationState.AwaitingReminderChoice => await HandleReminderChoiceAsync(text),
            ConversationState.AwaitingReminderTime => await HandleReminderTimeAsync(text),
            _ => null
        };
        if (followUp != null)
        {
            return followUp;
        }

        // NLP: work out what the user wants.

        NlpResult result = _nlp.Analyse(text);

        switch (result.Intent)
        {
            case Intent.AddTask:
                return await HandleAddTaskAsync(result);
            case Intent.SetReminder:
                return await HandleSetReminderAsync(result);
            case Intent.ShowTasks:
                return await HandleShowTasksAsync();
            case Intent.CompleteTask:
                return await CompleteTaskAsync(result.TaskId ?? 0);
            case Intent.DeleteTask:
                return await DeleteTaskAsync(result.TaskId ?? 0);
            case Intent.StartQuiz:
                OpenPanel?.Invoke("quiz");
                QuizRequested?.Invoke();
                return "Great, let's test your cybersecurity knowledge! I've started the quiz in the Quiz tab.";
            case Intent.ShowActivityLog:
                return HandleActivityLog(false);
            case Intent.ShowMore:
                return HandleActivityLog(true);
            case Intent.Help:
                return HelpText();
        }

        // Parts 1 and 2: keyword recognition, dynamic responses, sentiment.

        BotResponse? response = _responses.GetResponse(text, _user);
        if (response != null)
        {
            _log.Add($"Recognised '{response.Keyword}' in your message and responded");
            return response.Text;
        }

        return "I didn't quite understand that. Could you rephrase? You can ask about passwords, phishing or safe browsing, " +
               "or say 'add task', 'start quiz' or 'show activity log'.";
    }

    // ------------------------------------- Name (Part 1) ----------------------------------

    private string HandleName(string text)
    {
        Match match = Regex.Match(text, @"^(?:my name is|i am|i'm|im|call me|it's|its)\s+(.+)$", RegexOptions.IgnoreCase);
        string name = (match.Success ? match.Groups[1].Value : text).Trim().TrimEnd('.', '!', ',');

        if (!_user.TrySetName(name))
        {
            return "I didn't quite catch a name there. What should I call you?";
        }

        _state = ConversationState.None;
        _log.Add($"Chat started with {_user.Name}");

        return $"Nice to meet you, {_user.Name}! You can ask me about passwords, phishing or safe browsing, " +
               "add cybersecurity tasks and reminders, take a quiz, or view my activity log. Type 'help' to see everything I can do.";
    }

    // ----------------------------------- Task assistant (Task 1) ------------------------

    private async Task<string> HandleAddTaskAsync(NlpResult result)
    {
        if (result.TaskTitle.Length == 0)
        {
            _state = ConversationState.AwaitingTaskTitle;
            _pendingWantsReminder = false;
            _pendingTimeframe = result.Timeframe;
            return "Sure! What should the task be? For example: 'Enable two-factor authentication'.";
        }

        return await CreateTaskAsync(result.TaskTitle, result.Timeframe, wantsReminder: false);
    }

    private async Task<string> HandleSetReminderAsync(NlpResult result)
    {
        if (result.TaskTitle.Length == 0)
        {
            _state = ConversationState.AwaitingTaskTitle;
            _pendingWantsReminder = true;
            _pendingTimeframe = result.Timeframe;
            return "Happy to set a reminder. What would you like me to remind you about?";
        }

        return await CreateTaskAsync(result.TaskTitle, result.Timeframe, wantsReminder: true);
    }

    // Creates the task in the database, then either confirms the reminder or asks about one.
    private async Task<string> CreateTaskAsync(string title, Timeframe? timeframe, bool wantsReminder)
    {
        string description = TaskDescriptions.For(title);
        DateTime? reminder = timeframe?.Date;

        int id = await _tasks.AddTaskAsync(title, description, reminder);
        _log.Add($"Task added: '{title}'");
        if (reminder.HasValue)
        {
            _log.Add($"Reminder set: '{title}' for {reminder.Value:dd MMM yyyy}");
        }
        TasksChanged?.Invoke();

        if (timeframe != null)
        {
            return wantsReminder
                ? $"Reminder set for '{title}' {Describe(timeframe)}."
                : $"Task added with the description \"{description}\" I'll remind you {Describe(timeframe)}.";
        }

        _pendingTaskId = id;
        _pendingTaskTitle = title;

        if (wantsReminder)
        {
            _state = ConversationState.AwaitingReminderTime;
            return $"Task added: '{title}'. When should I remind you? For example 'tomorrow' or 'in 3 days'.";
        }

        _state = ConversationState.AwaitingReminderChoice;
        return $"Task added with the description \"{description}\" Would you like a reminder?";
    }

    // The user answered "What should the task be?"
    private async Task<string?> HandleTaskTitleAsync(string text)
    {
        if (Regex.IsMatch(text, @"^(?:cancel|never ?mind|forget it)\b", RegexOptions.IgnoreCase))
        {
            ResetPending();
            return "No problem, I've cancelled that.";
        }

        // If the user changed topic (for example "show tasks"), drop this question and handle the new request.

        if (_nlp.Analyse(text).Intent != Intent.Unknown)
        {
            ResetPending();
            return null;
        }

        Timeframe? inText = _nlp.ParseTimeframe(text);
        Timeframe? timeframe = inText ?? _pendingTimeframe;
        string title = NlpEngine.CleanTitle(inText != null ? _nlp.RemovePhrase(text, inText.Phrase) : text);

        if (title.Length == 0)
        {
            return "I need a short title for the task. What should it be?";
        }

        bool wantsReminder = _pendingWantsReminder;
        ResetPending();
        return await CreateTaskAsync(title, timeframe, wantsReminder);
    }

    // The user answered "Would you like a reminder?"

    private async Task<string?> HandleReminderChoiceAsync(string text)
    {
        if (_nlp.IsNo(text))
        {
            ResetPending();
            return "No problem, I've saved the task without a reminder.";
        }

        Timeframe? timeframe = _nlp.ParseTimeframe(text);
        if (timeframe != null)
        {
            return await SetReminderForPendingTaskAsync(timeframe);
        }

        if (_nlp.IsYes(text))
        {
            _state = ConversationState.AwaitingReminderTime;
            return "When should I remind you? For example 'tomorrow' or 'in 3 days'.";
        }

        // Something else entirely: drop the question and handle the message normally.

        ResetPending();
        return null;
    }

    // The user answered "When should I remind you?"

    private async Task<string?> HandleReminderTimeAsync(string text)
    {
        if (_nlp.IsNo(text) || Regex.IsMatch(text, @"^(?:cancel|never ?mind|forget it)\b", RegexOptions.IgnoreCase))
        {
            ResetPending();
            return "Okay, no reminder. The task is saved.";
        }

        Timeframe? timeframe = _nlp.ParseTimeframe(text);
        if (timeframe != null)
        {
            return await SetReminderForPendingTaskAsync(timeframe);
        }

        if (_nlp.Analyse(text).Intent != Intent.Unknown)
        {
            ResetPending();
            return null;
        }

        return "I didn't catch a timeframe. Try 'tomorrow' or 'in 3 days', or say 'no' to skip the reminder.";
    }

    private async Task<string> SetReminderForPendingTaskAsync(Timeframe timeframe)
    {
        string title = _pendingTaskTitle;
        await _tasks.SetReminderAsync(_pendingTaskId, timeframe.Date);

        _log.Add($"Reminder set: '{title}' for {timeframe.Date:dd MMM yyyy}");
        TasksChanged?.Invoke();
        ResetPending();

        return $"Got it! I'll remind you {Describe(timeframe)}.";
    }

    private async Task<string> HandleShowTasksAsync()
    {
        List<TaskItem> tasks = await _tasks.GetAllTasksAsync();
        OpenPanel?.Invoke("tasks");
        TasksChanged?.Invoke();

        if (tasks.Count == 0)
        {
            return "You don't have any tasks yet. Try 'Add task - Review privacy settings'.";
        }

        StringBuilder builder = new("Here are your tasks:");
        foreach (TaskItem task in tasks)
        {
            builder.Append($"\n#{task.TaskId} [{task.StatusText}] {task.Title}");
            if (task.ReminderDate.HasValue)
            {
                builder.Append($" (reminder: {task.ReminderText})");
            }
        }

        return builder.ToString();
    }

    // ----------------------------- Called by the GUI buttons as well as the chat -----------------------

    public Task<string> AddTaskFromFormAsync(string title, string description, DateTime? reminder)
    {
        return GuardAsync(async () =>
        {
            title = title.Trim();
            if (title.Length == 0)
            {
                return "Please enter a task title first.";
            }

            description = string.IsNullOrWhiteSpace(description) ? TaskDescriptions.For(title) : description.Trim();
            await _tasks.AddTaskAsync(title, description, reminder);

            _log.Add($"Task added: '{title}'");
            if (reminder.HasValue)
            {
                _log.Add($"Reminder set: '{title}' for {reminder.Value:dd MMM yyyy}");
            }
            TasksChanged?.Invoke();

            return reminder.HasValue
                ? $"Task added: '{title}' with a reminder for {reminder.Value:dd MMM yyyy}."
                : $"Task added: '{title}'.";
        });
    }

    public Task<string> CompleteTaskAsync(int taskId)
    {
        return GuardAsync(async () =>
        {
            TaskItem? task = await _tasks.GetTaskAsync(taskId);
            if (task == null)
            {
                return $"I couldn't find task #{taskId}.";
            }
            if (task.IsCompleted)
            {
                return $"Task #{taskId} ('{task.Title}') is already completed.";
            }

            await _tasks.MarkCompletedAsync(taskId);
            _log.Add($"Task completed: '{task.Title}'");
            TasksChanged?.Invoke();

            return $"Nice work! I've marked '{task.Title}' as completed.";
        });
    }

    public Task<string> DeleteTaskAsync(int taskId)
    {
        return GuardAsync(async () =>
        {
            TaskItem? task = await _tasks.GetTaskAsync(taskId);
            if (task == null)
            {
                return $"I couldn't find task #{taskId}.";
            }

            await _tasks.DeleteTaskAsync(taskId);
            _log.Add($"Task deleted: '{task.Title}'");
            TasksChanged?.Invoke();

            return $"I've deleted the task '{task.Title}'.";
        });
    }

    // -------------------------------- Activity log (Task 4) -----------------------------

    private string HandleActivityLog(bool showAll)
    {
        OpenPanel?.Invoke("log");

        if (_log.Count == 0)
        {
            return "I haven't done anything yet. Add a task, set a reminder or take the quiz and I'll log it here.";
        }

        int limit = showAll ? int.MaxValue : 5;
        List<ActivityEntry> entries = _log.Recent(limit);

        StringBuilder builder = new("Here's a summary of recent actions:");
        for (int i = 0; i < entries.Count; i++)
        {
            builder.Append($"\n{i + 1}. {entries[i].Description} ({entries[i].Timestamp:HH:mm})");
        }

        if (!showAll && _log.Count > entries.Count)
        {
            builder.Append($"\n(Showing the last {entries.Count} of {_log.Count}. Type 'show more' to see everything.)");
        }

        return builder.ToString();
    }

    // ----------------------------------------- Helpers ----------------------------------

    private static string HelpText()
    {
        return "Here's what I can do:\n" +
               "- Give tips on passwords, phishing, safe browsing, privacy, 2FA, malware and Wi-Fi\n" +
               "- Add tasks: 'Add task - Review privacy settings'\n" +
               "- Set reminders: 'Remind me to update my password tomorrow'\n" +
               "- Show or manage tasks: 'show tasks', 'complete task 2', 'delete task 3'\n" +
               "- Start the quiz: 'start quiz'\n" +
               "- Show my activity: 'show activity log' or 'what have you done for me?'";
    }

    // Turns a time phrase into something that reads naturally after "I'll remind you".
    private static string Describe(Timeframe timeframe)
    {
        string phrase = timeframe.Phrase.Trim().ToLowerInvariant();
        string[] natural = { "in ", "on ", "next ", "this ", "tomorrow", "today", "tonight" };

        return natural.Any(prefix => phrase.StartsWith(prefix)) ? phrase : "on " + phrase;
    }

    private void ResetPending()
    {
        _state = ConversationState.None;
        _pendingTaskId = 0;
        _pendingTaskTitle = string.Empty;
        _pendingWantsReminder = false;
        _pendingTimeframe = null;
    }

    // Runs an action and turns any database problem into a friendly message instead of a crash.
    private static async Task<string> GuardAsync(Func<Task<string>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            return "Something went wrong while working with the MySQL database. Please check that MySQL is running " +
                   $"and that appsettings.json has the correct details. ({ex.Message})";
        }
    }
}