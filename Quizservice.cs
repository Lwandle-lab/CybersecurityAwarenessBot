//====================================================
                    //QUIZE SERVICE
//====================================================
using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services;

// Task 2: the cybersecurity quiz. Questions are stored in a list, as the brief allows.
public class QuizService
{
    private readonly List<QuizQuestion> _bank = new()
    {
        new QuizQuestion
        {
            Text = "What should you do if you receive an email asking for your password?",
            Options = { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
            CorrectIndex = 2,
            Explanation = "Reporting phishing emails helps prevent scams."
        },
        new QuizQuestion
        {
            Text = "Using the same password on every account is safe as long as the password is long.",
            Options = { "True", "False" },
            IsTrueFalse = true,
            CorrectIndex = 1,
            Explanation = "If one site is breached, attackers try that password everywhere. Use a unique password for each account."
        },
        new QuizQuestion
        {
            Text = "Which of these is the strongest password?",
            Options = { "password123", "Summer2024", "Tr!pl3-Blue-Giraffe-Walks-99", "qwerty" },
            CorrectIndex = 2,
            Explanation = "Long passphrases that mix words, symbols and numbers are much harder to guess or crack."
        },
        new QuizQuestion
        {
            Text = "A padlock and HTTPS in the address bar mean a website is completely trustworthy.",
            Options = { "True", "False" },
            IsTrueFalse = true,
            CorrectIndex = 1,
            Explanation = "HTTPS only means the connection is encrypted. Scam sites can use it too, so still check the address."
        },
        new QuizQuestion
        {
            Text = "What is two-factor authentication (2FA)?",
            Options =
            {
                "A password that is typed twice",
                "An extra verification step on top of your password",
                "Two different antivirus programs",
                "Logging in from two devices at once"
            },
            CorrectIndex = 1,
            Explanation = "2FA asks for something more than your password, such as a code, so a stolen password alone is not enough."
        },
        new QuizQuestion
        {
            Text = "What is social engineering?",
            Options =
            {
                "Building secure social media apps",
                "Tricking people into giving up information or access",
                "A type of firewall",
                "Encrypting messages between friends"
            },
            CorrectIndex = 1,
            Explanation = "Social engineering attacks target people rather than software, using trust, urgency or fear."
        },
        new QuizQuestion
        {
            Text = "Public Wi-Fi is safe for online banking as long as the network has a password.",
            Options = { "True", "False" },
            IsTrueFalse = true,
            CorrectIndex = 1,
            Explanation = "Other users on the same network may be able to snoop. Use mobile data or a trusted VPN for sensitive tasks."
        },
        new QuizQuestion
        {
            Text = "You find a USB drive in the parking lot. What should you do?",
            Options =
            {
                "Plug it in to see who owns it",
                "Plug it into an old computer",
                "Hand it to your IT or security team without plugging it in",
                "Format it and use it"
            },
            CorrectIndex = 2,
            Explanation = "Unknown USB drives can carry malware that runs as soon as they are plugged in."
        },
        new QuizQuestion
        {
            Text = "Software updates often include security fixes.",
            Options = { "True", "False" },
            IsTrueFalse = true,
            CorrectIndex = 0,
            Explanation = "Updates patch known vulnerabilities, so installing them promptly closes doors attackers rely on."
        },
        new QuizQuestion
        {
            Text = "Which of these is a common red flag of a phishing message?",
            Options =
            {
                "A personal greeting using your correct full name",
                "Urgent threats, a mismatched sender address and unexpected links",
                "A message sent during office hours",
                "A short message with no attachments"
            },
            CorrectIndex = 1,
            Explanation = "Urgency, odd sender addresses and unexpected links are classic signs of phishing."
        },
        new QuizQuestion
        {
            Text = "What should you do before clicking a link in an unexpected message?",
            Options =
            {
                "Click it quickly before it expires",
                "Forward it to all your contacts",
                "Check where it really leads, or go to the website directly instead",
                "Reply asking if it is safe"
            },
            CorrectIndex = 2,
            Explanation = "Typing the address yourself, or checking where the link points, avoids fake login pages."
        },
        new QuizQuestion
        {
            Text = "A password manager can help you use a different strong password for every account.",
            Options = { "True", "False" },
            IsTrueFalse = true,
            CorrectIndex = 0,
            Explanation = "Password managers generate and remember unique passwords, so you only need one strong master password."
        }
    };

    private List<QuizQuestion> _session = new();
    private int _index;

    public bool IsActive { get; private set; }
    public int Score { get; private set; }
    public int Answered { get; private set; }
    public int TotalQuestions => _session.Count;
    public int CurrentNumber => _index + 1;
    public bool IsLastQuestion => _index >= _session.Count - 1;
    public QuizQuestion? Current => IsActive && _index < _session.Count ? _session[_index] : null;

    // Starts a new round with the questions in a random order.
    public void Start()
    {
        _session = _bank.OrderBy(_ => Random.Shared.Next()).ToList();
        _index = 0;
        Score = 0;
        Answered = 0;
        IsActive = true;
    }

    // Checks the chosen option for the current question and updates the score.
    public bool Submit(int optionIndex)
    {
        QuizQuestion? question = Current;
        if (question == null)
        {
            return false;
        }

        Answered++;
        bool correct = optionIndex == question.CorrectIndex;
        if (correct)
        {
            Score++;
        }

        return correct;
    }

    // Moves to the next question. Returns false (and ends the quiz) when there are no more.
    public bool MoveNext()
    {
        _index++;
        if (_index >= _session.Count)
        {
            IsActive = false;
            return false;
        }

        return true;
    }

    public string FinalMessage()
    {
        double percentage = TotalQuestions == 0 ? 0 : (double)Score / TotalQuestions;

        if (percentage >= 0.8)
        {
            return "Great job! You're a cybersecurity pro!";
        }
        if (percentage >= 0.5)
        {
            return "Good effort! A little more practice and you'll be a pro.";
        }

        return "Keep learning to stay safe online!";
    }
}