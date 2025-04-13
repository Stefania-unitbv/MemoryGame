using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Services;
using Microsoft.Win32; // openFileDialog
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace MemoryGame.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly UserService _userService;
        private User _selectedUser;
        private string _newUsername;
        private string _selectedImagePath;

        public ObservableCollection<User> Users { get; private set; }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (SetProperty(ref _selectedUser, value))
                {
                    (DeleteUserCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (PlayCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string NewUsername
        {
            get => _newUsername;
            set => SetProperty(ref _newUsername, value);
        }

        public string SelectedImagePath
        {
            get => _selectedImagePath;
            set => SetProperty(ref _selectedImagePath, value);
        }

        private ICommand _playCommand;
        public ICommand PlayCommand
        {
            get => _playCommand;
            set => SetProperty(ref _playCommand, value);
        }

        public LoginViewModel(UserService userService)
        {
            _userService = userService;
            Users = new ObservableCollection<User>(_userService.GetAllUsers());

            CreateUserCommand = new RelayCommand(CreateUser, CanCreateUser);
            SelectImageCommand = new RelayCommand(SelectImage);
            DeleteUserCommand = new RelayCommand(DeleteUser, CanDeleteUser);
            _playCommand = new RelayCommand(parameter => { }, CanPlay);
        }

        private bool CanCreateUser(object parameter)
        {
            return !string.IsNullOrWhiteSpace(NewUsername) && !string.IsNullOrWhiteSpace(SelectedImagePath);
        }

        private void CreateUser(object parameter)
        {
            if (_userService.GetUserByUsername(NewUsername) != null)
            {
                MessageBox.Show("Numele de utilizator există deja!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
              
                string relativePath = MakeRelativePath(SelectedImagePath);

                var newUser = new User(NewUsername, relativePath);
                if (_userService.AddUser(newUser))
                {
                    Users.Add(newUser);
                    NewUsername = string.Empty;
                    SelectedImagePath = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la crearea utilizatorului: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string MakeRelativePath(string fullPath)
        {
            
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            string imagesDir = Path.Combine(appDir, "Images");

          
            if (!Directory.Exists(imagesDir))
            {
                Directory.CreateDirectory(imagesDir);
            }

        
            string fileName = Path.GetFileName(fullPath);
            string destPath = Path.Combine(imagesDir, fileName);

            
            if (fullPath != destPath && !File.Exists(destPath))
            {
                File.Copy(fullPath, destPath);
            }

           
            return Path.Combine("Images", fileName);
        }

        private void SelectImage(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif",
                Title = "Selectează o imagine"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedImagePath = openFileDialog.FileName;//calea completa
                (CreateUserCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private bool CanDeleteUser(object parameter)
        {
            return SelectedUser != null;
        }

        private void DeleteUser(object parameter)
        {
            if (SelectedUser == null) return;

            var result = MessageBox.Show(
                $"Ești sigur că vrei să ștergi utilizatorul {SelectedUser.Username}? Toate datele asociate vor fi șterse!",
                "Confirmare",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (_userService.DeleteUser(SelectedUser.Username))
                {
                    Users.Remove(SelectedUser);
                    SelectedUser = null;
                }
            }
        }

        private bool CanPlay(object parameter)
        {
            return SelectedUser != null;
        }

        public ICommand CreateUserCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand DeleteUserCommand { get; }
    }
}