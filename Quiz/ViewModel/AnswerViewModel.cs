using Quiz.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.ViewModel
{
    public class AnswerViewModel(string text, bool isCorrect)
    {
        public string Text { get; } = text;
        public bool IsCorrect { get; } = isCorrect;

        public AnswerViewModel(Answer answer) : this(answer.Text, answer.IsCorrect) { }
    }
}
