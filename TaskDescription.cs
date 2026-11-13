//==========================================================
                    //TASK DESCRIPTION
//==========================================================

namespace CybersecurityAwarenessBot
{
    public static class TaskDescriptions
    {
        public static string For(string title)
        {
            return title switch
            {
                "Password Audit" => "Review and strengthen passwords.",
                "Enable MFA" => "Enable multi-factor authentication.",
                "Software Update" => "Install the latest security updates.",
                "Phishing Awareness" => "Complete phishing awareness training.",
                _ => "No description available."
            };
        }
    }
}