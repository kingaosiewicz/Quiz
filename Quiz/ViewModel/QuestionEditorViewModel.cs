using Quiz.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.ViewModel
{
    public class QuestionEditorViewModel
    {
        public Question EditedQuestion { get; set; } = new();
    }
}
