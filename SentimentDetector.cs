//==============================================================================
                            //SENTIMENTDETECTOR
//==============================================================================

namespace CybersecurityChatbotGUI
{
    internal class SentimentResult
    {
        public SentimentResult(string word, string opener)
        {
            Word = word;
            Opener = opener;
        }

        public string Word { get; }
        public string Opener { get; }
    }
    //This is the simple sentiment part of the code:
    //It where the chatbot provides encouraging ot support if the user feels overwhelmed or unsure.
    internal class SentimentDetector
    {
        private readonly Dictionary<string, string> _openers = new()
        {
            ["worried"] = "It's completely understandable to feel that way.",
            ["worry"] = "It's completely understandable to feel that way.",
            ["anxious"] = "It's completely understandable to feel that way.",
            ["scared"] = "That's a normal reaction, and you are already taking a good step by asking.",
            ["overwhelmed"] = "That's okay. You don't have to learn everything at once, so we'll take it one step at a time.",
            ["curious"] = "Great question! Curiosity is one of your best defences online.",
            ["frustrated"] = "I'm sorry this feels frustrating. Let's work through it together."
        };

        private readonly Dictionary<string, string> _topicEmpathy = new()
        {
            ["scam"] = "Scammers can be very convincing.",
            ["phishing"] = "Phishing emails can look very convincing."
        };

        public SentimentResult? Detect(string lowerCaseInput)
        {
            foreach (KeyValuePair<string, string> pair in _openers)
            {
                if (lowerCaseInput.Contains(pair.Key))
                {
                    return new SentimentResult(pair.Key, pair.Value);
                }
            }

            return null;
        }

        public string GetTopicEmpathy(string topic)
        {
            return _topicEmpathy.TryGetValue(topic, out string? line) ? line : string.Empty;
        }
    }
}

