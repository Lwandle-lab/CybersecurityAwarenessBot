//===============================================================
                        //USER PROFILE
//===============================================================
namespace CybersecurityAwarenessBot.Models;

//Stores the basic information about the person chattimg with the bot (carried over from Part 1)
public class UserProfile
{
    public string Name { get; set; } = string.Empty;

    //Rememebers the messages such as "I am interested in privacy".
    public string FavouriteTopic {  get; set; } = string.Empty;

    //Validation and the toring o the name. returns false if the input is empty or unreasonably long.
    public bool TrySetName(string rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
        {
            return false;
        }

        string trimmed = rawInput.Trim();
        if (trimmed.Length > 40 )
        {
            return false;
        }

        Name = char.ToUpper(trimmed[0]) + trimmed.Substring(1);
        return true;
    }
}