using Quiz.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Quiz.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        public ICommand ShowSolverCommand { get; }
        public ICommand ShowEditorCommand { get; }

        public MainViewModel()
        {
            ShowSolverCommand = new RelayCommand(_ => CurrentView = new QuizSolverView());
            ShowEditorCommand = new RelayCommand(_ => CurrentView = new QuizEditorView());
            CurrentView = new QuizSolverView(); // widok domyślny
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
