//=================================================
                // CHATMESSAGE
//=================================================

namespace CybersecurityAwarenessBot.Models;

//The one message that is shown in the chat window.
public class ChatMessage
{
    public string Sender { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsSent { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}