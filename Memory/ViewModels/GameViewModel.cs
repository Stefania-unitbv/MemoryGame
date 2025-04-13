using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.IO;
using MemoryGame.Helpers;
using System.Windows.Media.Imaging;


namespace MemoryGame.ViewModels
{
    public class GameViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private readonly GameService _gameService;
        private readonly CategoryService _categoryService;
        private readonly DispatcherTimer _gameTimer;

        private Game _currentGame;
        private string _currentUsername;
        private string _selectedCategory;


        public string CurrentUsername
        {
            get => _currentUsername;
            private set => SetProperty(ref _currentUsername, value);
        }
        private ObservableCollection<Card> _cards;
        private Card _firstSelectedCard;
        private Card _secondSelectedCard;
        private bool _isProcessingCards;
        private int _rows;
        private int _columns;
        private int _timeInSeconds;
        private TimeSpan _remainingTime;
        private bool _isGameOver;
        private bool _isCustomSize;



        public ObservableCollection<Card> Cards
        {
            get => _cards;
            set => SetProperty(ref _cards, value);
        }

        public ObservableCollection<string> Categories { get; private set; }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public int Rows
        {
            get => _rows;
            set
            {
                if (SetProperty(ref _rows, value))
                {
                    (NewGameCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public int Columns
        {
            get => _columns;
            set
            {
                if (SetProperty(ref _columns, value))
                {
                    (NewGameCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public int TimeInSeconds
        {
            get => _timeInSeconds;
            set => SetProperty(ref _timeInSeconds, value);
        }

        public TimeSpan RemainingTime
        {
            get => _remainingTime;
            set => SetProperty(ref _remainingTime, value);
        }

        public bool IsGameOver
        {
            get => _isGameOver;
            set => SetProperty(ref _isGameOver, value);
        }

        public bool IsCustomSize
        {
            get => _isCustomSize;
            set
            {
                if (SetProperty(ref _isCustomSize, value))
                {
                    if (!_isCustomSize)
                    {

                        Rows = 4;
                        Columns = 4;
                    }
                }
            }
        }

        // Comenzi

        private ICommand _showStatisticsCommand;
        private ICommand _exitCommand;

        public ICommand NewGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand OpenGameCommand { get; }
        public ICommand CardClickCommand { get; }

        public ICommand ShowStatisticsCommand
        {
            get => _showStatisticsCommand;
            set => SetProperty(ref _showStatisticsCommand, value);
        }

        public ICommand ExitCommand
        {
            get => _exitCommand;
            set => SetProperty(ref _exitCommand, value);
        }

        // Constructor

        public GameViewModel(UserService userService, GameService gameService, CategoryService categoryService, string username)
        {
            _userService = userService;
            _gameService = gameService;
            _categoryService = categoryService;
            _currentUsername = username;
            CurrentUsername = username;


            Cards = new ObservableCollection<Card>();
            Categories = new ObservableCollection<string>(_categoryService.GetCategoryNames());


            SelectedCategory = Categories.FirstOrDefault();
            Rows = 4;
            Columns = 4;
            TimeInSeconds = 120; // 2 minute
            IsCustomSize = false;
            IsGameOver = false;


            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromSeconds(1);
            _gameTimer.Tick += GameTimer_Tick;


            NewGameCommand = new RelayCommand(NewGame, CanStartNewGame);
            SaveGameCommand = new RelayCommand(SaveGame, CanSaveGame);
            OpenGameCommand = new RelayCommand(OpenGame, CanOpenGame);
            CardClickCommand = new RelayCommand(CardClick, CanClickCard);
            _showStatisticsCommand = new RelayCommand(_ => { });
            _exitCommand = new RelayCommand(_ => { });
        }



        private bool CanStartNewGame(object parameter)
        {
            if (SelectedCategory == null)
                return false;


            if (Rows < 2 || Rows > 6 || Columns < 2 || Columns > 6)
                return false;


            if ((Rows * Columns) % 2 != 0)
                return false;

            return true;
        }


        private void PreloadCardImages()
        {
            if (_currentGame == null || !_currentGame.Cards.Any())
                return;

            foreach (var card in _currentGame.Cards)
            {
                try
                {

                    if (!string.IsNullOrEmpty(card.ImagePath))
                    {
                        var image = new BitmapImage(new Uri(card.ImagePath));
                        Console.WriteLine($"Preloaded image for card {card.Id}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to preload image for card {card.Id}: {ex.Message}");
                }
            }
        }
        private void NewGame(object parameter)
        {
            try
            {

                if (_gameTimer.IsEnabled)
                {
                    _gameTimer.Stop();
                }


                _currentGame = _gameService.CreateNewGame(
                    _currentUsername,
                    SelectedCategory,
                    Rows,
                    Columns,
                    TimeInSeconds);


                Cards.Clear();
                foreach (var card in _currentGame.Cards)
                {
                    Cards.Add(card);
                }
                PreloadCardImages();

                _firstSelectedCard = null;
                _secondSelectedCard = null;
                _isProcessingCards = false;
                IsGameOver = false;
                RemainingTime = _currentGame.TotalTime;



                _gameTimer.Start();


                (SaveGameCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting new game: {ex.Message}\n\nStack trace: {ex.StackTrace}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CanSaveGame(object parameter)
        {
            return _currentGame != null && !IsGameOver;
        }

        private void SaveGame(object parameter)
        {
            if (_currentGame == null)
                return;

            try
            {

                _gameTimer.Stop();

                _gameService.SaveGame(_currentGame);

                MessageBox.Show("Game saved successfully!", "Save Game", MessageBoxButton.OK, MessageBoxImage.Information);


                if (!IsGameOver)
                {
                    _gameTimer.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                if (!IsGameOver)
                {
                    _gameTimer.Start();
                }
            }
        }

        private bool CanOpenGame(object parameter)
        {
            return _gameService.HasSavedGame(_currentUsername);
        }

        private void OpenGame(object parameter)
        {
            try
            {

                if (_gameTimer.IsEnabled)
                {
                    _gameTimer.Stop();
                }


                _currentGame = _gameService.LoadGame(_currentUsername);
                if (_currentGame == null)
                {
                    MessageBox.Show("No saved game found or error loading game.", "Open Game", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                SelectedCategory = _currentGame.CategoryName;
                Rows = _currentGame.Rows;
                Columns = _currentGame.Columns;
                TimeInSeconds = (int)_currentGame.TotalTime.TotalSeconds;
                IsCustomSize = !(Rows == 4 && Columns == 4);


                Cards.Clear();
                foreach (var card in _currentGame.Cards)
                {
                    Cards.Add(card);
                }


                _firstSelectedCard = null;
                _secondSelectedCard = null;
                _isProcessingCards = false;
                IsGameOver = _currentGame.IsCompleted;
                RemainingTime = _currentGame.RemainingTime();


                if (!IsGameOver)
                {
                    _gameTimer.Start();
                }


                (SaveGameCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanClickCard(object parameter)
        {
            if (IsGameOver || _isProcessingCards || _currentGame == null)
                return false;

            if (parameter is not Card card)
                return false;


            return !card.IsRevealed && !card.IsMatched;
        }

        private void CardClick(object parameter)
        {
            if (parameter is not Card card)
                return;

            ProcessCardSelection(card);
        }


        public void PrepareForExit()
        {

            _gameTimer.Stop();


            if (_currentGame != null && !IsGameOver)
            {
                var result = MessageBox.Show("Do you want to save the current game before exiting?",
                    "Save Game", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SaveGame(null);
                }
                else if (result == MessageBoxResult.Cancel)
                {

                    _gameTimer.Start();
                    throw new OperationCanceledException("Exit cancelled");
                }
            }
        }

        // Metode helper

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (_currentGame == null || IsGameOver)
            {
                _gameTimer.Stop();
                return;
            }


            TimeSpan elapsed = DateTime.Now - _currentGame.StartTime;
            _currentGame.ElapsedTime = elapsed;
            RemainingTime = _currentGame.RemainingTime();


            if (RemainingTime <= TimeSpan.Zero)
            {
                GameOver(false);
            }
        }

        private void ProcessCardSelection(Card card)
        {

            card.Flip();
            OnPropertyChanged(nameof(Cards));


            if (_firstSelectedCard == null)
            {
                _firstSelectedCard = card;
                return;
            }


            if (_secondSelectedCard == null && card != _firstSelectedCard)
            {
                _secondSelectedCard = card;
                _isProcessingCards = true;


                bool isMatch = _firstSelectedCard.ImagePath == _secondSelectedCard.ImagePath;


                Task.Delay(1000).ContinueWith(_ =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (isMatch)
                        {

                            _firstSelectedCard.Match();
                            _secondSelectedCard.Match();


                            if (Cards.All(c => c.IsMatched))
                            {
                                GameOver(true);
                            }
                        }
                        else
                        {

                            _firstSelectedCard.Flip();
                            _secondSelectedCard.Flip();
                        }


                        _firstSelectedCard = null;
                        _secondSelectedCard = null;
                        _isProcessingCards = false;
                    });
                });
            }
        }

        private void GameOver(bool isWin)
        {

            _gameTimer.Stop();


            IsGameOver = true;
            _currentGame.IsCompleted = true;


            _userService.UpdateUserStatistics(_currentUsername, isWin);


            string message = isWin
                ? "Congratulations! You've won the game!"
                : "Game over! Time has expired.";

            MessageBox.Show(message, "Game Over", MessageBoxButton.OK,
                isWin ? MessageBoxImage.Information : MessageBoxImage.Exclamation);


            _gameService.DeleteSavedGame(_currentUsername);


            (SaveGameCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }


}