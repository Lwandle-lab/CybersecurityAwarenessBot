//=======================================================
                        //QUIZ QUESTION
//=======================================================

//This is a one quiz question. True/False questions simply have the two options "True" and "False".
using System.Collections.Generic;

namespace CybersecurityAwarenessBot.Models
{
    public class QuizQuestion
    {
        public string Text { get; set; } = string.Empty;

        public List<string> Options { get; set; } = new();

        public int CorrectIndex { get; set; }

        public string Explanation { get; set; } = string.Empty;

        public bool IsTrueFalse { get; set; }
    }
}