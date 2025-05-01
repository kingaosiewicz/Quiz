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
            ShowSolverCommand = new RelayCommand(_ => ShowSolver());
            ShowEditorCommand = new RelayCommand(_ => ShowEditor());

            ShowSolver(); // Domyślny widok
        }

        private void ShowSolver()
        {
            var view = new Quiz.View.QuizSolverView();
            view.DataContext = new QuizSolverViewModel();
            CurrentView = view;
        }

        private void ShowEditor()
        {
            var view = new Quiz.View.QuizEditorView();
            view.DataContext = new QuizEditorViewModel(); // na razie pusty, ale gotowy na edycję
            CurrentView = view;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
