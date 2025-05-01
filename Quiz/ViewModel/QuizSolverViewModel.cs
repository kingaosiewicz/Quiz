using Quiz.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Quiz.ViewModel
{
    public class QuizSolverViewModel : INotifyPropertyChanged
    {
        public string QuizTitle { get; set; } = "Przykładowy Quiz";

        public ObservableCollection<QuestionViewModel> Questions { get; set; } = new();

        private TimerService _timerService = new();
        public string ElapsedTime => $"{_timerService.SecondsElapsed} sek.";

        private string _result;
        public string Result
        {
            get => _result;
            set { _result = value; OnPropertyChanged(); }
        }

        public ICommand StartQuizCommand { get; }
        public ICommand FinishQuizCommand { get; }

        public QuizSolverViewModel()
        {
            LoadSampleQuiz();

            StartQuizCommand = new RelayCommand(_ => StartQuiz());
            FinishQuizCommand = new RelayCommand(_ => FinishQuiz());

            _timerService.Tick += _ => OnPropertyChanged(nameof(ElapsedTime));
        }

        private void StartQuiz()
        {
            _timerService.Start();
        }

        private void FinishQuiz()
        {
            _timerService.Stop();
            int correct = Questions.Count(q => q.IsCorrectlyAnswered);
            Result = $"Wynik: {correct}/{Questions.Count}";
        }

        private void LoadSampleQuiz()
        {
            var q = new Question
            {
                Text = "Które miasta były stolicą Polski?",
                Answers = new List<Answer>
                {
                    new() { Text = "Warszawa", IsCorrect = true },
                    new() { Text = "Kraków", IsCorrect = true },
                    new() { Text = "Poznań", IsCorrect = false },
                    new() { Text = "Gniezno", IsCorrect = true }
                }
            };
            Questions.Add(new QuestionViewModel(q));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
