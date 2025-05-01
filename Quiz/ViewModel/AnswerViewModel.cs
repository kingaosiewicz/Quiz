using Quiz.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.ViewModel
{
    public class AnswerViewModel : INotifyPropertyChanged
    {
        public string Text { get; }
        public bool IsCorrect { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
        }

        public AnswerViewModel(Answer answer)
        {
            Text = answer.Text;
            IsCorrect = answer.IsCorrect;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
