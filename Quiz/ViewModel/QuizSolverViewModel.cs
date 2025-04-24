using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Quiz.Model;

namespace Quiz.ViewModel
{
    public class QuizSolverViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Question> Questions { get; set; }
        public Question CurrentQuestion => Questions[CurrentQuestionIndex];

        public int CurrentQuestionIndex { get; set; }

        public int SelectedAnswerIndex { get; set; } = -1;

        private string _result;
        public string Result
        {
            get => _result;
            set { _result = value; OnPropertyChanged(); }
        }

        public QuizSolverViewModel()
        {
            Questions = new ObservableCollection<Question>
            {
                new Question
                {
                    Text = "Stolica Polski to:",
                    Answers = new List<Answer>
                    {
                        new() { Text = "Warszawa", IsCorrect = true },
                        new() { Text = "Kraków", IsCorrect = false },
                        new() { Text = "Wrocław", IsCorrect = false },
                        new() { Text = "Gdańsk", IsCorrect = false }
                    }
                }
            };
        }

        public void CheckAnswer()
        {
            Result = CurrentQuestion.Answers[SelectedAnswerIndex].IsCorrect ? "Dobrze!" : "Źle!";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
