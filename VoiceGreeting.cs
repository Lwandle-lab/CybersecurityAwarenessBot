
//======================================================================
                            //VOICEGREETING
//======================================================================

//This part of the program plays a short pre-recorded WAV voice greeting using System.Media.SoundPlayer
//when the application starts. If by any chance the audio file doesn't play, the chatbot will still 
//run without crashing.

namespace CybersecurityChatbotGUI
{
    internal static class VoiceGreeting
    {
        //1. Tries these file names in the Assets folder, in this order
        private static readonly string[] AudioFileNames = { "greetings.wav", "greeting.wav" };

        private static System.Media.SoundPlayer? _player;

        //2. Returns null on success, or a short message explaining why it was skipped.
        public static string? Play()
        {
            try
            {
                string? path = FindAudioFile();

                if (path == null)
                {
                    return "[Voice greeting skipped: Assets/greetings.wav not found.]";
                }

                _player = new System.Media.SoundPlayer(path);
                _player.Play();
                return null;
            }
            catch (Exception ex)
            {
                return $"[Voice greeting could not be played: {ex.Message}]";
            }
        }

        private static string? FindAudioFile()
        {
            foreach (string fileName in AudioFileNames)
            {
                string path = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", fileName);

                if (System.IO.File.Exists(path))
                {
                    return path;
                }
            }

            return null;
        }
    }
}