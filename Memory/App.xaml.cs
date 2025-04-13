using MemoryGame.Commands;
using MemoryGame.Services;
using MemoryGame.ViewModels;
using MemoryGame.Views;
using System;
using System.IO;
using System.Windows;

namespace MemoryGame
{
    public partial class App : Application
    {
        private UserService _userService;
        private GameService _gameService;
        private CategoryService _categoryService;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
          
            _userService = new UserService();

            string categoryBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Categories");
            _categoryService = new CategoryService(categoryBasePath);

            _gameService = new GameService(_categoryService);

          
            ShowLoginScreen();
        }

        public void ShowLoginScreen()
        {
            var loginViewModel = new LoginViewModel(_userService);
            var loginView = new LoginView
            {
                DataContext = loginViewModel
            };

            loginViewModel.PlayCommand = new RelayCommand(_ =>
            {
                    loginView.Hide();
                    ShowGameScreen(loginViewModel.SelectedUser.Username);
                
            }, _ => loginViewModel.SelectedUser != null);

            loginView.Show();
        }

        private void ShowGameScreen(string username)
        {
            var gameViewModel = new GameViewModel(_userService, _gameService, _categoryService, username);
            var gameView = new GameView
            {
                DataContext = gameViewModel
            };

          
            gameViewModel.ShowStatisticsCommand = new RelayCommand(_ =>
            {
                ShowStatisticsScreen(gameView);
            });

            gameViewModel.ExitCommand = new RelayCommand(_ =>
            {
                try
                {
                    gameViewModel.PrepareForExit();
                    gameView.Close();
                    ShowLoginScreen();
                }
                catch (OperationCanceledException)
                {
                   
                }
            });

            gameView.Show();
        }

        private void ShowStatisticsScreen(Window owner)
        {
            var statisticsViewModel = new StatisticsViewModel(_userService);
            var statisticsView = new StatisticsView
            {
                DataContext = statisticsViewModel,
                Owner = owner
            };

            statisticsView.ShowDialog();
        }
    }
}