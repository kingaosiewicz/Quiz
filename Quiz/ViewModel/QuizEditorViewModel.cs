using Quiz.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Quiz.ViewModel
{
    public class QuizEditorViewModel
    {
        public ObservableCollection<Question> Questions { get; set; } = new();
        public string QuizName { get; set; } = "Nowy Quiz";
        public string Password { get; set; } = ""; // tymczasowo na sztywno

        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        public QuizEditorViewModel()
        {
            SaveCommand = new RelayCommand(_ => Save());
            LoadCommand = new RelayCommand(_ => Load());
        }

        private void Save()
        {
            var quiz = new Quiz.Model.Quiz
            {
                Name = QuizName,
                Questions = Questions.ToList()
            };
            QuizSerializer.SaveEncrypted($"{QuizName}.quiz", quiz, Password);
        }

        private void Load()
        {
            var loaded = QuizSerializer.LoadEncrypted($"{QuizName}.quiz", Password);
            Questions.Clear();
            foreach (var q in loaded.Questions)
                Questions.Add(q);
        }
    }
}
