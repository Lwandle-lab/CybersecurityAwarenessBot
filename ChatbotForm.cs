//=========================================================================
                                // CHATBOTFORM
//=========================================================================

// The GUI version of the Part 1 console chatbot. This form only handles what the
// user sees: the ASCII art header, the voice greeting, the chat window and the
// input box. All the conversation logic lives in ChatBot.cs.

namespace CybersecurityChatbotGUI
    {
        public partial class ChatbotForm : Form
        {
            // The ASCII art from Part 1, shown in the header of the window
            private const string AsciiLogo = @"  
   _____           _                _____           _ _   _          _
  / ____|         | |              / ____|         (_) | | |        | |
 | |    _   _  ___| |__   ___ _ __| (___   ___  ___ _| |_| |__  ___ | |__
 | |   | | | |/ __| '_ \ / _ \ '__|\___ \ / _ \/ __| | __|  _ \/   \| ___|
 | |___| |_| | (__| |_) |  __/ |   ____) |  __/ (__| | |_| |_)| ( ) | |__
  \_____\__, |\___|_.__/ \___|_|  |_____/ \___|\___|_|\__|_.__/\___/\____|
         __/ |                                             
        |___/     A W A R E N E S S   B O T                ";

        private static readonly Color UserColor = Color.FromArgb(96, 165, 250);
        private static readonly Color BotColor = Color.FromArgb(34, 197, 94);
        private static readonly Color MessageColor = Color.FromArgb(226, 232, 240);

        private readonly ChatBot _chatBot = new();
        private Font _regularFont = SystemFonts.DefaultFont;
        private Font _boldFont = SystemFonts.DefaultFont;

        public ChatbotForm()
        {
            InitializeComponent();
        }

        private void ChatbotForm_Load(object? sender, EventArgs e)
        {
            lblAscii.Text = AsciiLogo;
            CenterAsciiArt();

            _regularFont = rtbChat.Font;
            _boldFont = new Font(rtbChat.Font, FontStyle.Bold);

            // Voice greeting (plays in the background)
            string? voiceProblem = VoiceGreeting.Play();
            if (voiceProblem != null)
            {
                AppendMessage("Bot", voiceProblem, BotColor);
            }

            foreach (string line in _chatBot.GetOpeningMessages())
            {
                AppendMessage("Bot", line, BotColor);
            }

            txtInput.Focus();
        }

        private void pnlHeader_Resize(object? sender, EventArgs e)
        {
            CenterAsciiArt();
        }

        // Pressing Enter also triggers this, because btnSend is the form's AcceptButton.
        private void btnSend_Click(object? sender, EventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            string text = txtInput.Text.Trim();
            txtInput.Clear();
            txtInput.Focus();

            if (_chatBot.IsClosing)
            {
                return;
            }

            // Input validation: blank input never reaches the chatbot logic
            if (string.IsNullOrWhiteSpace(text))
            {
                string? hint = _chatBot.HandleBlankInput();
                if (hint != null)
                {
                    AppendMessage("Bot", hint, BotColor);
                }
                return;
            }

            AppendMessage("You", text, UserColor);

            try
            {
                ChatReply reply = _chatBot.Process(text);
                AppendMessage("Bot", reply.Text, BotColor);

                if (reply.ShouldExit)
                {
                    CloseAfterDelay();
                }
            }
            catch (Exception)
            {
                // The chatbot must keep working even if something unexpected happens
                AppendMessage("Bot", "Something went wrong, but I'm still here. Please try again.", BotColor);
            }
        }

        // Adds one message to the chat window: a coloured bold sender, then the text.
        private void AppendMessage(string sender, string text, Color senderColor)
        {
            AppendSegment(sender + ": ", senderColor, _boldFont);
            AppendSegment(text + Environment.NewLine + Environment.NewLine, MessageColor, _regularFont);

            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.ScrollToCaret();
        }

        private void AppendSegment(string text, Color color, Font font)
        {
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionLength = 0;
            rtbChat.SelectionColor = color;
            rtbChat.SelectionFont = font;
            rtbChat.SelectedText = text;
        }

        // Keeps the ASCII art block centred in the header (each line keeps its alignment).
        private void CenterAsciiArt()
        {
            lblAscii.Left = Math.Max(0, (pnlHeader.ClientSize.Width - lblAscii.Width) / 2);
            lblAscii.Top = 10;
        }

        private void CloseAfterDelay()
        {
            txtInput.Enabled = false;
            btnSend.Enabled = false;

            System.Windows.Forms.Timer timer = new() { Interval = 2000 };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                timer.Dispose();
                Close();
            };
            timer.Start();
        }
    }
}





