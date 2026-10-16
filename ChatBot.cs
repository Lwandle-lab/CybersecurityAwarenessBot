//==============================================================================
                                    //CHATBOT
//==============================================================================

//This part of the code controls the CONVERSATION between the BOT and USER.
//It has no GUI code: the form passes in what the user typed and shows whatever ChatBot returns.
//The flow matches Part 1: welcome ->
// name capture  -> personalised greeting -> question and answer
// loop -> goodbye. Part 2 adds memory, sentiment and conversation flow.

namespace CybersecurityChatbotGUI
{
    //1. This is where the Chatbot says back to the user, and also concludes that the window
    //closes.
    internal record ChatReply(string Text, bool ShouldExit = false);

    internal class ChatBot
    {
        private enum Stage
        {
            AwaitingName,
            Chatting,
            Closing
        }

        private const string NamePrompt = "So sorry, I didn't catch that. Please try again.";

        private const string HelpText =
            "These are the topic options you can ask: passwords, phishing, safe browsing, scams, and privacy, " +
            "how I am, or what my purpose is. Tell me how you feel, or say 'tell me more'. " +
            "Type 'exit' to quit.";

        private readonly MemoryStore _memory = new();
        private readonly SentimentDetector _sentiment = new();
        private readonly KeywordResponder _responder;
        private Stage _stage = Stage.AwaitingName;

        private readonly string[] _followUpPhrases =
        {
            "another tip", "explain more", "tell me more", "more details", "give me more", "what else", "another one"
        };

        private readonly string[] _confusionPhrases =
        {
            "confused", "don't understand", "what do you mean", "not sure what", "unclear"
        };

        private readonly string[] _interestPhrases =
        {
            "interested in", "favourite topic", "favorite topic", "i care about"
        };

        public ChatBot()
        {
            _responder = new KeywordResponder(_memory);
        }

        public bool IsClosing => _stage == Stage.Closing;

        public string[] GetOpeningMessages()
        {
            return new[]
            {
                "Hello! Welcome to the Cybersecurity Awareness Bot.",
                "I'm here to help you stay safe online.",
                "Before we begin, what's your name?"
            };
        }

        //2. This part is called when the user sends blank input. Only the name step needs a reply.
        public string? HandleBlankInput()
        {
            return _stage == Stage.AwaitingName ? NamePrompt : null;
        }

        //3. Takes what the user's input and returns the chatbot's reply.
        public ChatReply Process(string input)
        {
            if (_stage == Stage.Closing)
            {
                return new ChatReply(string.Empty);
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                return new ChatReply(HandleBlankInput() ?? "I didn't catch that. Please follow the topics provided.");
            }

            if (_stage == Stage.AwaitingName)
            {
                return CaptureName(input);
            }

            if (IsExitCommand(input))
            {
                _stage = Stage.Closing;
                return new ChatReply($"Stay safe online {_memory.Name}. Goodbye!", true);
            }

            return new ChatReply(BuildReply(input));
        }

        //4.Follows the same idea as Part 1: only accepts a valid name, otherwise re-prompt.
        private ChatReply CaptureName(string input)
        {
            if (!_memory.TrySetName(StripNamePrefix(input)))
            {
                return new ChatReply(NamePrompt);
            }

            _stage = Stage.Chatting;
            return new ChatReply($"Welcome, {_memory.Name}. Ask me a question, or type 'help' to see the topics.");
        }

        private string BuildReply(string input)
        {
            string lower = input.Trim().ToLowerInvariant().Replace('\u2019', '\'');

            if (lower == "help")
            {
                return HelpText;
            }

            if (lower == "hi" || lower == "hello" || lower == "hey")
            {
                return $"Hello {_memory.Name}! Ask me about {KeywordResponder.TopicList}.";
            }

            if (_responder.TryGetSmallTalk(lower, out string smallTalk))
            {
                return smallTalk;
            }

            string? topic = _responder.FindTopic(lower);

            //5. MEMORY: "I'm interested in privacy"
            if (topic != null && ContainsAny(lower, _interestPhrases))
            {
                _memory.FavouriteTopic = topic;
                _memory.LastTopic = topic;
                return $"Great, {_memory.Name}! I'll remember that you're interested in {topic}. It's a crucial part of staying safe online.";
            }

            //6.THE SENTIMENT DETECTION
            SentimentResult? mood = _sentiment.Detect(lower);
            if (mood != null)
            {
                _memory.LastSentiment = mood.Word;
            }

            //7. CONVERSATION FLOW: follow-ups and confusion stay on the current topic
            if (topic == null && _memory.LastTopic != string.Empty)
            {
                if (ContainsAny(lower, _confusionPhrases))
                {
                    return $"No problem, let me explain {_memory.LastTopic} another way. " + _responder.GetTopicReply(_memory.LastTopic);
                }

                if (ContainsAny(lower, _followUpPhrases))
                {
                    topic = _memory.LastTopic;
                }
            }

            if (topic != null)
            {
                _memory.LastTopic = topic;
                string tip = _responder.GetTopicReply(topic);

                if (mood == null)
                {
                    return tip;
                }

                //8. With a mood, the bot shows empathy AND gives the tip straight away
                string empathy = (mood.Opener + " " + _sentiment.GetTopicEmpathy(topic)).Trim();
                return empathy + " Let me share a tip to help you stay safe. " + tip;
            }

            if (mood != null)
            {
                return mood.Opener + $" Which topic would you like help with: {KeywordResponder.TopicList}?";
            }

            return KeywordResponder.DefaultResponse; // ERROR HANDLING: unknown input (shows when the input is incorrect.
        }

        private static string StripNamePrefix(string input)
        {
            string[] prefixes = { "my name is ", "i am ", "i'm ", "im " };

            foreach (string prefix in prefixes)
            {
                if (input.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return input.Substring(prefix.Length).Trim().Trim('.', '!', ',');
                }
            }

            return input.Trim().Trim('.', '!', ',');
        }

        private static bool IsExitCommand(string input)
        {
            return string.Equals(input.Trim(), "exit", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(input.Trim(), "quit", StringComparison.OrdinalIgnoreCase);
        }

        private static bool ContainsAny(string text, string[] phrases)
        {
            foreach (string phrase in phrases)
            {
                if (text.Contains(phrase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}


