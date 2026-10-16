//=========================================================================
                        // KEYWORDRESPONDER
//=========================================================================

//This part of the code: Recognises cybersecurity keywords in the user's input and replies with a
// random tip from a list for that topic. Each topic is handled through a
// DELEGATE, and all data is stored in generic collections.

namespace CybersecurityChatbotGUI
{
    //1. DELEGATE: every topic has a handler that builds a reply using the user's memory.
    internal delegate string TopicHandler(MemoryStore memory);

    internal class KeywordResponder
    {
        public const string TopicList = "passwords, phishing, safe browsing, scams or privacy";

        public const string DefaultResponse =
            "I didn't quite understand that, could you please rephrase?" +
            "You can ask me about " + TopicList + ".";

        private readonly MemoryStore _memory;
        private readonly Random _random = new();
        private readonly Dictionary<string, int> _lastTipIndex = new();

        // Part 1 questions, kept in lower case so they match the lower-cased input
        private readonly Dictionary<string, string> _smallTalk = new()
        {
            ["how are you"] = "I'm running smoothly and ready to help you stay safe online!",
            ["what's your purpose"] = "My purpose is to help you learn about more cybersecurity habits.",
            ["what is your purpose"] = "My purpose is to help you learn more cybersecurity habits.",
            ["your purpose"] = "My purpose is to help you learn more cybersecurity habits.",
            ["what can i ask"] = "You can ask me about password safety, phishing, safe browsing, scams or privacy."
        };

        //2. GENERIC COLLECTION: topic, keywords that trigger it (KEYWORD RECOGNITION)
        private readonly Dictionary<string, string[]> _keywords = new()
        {
            ["password"] = new[] { "password", "passphrase", "login" },
            ["phishing"] = new[] { "phishing", "phish", "suspicious email", "fake email" },
            ["safe browsing"] = new[] { "safe browsing", "browsing", "browser", "https" },
            ["scam"] = new[] { "scam", "fraud", "swindle" },
            ["privacy"] = new[] { "privacy", "private", "personal information", "personal data" }
        };

        //3. GENERIC COLLECTION: topic,  several tips; one is picked at random (RANDOM RESPONSES)
        private readonly Dictionary<string, List<string>> _tips = new()
        {
            ["password"] = new List<string>
            {
                "Use a long, unique passphrase for every account, and turn on multi-factor authentication wherever it's offered.",
                "Make sure to use strong, unique passwords for each account. Avoid using personal details in your passwords.",
                "A passphrase of four or more random words is easier to remember and harder to crack than a short complex password.",
                "Consider a password manager. It creates and stores strong passwords so you do not have to remember them."
            },
            ["phishing"] = new List<string>
            {
                "Never click links or open attachments from unexpected emails - rather type the website address directly into your browser.",
                "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
                "Hover over a link before clicking. If the address looks odd or does not match the sender, do not open it.",
                "Urgent threats like 'your account will be closed today' are a classic phishing trick. Slow down and verify."
            },
            ["safe browsing"] = new List<string>
            {
                "Keep your browser updated, avoid unknown downloads, and check for HTTPS before entering personal information on a site.",
                "Look for the padlock and 'https' in the address bar before logging in or paying on a website.",
                "Be careful with browser extensions. Only install ones you trust, and remove those you no longer use.",
                "Avoid doing banking or shopping on public Wi-Fi unless you are using a VPN."
            },
            ["scam"] = new List<string>
            {
                "If an offer sounds too good to be true, it probably is. Never pay upfront fees to claim a prize.",
                "Never share one-time PINs or passwords with anyone, even if they say they are from your bank.",
                "If someone pressures you to act quickly, stop and contact the company using its official number.",
                "Check online sellers for reviews and secure payment options before you buy."
            },
            ["privacy"] = new List<string>
            {
                "Review the privacy settings on your social media accounts and limit who can see your posts.",
                "Only share personal information with websites that use HTTPS and that you trust.",
                "Check which apps have access to your camera, microphone and location, and remove what is not needed.",
                "Think before you post. Details like your address, school or workplace can be used against you."
            }
        };

        // Added to a reply when the topic matches what the user said they are interested in
        private readonly Dictionary<string, string> _personalActions = new()
        {
            ["password"] = "set up a password manager and turn on multi-factor authentication",
            ["phishing"] = "before clicking on unkown links, first double check througly",
            ["safe browsing"] = "check for the HTTPS before entering your personal information",
            ["scam"] = "verify unexpected requests directly with the company",
            ["privacy"] = "review the security settings on your accounts"
        };

        // DELEGATES: topic -> handler, filled in the constructor
        private readonly Dictionary<string, TopicHandler> _handlers = new();

        public KeywordResponder(MemoryStore memory)
        {
            _memory = memory;

            foreach (string topic in _keywords.Keys)
            {
                string captured = topic; // keep a copy for the lambda
                _handlers[captured] = m => BuildTopicReply(captured, m);
            }
        }

        // KEYWORD RECOGNITION: returns the topic found in the text, or null.
        public string? FindTopic(string lowerCaseInput)
        {
            foreach (KeyValuePair<string, string[]> pair in _keywords)
            {
                foreach (string keyword in pair.Value)
                {
                    if (lowerCaseInput.Contains(keyword))
                    {
                        return pair.Key;
                    }
                }
            }

            return null;
        }

        // Part 1 questions such as "how are you" and "what is your purpose"
        public bool TryGetSmallTalk(string lowerCaseInput, out string reply)
        {
            foreach (KeyValuePair<string, string> entry in _smallTalk)
            {
                if (lowerCaseInput.Contains(entry.Key))
                {
                    reply = entry.Value;
                    return true;
                }
            }

            reply = string.Empty;
            return false;
        }

        // Invokes the delegate for the topic and returns its reply.
        public string GetTopicReply(string topic)
        {
            return _handlers[topic](_memory);
        }

        private string BuildTopicReply(string topic, MemoryStore memory)
        {
            string reply = NextTip(topic);

            if (memory.FavouriteTopic == topic && _personalActions.TryGetValue(topic, out string? action))
            {
                reply += $" As someone interested in {topic}, you might want to {action}.";
            }

            return reply;
        }

        // RANDOM RESPONSES: pick a tip at random, but never the same one twice in a row
        private string NextTip(string topic)
        {
            List<string> tips = _tips[topic];
            int index = _random.Next(tips.Count);

            if (tips.Count > 1 && _lastTipIndex.TryGetValue(topic, out int last))
            {
                while (index == last)
                {
                    index = _random.Next(tips.Count);
                }
            }

            _lastTipIndex[topic] = index;
            return tips[index];
        }
    }
}
