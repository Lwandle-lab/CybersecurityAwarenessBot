//==============================================================================
                                //MEMORYSTORE
//==============================================================================
namespace CybersecurityChatbotGUI
{
    internal class MemoryStore
    {
        public string Name { get; private set; } = string.Empty;

        //1. The topic the user said they are interested in (e.g. "privacy")
        public string FavouriteTopic { get; set; } = string.Empty;

        //2. The topic being discussed right now (used for follow-up questions)
        public string LastTopic { get; set; } = string.Empty;

        //3. The last mood word the user expressed (e.g. "worried")
        public string LastSentiment { get; set; } = string.Empty;

        //4. Stores the raw input as a clean, capitalised name.
        // Returns false if the name is empty or blank, so the caller can re-prompt.
        public bool TrySetName(string rawInput)
        {
            if (string.IsNullOrWhiteSpace(rawInput))
            {
                return false;
            }

            string trimmed = rawInput.Trim();
            Name = char.ToUpper(trimmed[0]) + trimmed.Substring(1);
            return true;
        }
    }
}
