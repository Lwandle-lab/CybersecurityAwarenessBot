//=============================================================================
                            //RESPONSE HANDLER
//=============================================================================

using System.Text.RegularExpressions;
using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services;

// The text the bot replies with, plus the keyword that triggered it (used for the activity log).
public record BotResponse(string Text, string Keyword);

// Parts 1 and 2 features: keyword recognition, dynamic (random) responses, sentiment detection
// and a small memory (name and favourite topic). Replace or extend this with your own Part 2 logic.
public class ResponseHandler
{
    private record Topic(string Name, string[] Keywords, string[] Tips);

    private readonly Random _random = new();

    private readonly List<Topic> _topics = new()
    {
        new Topic("passwords", new[] { "password", "passphrase" }, new[]
        {
            "Use a long, unique passphrase for every account and turn on multi-factor authentication wherever it's offered.",
            "Never reuse passwords. If one site is breached, attackers try the same password everywhere.",
            "A password manager can create and remember strong, unique passwords for you."
        }),
        new Topic("phishing", new[] { "phish" }, new[]
        {
            "Never click links or open attachments from unexpected emails. Type the website address directly into your browser instead.",
            "Check the sender's address carefully. Phishing emails often use look-alike domains and create false urgency.",
            "If a message feels urgent or too good to be true, stop and verify it through another channel before acting."
        }),
        new Topic("safe browsing", new[] { "safe browsing", "browsing", "browser" }, new[]
        {
            "Keep your browser updated, avoid unknown downloads, and check for HTTPS before entering personal information.",
            "Be careful with browser extensions. Only install ones you trust and remove the ones you don't use."
        }),
        new Topic("privacy", new[] { "privacy", "personal information", "personal data" }, new[]
        {
            "Review your social media privacy settings regularly and think twice before sharing personal details publicly.",
            "Share the minimum personal information needed. What you post can be used to guess passwords or impersonate you."
        }),
        new Topic("two-factor authentication", new[] { "2fa", "two-factor", "two factor", "multi-factor", "mfa" }, new[]
        {
            "Two-factor authentication adds a second check, like a code from an app, so a stolen password alone isn't enough.",
            "Prefer an authenticator app over SMS codes for 2FA when you can."
        }),
        new Topic("malware", new[] { "malware", "virus", "ransomware", "antivirus" }, new[]
        {
            "Keep your system and antivirus updated, and only download software from official sources.",
            "Back up important files regularly. It is your best protection against ransomware."
        }),
        new Topic("public Wi-Fi", new[] { "wifi", "wi-fi", "public network" }, new[]
        {
            "Avoid banking or entering passwords on public Wi-Fi. Use mobile data or a trusted VPN instead.",
            "Turn off automatic Wi-Fi connection so your device doesn't join unknown networks."
        }),
        new Topic("social engineering", new[] { "social engineering", "scam", "fraud" }, new[]
        {
            "Scammers use urgency and trust. Slow down and verify any unexpected request for money or information.",
            "Never share one-time PINs or passwords with anyone, even someone claiming to be from your bank."
        })
    };

    private static readonly string[] WorriedWords = { "worried", "scared", "afraid", "nervous", "anxious", "panic", "hacked" };
    private static readonly string[] FrustratedWords = { "frustrated", "annoyed", "angry", "confused", "stuck", "fed up" };
    private static readonly string[] CuriousWords = { "curious", "excited", "eager", "love learning" };

    // Returns null when nothing in the message is recognised, so the caller can fall back gracefully.
    public BotResponse? GetResponse(string input, UserProfile user)
    {
        string lower = input.Trim().ToLowerInvariant();
        string sentiment = DetectSentiment(lower, out string sentimentLabel);

        // Memory: remember a favourite topic.
        Match interest = Regex.Match(lower,
            @"(?:i am interested in|i'm interested in|interested in|my favou?rite topic is|favou?rite topic:)\s+(.+?)[.!?]*$");
        if (interest.Success)
        {
            user.FavouriteTopic = interest.Groups[1].Value.Trim();
            return new BotResponse(
                $"{Name(user)}Great choice! I'll remember that you're interested in {user.FavouriteTopic}. Ask me anything about it.",
                "interest");
        }

        // Keyword recognition for cybersecurity topics, with a random tip each time.
        foreach (Topic topic in _topics)
        {
            string? keyword = topic.Keywords.FirstOrDefault(k => lower.Contains(k));
            if (keyword == null)
            {
                continue;
            }

            string tip = topic.Tips[_random.Next(topic.Tips.Length)];
            string text = sentiment + tip;

            if (user.FavouriteTopic.Length > 0 &&
                (user.FavouriteTopic.Contains(topic.Name, StringComparison.OrdinalIgnoreCase) ||
                 topic.Name.Contains(user.FavouriteTopic, StringComparison.OrdinalIgnoreCase)))
            {
                text += $" Since {topic.Name} is one of your favourite topics, ask me for more tips any time.";
            }

            return new BotResponse(text, keyword);
        }

        // Small talk carried over from Part 1.
        if (ContainsWord(lower, "hello") || ContainsWord(lower, "hi") || ContainsWord(lower, "hey"))
        {
            return new BotResponse($"Hello{(user.Name.Length > 0 ? " " + user.Name : string.Empty)}! How can I help you stay safe online today?", "greeting");
        }

        if (lower.Contains("how are you"))
        {
            return new BotResponse("I'm running smoothly and ready to help you stay safe online!", "how are you");
        }

        if (lower.Contains("purpose") || lower.Contains("who are you") || lower.Contains("what do you do"))
        {
            return new BotResponse(
                "My purpose is to help you build safer online habits: I give cybersecurity tips, keep track of your security tasks and quiz you on what you've learned.",
                "purpose");
        }

        if (lower.Contains("thank"))
        {
            return new BotResponse("You're welcome! Stay safe out there.", "thanks");
        }

        // Sentiment on its own: respond to the feeling and point the user forward.
        if (sentimentLabel.Length > 0)
        {
            return new BotResponse(
                sentiment + "Tell me what's on your mind, for example passwords, phishing or privacy, and I'll help.",
                sentimentLabel);
        }

        return null;
    }

    // Returns a short opening sentence matching the user's mood, or an empty string.
    private static string DetectSentiment(string lower, out string label)
    {
        label = string.Empty;

        if (WorriedWords.Any(word => lower.Contains(word)))
        {
            label = "worried";
            return "It's completely normal to feel worried about online safety. Let's take it one step at a time. ";
        }
        if (FrustratedWords.Any(word => lower.Contains(word)))
        {
            label = "frustrated";
            return "I understand this can be frustrating. I'll keep it simple. ";
        }
        if (CuriousWords.Any(word => lower.Contains(word)))
        {
            label = "curious";
            return "Great to see your curiosity! ";
        }

        return string.Empty;
    }

    private static bool ContainsWord(string text, string word)
    {
        return Regex.IsMatch(text, $@"\b{Regex.Escape(word)}\b");
    }

    private static string Name(UserProfile user)
    {
        return user.Name.Length > 0 ? user.Name + ", " : string.Empty;
    }
}