using Quiz.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.ViewModel
{
    public class AnswerViewModel
    {
        public string Text { get; }
        public bool IsCorrect { get; }

        public AnswerViewModel(Answer answer)
        {
            Text = answer.Text;
            IsCorrect = answer.IsCorrect;
        }
    }
}
