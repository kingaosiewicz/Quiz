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
                IsInEditMode = false;
                if (_selectedQuiz != null)
                {
                    QuizName = _selectedQuiz.Name;
                    Questions.Clear();
                    foreach (var q in _selectedQuiz.Questions)
                        Questions.Add(q);
                }
            }
        }

        public bool IsReadOnlyMode => !IsInEditMode;
        private bool _isInEditMode;
        private bool _isNewQuiz;
        public bool IsEditingExistingQuiz => IsInEditMode && !_isNewQuiz;
        public bool IsInEditMode
        {
            get => _isInEditMode;
            set
            {
                if (_isInEditMode != value)
                {
                    _isInEditMode = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsReadOnlyMode));
                    OnPropertyChanged(nameof(IsEditingExistingQuiz));
                }
            }
        }

        private Model.Quiz _originalQuiz; // przechowujemy oryginał na wypadek anulowania
        public ICommand AddQuestionCommand { get; }
        public ICommand SaveQuizCommand { get; }
        public ICommand LoadQuizCommand { get; }
        public ICommand CreateNewQuizCommand { get; }
        public ICommand DeleteQuizCommand { get; }
        public ICommand EditQuizCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand RemoveQuestionCommand { get; }


        public QuizEditorViewModel()
        {
            AddQuestionCommand = new RelayCommand(_ => AddQuestion());
            SaveQuizCommand = new RelayCommand(_ => SaveQuiz());
            LoadQuizCommand = new RelayCommand(_ => LoadQuizzes());
            CreateNewQuizCommand = new RelayCommand(_ => CreateNewQuiz());
            DeleteQuizCommand = new RelayCommand(_ => DeleteQuiz(), _ => SelectedQuiz != null);
            EditQuizCommand = new RelayCommand(_ => BeginEdit(), _ => SelectedQuiz != null && !IsInEditMode);
            SaveChangesCommand = new RelayCommand(_ => CommitEdit(),
            _ => IsInEditMode && (_isNewQuiz || SelectedQuiz != null));

            CancelEditCommand = new RelayCommand(_ => CancelEdit(), _ => IsInEditMode);
            RemoveQuestionCommand = new RelayCommand(q => RemoveQuestion(q as Question), _ => IsInEditMode);

            // pamiętaj o invalidacji CommandManager jeśli RelayCommand opiera się na RequerySuggested
            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(SelectedQuiz) || e.PropertyName == nameof(IsInEditMode))
                    CommandManager.InvalidateRequerySuggested();
            };
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
            if (!Questions.Any())
            {
                MessageBox.Show("Quiz musi zawierać przynajmniej jedno pytanie.");
                return;
            }

            var quiz = new Model.Quiz
            {
                Name = QuizName,
                Questions = Questions.ToList()
            };

            Quizzes.Add(quiz);

            string json = JsonSerializer.Serialize(quiz);
            string password = "moje_super_tajne_haslo";
            byte[] encryptedData = AesEncryption.Encrypt(json, password);
            File.WriteAllBytes($"{quiz.Name}.enc", encryptedData);
        }


        private void LoadQuizzes()
        {
            Quizzes.Clear();
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.enc");
            foreach (var file in files)
            {
                var encryptedData = File.ReadAllBytes(file);

                // Zdefiniowanie hasła (możesz je zmienić na coś bardziej bezpiecznego)
                string password = "moje_super_tajne_haslo"; // Hasło używane do deszyfrowania

                // Deszyfrowanie danych
                string json = AesEncryption.Decrypt(encryptedData, password);

                // Deserializacja JSON
                var quiz = System.Text.Json.JsonSerializer.Deserialize<Model.Quiz>(json);
                if (quiz != null)
                    Quizzes.Add(quiz);
            }
        }

        private void CreateNewQuiz()
        {
            QuizName = "Nowy Quiz";
            Questions.Clear();

            // Nie mamy jeszcze obiektu w Quizzes, więc SelectedQuiz zostawiamy null 
            SelectedQuiz = null;

            // Włącz tryb edycji, by odblokować przyciski i pola
            _isNewQuiz = true;
            IsInEditMode = true;
            OnPropertyChanged(nameof(IsEditingExistingQuiz));
        }
        private void DeleteQuiz()
        {
            //1.Złap referencję do quizu, który chcemy usunąć
            var quizToDelete = SelectedQuiz;
            if (quizToDelete == null)
            {
                MessageBox.Show("Nie wybrano żadnego quizu do usunięcia.");
                return;
            }

            // 2. Usuń go z kolekcji
            Quizzes.Remove(quizToDelete);

            // 3. Wyczyść SelectedQuiz (to spowoduje, że przycisk "Usuń" się zablokuje)
            SelectedQuiz = null;

            // 4. Usuń związany plik .enc
            try
            {
                // Budujemy ścieżkę na podstawie nazwy quizu
                var filePath = $"{quizToDelete.Name}.enc";
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas usuwania pliku: {ex.Message}");
            }
        }
        private void BeginEdit()
        {
            _isNewQuiz = false;
            // zachowaj oryginał
            _originalQuiz = new Model.Quiz
            {
                Name = SelectedQuiz.Name,
                Questions = SelectedQuiz.Questions.Select(q => new Question
                {
                    Text = q.Text,
                    Answers = q.Answers.Select(a => new Answer
                    {
                        Text = a.Text,
                        IsCorrect = a.IsCorrect
                    }).ToList()
                }).ToList()
            };

            // ustaw pola edycyjne
            QuizName = SelectedQuiz.Name;
            Questions.Clear();
            foreach (var q in SelectedQuiz.Questions)
                Questions.Add(q);

            IsInEditMode = true;
            OnPropertyChanged(nameof(IsEditingExistingQuiz));
        }
        private void CommitEdit()
        {
            if (!Questions.Any())
            {
                MessageBox.Show("Quiz musi zawierać przynajmniej jedno pytanie.");
                return;
            }
            // Stwórz lokalną referencję do quizu, który chcemy zapisać
            Model.Quiz quizToSave;

            // 1) Obsługa nowego quizu
            if (_isNewQuiz)
            {
                quizToSave = new Model.Quiz
                {
                    Name = QuizName,
                    Questions = Questions.ToList()
                };
                Quizzes.Add(quizToSave);
                SelectedQuiz = quizToSave;
            }
            else
            {
                // 2) Obsługa edycji istniejącego
                quizToSave = SelectedQuiz;
                // Jeśli ktoś wcisnął „Zapisz zmiany” nie w edycji lub SelectedQuiz okazało się null, wyjdziemy
                if (quizToSave == null) return;

                // Usuń stary plik, gdy zmieniła się nazwa
                if (_originalQuiz.Name != QuizName)
                {
                    var oldPath = $"{_originalQuiz.Name}.enc";
                    if (File.Exists(oldPath))
                        File.Delete(oldPath);
                }

                // Nadpisujemy dane modelu
                quizToSave.Name = QuizName;
                quizToSave.Questions = Questions.ToList();

                // Odświeżenie listy, jeśli potrzebne
                var idx = Quizzes.IndexOf(quizToSave);
                if (idx >= 0)
                    Quizzes[idx] = quizToSave;
            }

            // 3) Serializacja + szyfrowanie + zapis pliku na podstawie quizToSave
            var json = JsonSerializer.Serialize(quizToSave);
            var encrypted = AesEncryption.Encrypt(json, "moje_super_tajne_haslo");
            File.WriteAllBytes($"{quizToSave.Name}.enc", encrypted);

            // 4) Wyłączamy tryb edycji
            IsInEditMode = false;

            // Reset flagi nowego quizu
            _isNewQuiz = false;
        }
        private void CancelEdit()
        {
            // przywróć oryginał z kopii
            SelectedQuiz.Name = _originalQuiz.Name;
            SelectedQuiz.Questions = _originalQuiz.Questions;
            QuizName = _originalQuiz.Name;

            Questions.Clear();
            foreach (var q in SelectedQuiz.Questions)
                Questions.Add(q);

            IsInEditMode = false;
        }
        private void RemoveQuestion(Question question)
        {
            if (question != null)
                Questions.Remove(question);
        }
    }
}

