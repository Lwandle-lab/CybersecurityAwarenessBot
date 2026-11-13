//=======================================================================================
                                    //NPLENGINE
//=======================================================================================
using System.Text.RegularExpressions;

namespace CybersecurityAwarenessBot.Services;

public enum Intent
{
    Unknown,
    AddTask,
    SetReminder,
    ShowTasks,
    CompleteTask,
    DeleteTask,
    StartQuiz,
    ShowActivityLog,
    ShowMore,
    Help
}

// A time phrase found in the user's message, such as "in 3 days", and the date it works out to.
public record Timeframe(DateTime Date, string Phrase);

public class NlpResult
{
    public Intent Intent { get; set; } = Intent.Unknown;
    public string TaskTitle { get; set; } = string.Empty;
    public Timeframe? Timeframe { get; set; }
    public int? TaskId { get; set; }
}

// Task 3: a simple NLP simulation. It uses keyword detection, regular expressions and basic
// string manipulation to work out what the user wants, even when they word it differently.
public class NlpEngine
{
    private const RegexOptions Ignore = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;

    private static readonly Dictionary<string, int> NumberWords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["a"] = 1,
        ["an"] = 1,
        ["one"] = 1,
        ["two"] = 2,
        ["three"] = 3,
        ["four"] = 4,
        ["five"] = 5,
        ["six"] = 6,
        ["seven"] = 7,
        ["eight"] = 8,
        ["nine"] = 9,
        ["ten"] = 10
    };

    private static readonly Regex RelativeTime = new(
        @"\bin\s+(\d+|an?|one|two|three|four|five|six|seven|eight|nine|ten)\s+(minutes?|hours?|days?|weeks?|months?)\b", Ignore);
    private static readonly Regex FromNow = new(
        @"\b(\d+)\s+(days?|weeks?|months?)\s+(?:from now|later)\b", Ignore);
    private static readonly Regex NamedTime = new(
        @"\b(tomorrow|today|tonight|next week|next month)\b", Ignore);
    private static readonly Regex Weekday = new(
        @"\b(?:(?:on|next|this)\s+)?(monday|tuesday|wednesday|thursday|friday|saturday|sunday)\b", Ignore);
   

    // -------------------------- Intent detection -------------------------

    public NlpResult Analyse(string input)
    {
        NlpResult result = new();
        string text = input.Trim();
        string lower = text.ToLowerInvariant();

        result.Timeframe = ParseTimeframe(text);

        // "complete task 3" / "delete task 2"

        Match taskNumber = Regex.Match(lower, @"\btask\s*#?(\d+)\b");
        if (taskNumber.Success)
        {
            result.TaskId = int.Parse(taskNumber.Groups[1].Value);

            if (Regex.IsMatch(lower, @"\b(delete|remove|cancel)\b"))
            {
                result.Intent = Intent.DeleteTask;
                return result;
            }
            if (Regex.IsMatch(lower, @"\b(complete|completed|finish|finished|done|mark)\b"))
            {
                result.Intent = Intent.CompleteTask;
                return result;
            }
        }

        if (Regex.IsMatch(lower, @"\bactivity log\b|what have you done|recent actions|show (?:me )?(?:the )?log\b"))
        {
            result.Intent = Intent.ShowActivityLog;
            return result;
        }

        if (lower is "show more" or "more" or "show all" or "show full history" or "full history")
        {
            result.Intent = Intent.ShowMore;
            return result;
        }

        if (Regex.IsMatch(lower, @"\b(?:add|create|new|make)\b[^.]*\btasks?\b"))
        {
            result.Intent = Intent.AddTask;
            result.TaskTitle = ExtractTaskTitle(text, result.Timeframe);
            return result;
        }

        if (Regex.IsMatch(lower, @"\bremind me\b|\b(?:set|add|create|make)\s+(?:a\s+|an\s+)?reminder\b|\breminder to\b"))
        {
            result.Intent = Intent.SetReminder;
            result.TaskTitle = ExtractReminderTitle(text, result.Timeframe);
            return result;
        }

        if (Regex.IsMatch(lower, @"\b(?:show|view|list|see|display|open)\b[^.]*\b(?:tasks?|to-?do)\b|\bmy tasks\b|\bwhat tasks\b"))
        {
            result.Intent = Intent.ShowTasks;
            return result;
        }

        if (Regex.IsMatch(lower, @"\bquiz\b|\bmini-?game\b|\bplay a game\b|\btest my knowledge\b"))
        {
            result.Intent = Intent.StartQuiz;
            return result;
        }

        if (Regex.IsMatch(lower, @"^help\b|\bwhat can you do\b|\bcommands\b|\bshow help\b"))
        {
            result.Intent = Intent.Help;
            return result;
        }

        return result;
    }

    public bool IsYes(string input)
    {
        return Regex.IsMatch(input.Trim(), @"^(?:yes|yeah|yep|yup|sure|ok|okay|please|y)\b", Ignore);
    }

    public bool IsNo(string input)
    {
        return Regex.IsMatch(input.Trim(), @"^(?:no|nope|nah|not now|skip|n)\b", Ignore);
    }

    // ----------------------------------- Time phrases ----------------------------

    // Looks for phrases like "in 3 days", "tomorrow", "next week" or "on Friday".
    public Timeframe? ParseTimeframe(string text)
    {
        Match match = RelativeTime.Match(text);
        if (match.Success)
        {
            int amount = ParseNumber(match.Groups[1].Value);
            return new Timeframe(AddUnits(amount, match.Groups[2].Value), match.Value);
        }

        match = FromNow.Match(text);
        if (match.Success)
        {
            int amount = ParseNumber(match.Groups[1].Value);
            return new Timeframe(AddUnits(amount, match.Groups[2].Value), match.Value);
        }

        match = NamedTime.Match(text);
        if (match.Success)
        {
            DateTime today = DateTime.Today;
            DateTime date = match.Value.ToLowerInvariant() switch
            {
                "tomorrow" => today.AddDays(1),
                "next week" => today.AddDays(7),
                "next month" => today.AddMonths(1),
                _ => today // "today" and "tonight"
            };
            return new Timeframe(date, match.Value);
        }

        match = Weekday.Match(text);
        if (match.Success)
        {
            DayOfWeek target = Enum.Parse<DayOfWeek>(match.Groups[1].Value, true);
            int daysAhead = ((int)target - (int)DateTime.Today.DayOfWeek + 7) % 7;
            if (daysAhead == 0)
            {
                daysAhead = 7;
            }
            return new Timeframe(DateTime.Today.AddDays(daysAhead), match.Value);
        }

        return null;
    }

    private static int ParseNumber(string value)
    {
        if (int.TryParse(value, out int number))
        {
            return number;
        }

        return NumberWords.TryGetValue(value, out int word) ? word : 1;
    }

    private static DateTime AddUnits(int amount, string unit)
    {
        unit = unit.ToLowerInvariant();

        if (unit.StartsWith("minute")) return DateTime.Now.AddMinutes(amount);
        if (unit.StartsWith("hour")) return DateTime.Now.AddHours(amount);
        if (unit.StartsWith("day")) return DateTime.Today.AddDays(amount);
        if (unit.StartsWith("week")) return DateTime.Today.AddDays(7 * amount);

        return DateTime.Today.AddMonths(amount);
    }

    // ------------------------- Title extraction (string manipulation) -------------------------

    // "Add task - Review privacy settings"  ->  "Review privacy settings"
    // "Add a task to enable 2FA"            ->  "Enable 2FA"
    private string ExtractTaskTitle(string text, Timeframe? timeframe)
    {
        string working = RemovePhrase(text, timeframe?.Phrase);

        Match match = Regex.Match(working, @"\btasks?\b[\s:\-\u2013\u2014]*(?:to\s+|for\s+)?(.*)$",
            Ignore | RegexOptions.Singleline);

        return CleanTitle(match.Success ? match.Groups[1].Value : string.Empty);
    }

    // "Remind me to update my password tomorrow"  ->  "Update my password"
    private string ExtractReminderTitle(string text, Timeframe? timeframe)
    {
        string working = RemovePhrase(text, timeframe?.Phrase);

        Match match = Regex.Match(working, @"\bremind me\b\s*(?:to\s+|about\s+|of\s+)?(.*)$",
            Ignore | RegexOptions.Singleline);

        if (!match.Success)
        {
            match = Regex.Match(working, @"\breminder\b\s*(?:to\s+|for\s+|about\s+)?(.*)$",
                Ignore | RegexOptions.Singleline);
        }

        return CleanTitle(match.Success ? match.Groups[1].Value : string.Empty);
    }

    public string RemovePhrase(string text, string? phrase)
    {
        if (string.IsNullOrEmpty(phrase))
        {
            return text;
        }

        int index = text.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
        return index < 0 ? text : text.Remove(index, phrase.Length);
    }

    // Tidies a title: drops a trailing "and remind me ...", punctuation and extra spaces, and capitalises it.
    public static string CleanTitle(string raw)
    {
        string title = Regex.Replace(raw, @"\s*\b(?:and\s+|then\s+)?(?:please\s+)?remind me\b.*$", string.Empty, Ignore);
        title = Regex.Replace(title, @"\bplease\b\s*$", string.Empty, Ignore);
        title = Regex.Replace(title, @"\s+", " ").Trim(' ', '.', ',', '!', '?', ':', ';', '-');

        if (title.Length == 0)
        {
            return string.Empty;
        }

        return char.ToUpper(title[0]) + title.Substring(1);
    }
}
