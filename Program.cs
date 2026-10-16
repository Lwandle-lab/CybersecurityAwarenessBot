//==============================================
                    // PROGRAM
//==============================================

//This is the "front door" of the application
//It's main job is to create my chatbot object and to start the program.
//Nothing much is done here

namespace CybersecurityChatbotGUI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            System.Windows.Forms.Application.Run(new ChatbotForm());
        }
    }
}