using Quiz.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.ViewModel
{
    public class QuizEditorViewModel
    {
        public ObservableCollection<Question> Questions { get; set; } = new();
        public string QuizName { get; set; } = "Nowy Quiz";
    }
}
