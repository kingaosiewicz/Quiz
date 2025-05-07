using Quiz.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Quiz.ViewModel
{
    public class QuizSolverViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Model.Quiz> Quizzes { get; } = new();

        private Model.Quiz _selectedQuiz;
        public Model.Quiz SelectedQuiz
        {
            get => _selectedQuiz;
            set
            {
                _selectedQuiz = value;
                OnPropertyChanged();

                QuizTitle = _selectedQuiz?.Name ?? "Nie wybrano quizu";
                Questions.Clear();
                if (_selectedQuiz != null)
                {
                    foreach (var question in _selectedQuiz.Questions)
                        Questions.Add(new QuestionViewModel(question));
                }
            }
        }

        private string _quizTitle = "Nie wybrano quizu";
        public string QuizTitle
        {
            get => _quizTitle;
            set { _quizTitle = value; OnPropertyChanged(); }
        }

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
            LoadQuizzes();

            StartQuizCommand = new RelayCommand(_ => StartQuiz(), _ => SelectedQuiz != null);
            FinishQuizCommand = new RelayCommand(_ => FinishQuiz(), _ => IsStarted);
            RetryCommand = new RelayCommand(_ => RetryQuiz(), _ => IsFinished);

            _timerService.Tick += _ => OnPropertyChanged(nameof(ElapsedTime));
        }

        private void LoadQuizzes()
        {
            Quizzes.Clear();
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.enc");
            foreach (var file in files)
            {
                var encryptedData = File.ReadAllBytes(file);
                string json = AesEncryption.Decrypt(encryptedData, "moje_super_tajne_haslo");
                var quiz = JsonSerializer.Deserialize<Model.Quiz>(json);
                if (quiz != null)
                    Quizzes.Add(quiz);
            }
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
