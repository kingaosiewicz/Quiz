using Quiz.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.ViewModel
{
    public class QuestionViewModel
    {
        public string Text { get; }
        public ObservableCollection<AnswerViewModel> Answers { get; }

        public QuestionViewModel(Question question)
        {
            Text = question.Text;
            Answers = new ObservableCollection<AnswerViewModel>(
                question.Answers.Select(a => new AnswerViewModel(a)));
        }

        public bool IsCorrectlyAnswered =>
            Answers.Where(a => a.IsCorrect).All(a => a.IsSelected) &&
            Answers.Where(a => !a.IsCorrect).All(a => !a.IsSelected);
    }
}
