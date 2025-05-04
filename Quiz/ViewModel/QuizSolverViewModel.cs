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

        public event Action RequestScrollToTop;

        private string _result;
        public string Result
        {
            get => _result;
            set { _result = value; OnPropertyChanged(); }
        }

        private bool _isStarted;
        public bool IsStarted
        {
            get => _isStarted;
            set { _isStarted = value; OnPropertyChanged(); }
        }


        private bool _isFinished;
        public bool IsFinished
        {
            get => _isFinished;
            set { _isFinished = value; OnPropertyChanged(); }
        }

        public ICommand StartQuizCommand { get; }
        public ICommand FinishQuizCommand { get; }
        public ICommand RetryCommand { get; }

        public QuizSolverViewModel()
        {
            LoadSampleQuiz();

            StartQuizCommand = new RelayCommand(_ => StartQuiz());
            FinishQuizCommand = new RelayCommand(_ => FinishQuiz());
            RetryCommand = new RelayCommand(_ => RetryQuiz());

            _timerService.Tick += _ => OnPropertyChanged(nameof(ElapsedTime));
        }

        private void LoadSampleQuiz()
        {
            var q1 = new Question
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

            var q2 = new Question
            {
                Text = "Wybierz liczby pierwsze:",
                Answers = new List<Answer>
                {
                    new() { Text = "2", IsCorrect = true },
                    new() { Text = "4", IsCorrect = false },
                    new() { Text = "3", IsCorrect = true },
                    new() { Text = "6", IsCorrect = false }
                }
            };

            Questions.Clear();
            Questions.Add(new QuestionViewModel(q1));
            Questions.Add(new QuestionViewModel(q2));
        }

        private void StartQuiz()
        {
            _timerService.Start();
            IsStarted = true;
        }

        private void FinishQuiz()
        {
            _timerService.Stop();
            int correct = Questions.Count(q => q.IsCorrectlyAnswered);
            Result = $"Wynik: {correct} / {Questions.Count}";
            IsFinished = true;
        }

        private void RetryQuiz()
        {
            _timerService.Stop();
            _timerService = new TimerService();
            _timerService.Tick += _ => OnPropertyChanged(nameof(ElapsedTime));
            OnPropertyChanged(nameof(ElapsedTime));

            foreach (var question in Questions)
            {
                foreach (var answer in question.Answers)
                    answer.IsSelected = false;
            }

            Result = string.Empty;
            IsFinished = false;
            IsStarted = false;
            RequestScrollToTop?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
