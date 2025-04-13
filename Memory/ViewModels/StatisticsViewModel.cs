using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MemoryGame.ViewModels
{
    public class StatisticsViewModel : BaseViewModel
    {
        private readonly UserService _userService;

       
        public event EventHandler CloseRequested;

        public ObservableCollection<UserStatistics> Statistics { get; private set; }

        public ICommand CloseCommand { get; }

        public StatisticsViewModel(UserService userService)
        {
            _userService = userService;
            Statistics = new ObservableCollection<UserStatistics>();
            LoadStatistics();

            CloseCommand = new RelayCommand(Close);
        }

        private void LoadStatistics()
        {
            Statistics.Clear();
            foreach (var user in _userService.GetAllUsers())
            {
                Statistics.Add(new UserStatistics
                {
                    Username = user.Username,
                    GamesPlayed = user.GamesPlayed,
                    GamesWon = user.GamesWon
                });
            }
        }

        private void Close(object parameter)
        {
            
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }

  
    public class UserStatistics
    {
        public string Username { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public double WinPercentage => GamesPlayed > 0 ? (double)GamesWon / GamesPlayed * 100 : 0;
    }
}