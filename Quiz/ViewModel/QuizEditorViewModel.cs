using Quiz.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.IO;

namespace Quiz.ViewModel
{
    public class QuizEditorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _quizName = "Nowy Quiz";
        public string QuizName
        {
            get => _quizName;
            set
            {
                if (_quizName != value)
                {
                    _quizName = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Question> Questions { get; set; } = new();
        public ObservableCollection<Model.Quiz> Quizzes { get; } = new(); // Fully qualify the Quiz type

        private Model.Quiz _selectedQuiz; // Fully qualify the Quiz type
        public Model.Quiz SelectedQuiz
        {
            get => _selectedQuiz;
            set
            {
                _selectedQuiz = value;
                OnPropertyChanged();
                if (_selectedQuiz != null)
                {
                    QuizName = _selectedQuiz.Name;
                    Questions.Clear();
                    foreach (var q in _selectedQuiz.Questions)
                        Questions.Add(q);
                }
            }
        }

        public ICommand AddQuestionCommand { get; }
        public ICommand SaveQuizCommand { get; }
        public ICommand LoadQuizCommand { get; }
        public ICommand CreateNewQuizCommand { get; }

        public QuizEditorViewModel()
        {
            AddQuestionCommand = new RelayCommand(_ => AddQuestion());
            SaveQuizCommand = new RelayCommand(_ => SaveQuiz());
            LoadQuizCommand = new RelayCommand(_ => LoadQuizzes());
            CreateNewQuizCommand = new RelayCommand(_ => CreateNewQuiz());
        }

        private void AddQuestion()
        {
            Questions.Add(new Question
            {
                Text = "Nowe pytanie",
                Answers = new List<Answer> // Changed from ObservableCollection to List
                {
                    new Answer { Text = "Odpowiedź 1", IsCorrect = false },
                    new Answer { Text = "Odpowiedź 2", IsCorrect = false },
                    new Answer { Text = "Odpowiedź 3", IsCorrect = false },
                    new Answer { Text = "Odpowiedź 4", IsCorrect = false }
                }
            });
        }

        private void SaveQuiz()
        {
            var quiz = new Model.Quiz // Fully qualify the Quiz type
            {
                Name = QuizName,
                Questions = Questions.ToList() // Convert ObservableCollection to List
            };

            Quizzes.Add(quiz);

            string json = System.Text.Json.JsonSerializer.Serialize(quiz);
            File.WriteAllText($"{quiz.Name}.json", json);
        }

        private void LoadQuizzes()
        {
            Quizzes.Clear();
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.json");
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var quiz = System.Text.Json.JsonSerializer.Deserialize<Model.Quiz>(json); // Fully qualify the Quiz type
                if (quiz != null)
                    Quizzes.Add(quiz);
            }
        }

        private void CreateNewQuiz()
        {
            QuizName = "Nowy Quiz";
            Questions.Clear();
        }
    }
}
