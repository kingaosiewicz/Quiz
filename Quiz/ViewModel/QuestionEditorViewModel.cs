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
    public class QuestionEditorViewModel : INotifyPropertyChanged
    {
        public string Text { get; set; }
        public ObservableCollection<Answer> Answers { get; }

        public event Action<QuestionEditorViewModel> OnSave;
        public event Action OnCancel;

        public ICommand SaveQuestionCommand { get; }
        public ICommand CancelEditCommand { get; }

        public QuestionEditorViewModel()
        {
            Answers = new ObservableCollection<Answer>
            {
                new Answer(), new Answer(), new Answer(), new Answer()
            };

            SaveQuestionCommand = new RelayCommand(_ => OnSave?.Invoke(this));
            CancelEditCommand = new RelayCommand(_ => OnCancel?.Invoke());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}