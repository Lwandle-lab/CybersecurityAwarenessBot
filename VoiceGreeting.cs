
//======================================================================
                                //VOICEGREETING
//======================================================================

//This part of the program plays a short pre-recorded WAV voice greeting using System.Media.SoundPlayer
//when the application starts. If by any chance the audio file doesn't play, the chatbot will still 
//run without crashing.

using System.IO;
using System.Windows.Media;

namespace CybersecurityAwarenessBot.Services;

// Plays Audio/greetings.wav when the app starts (carried over from Part 1).
// MediaPlayer is used because it is built into WPF and needs no extra NuGet package.
public static class VoiceGreeting
{
    // Kept in a field so the player isn't garbage collected while the audio is playing.
    private static MediaPlayer? _player;

    // Returns false if the file is missing or can't be played, so the app simply carries on.
    public static bool Play()
    {
        try
        {
            string path = Path.Combine(AppContext.BaseDirectory, "Audio", "greetings.wav");
            if (!File.Exists(path))
            {
                return false;
            }

            _player = new MediaPlayer();
            _player.Open(new Uri(path, UriKind.Absolute));
            _player.Play();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}