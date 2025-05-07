using Quiz.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.ViewModel
{
    public class QuestionViewModel : INotifyPropertyChanged
    {
        public string Text { get; }

        public ObservableCollection<AnswerViewModel> Answers { get; }

        public QuestionViewModel(Question question)
        {
            Text = question.Text;
            Answers = new ObservableCollection<AnswerViewModel>(
                question.Answers.Select(a => new AnswerViewModel(a)));

            foreach (var answer in Answers)
            {
                answer.PropertyChanged += (_, __) =>
                {
                    OnPropertyChanged(nameof(IsCorrectlyAnswered));
                    OnPropertyChanged(nameof(EvaluationMessage)); 
                };

            }
        }

        public bool IsCorrectlyAnswered =>
            Answers.All(a => a.IsSelected == a.IsCorrect);

        public string EvaluationMessage =>
            IsCorrectlyAnswered ? "✔ Poprawna odpowiedź" : $"✘ Błąd w odpowiedziach. Poprawne: {GetCorrectAnswers()}";

        private string GetCorrectAnswers()
        {
            return string.Join(", ", Answers.Where(a => a.IsCorrect).Select(a => a.Text));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
