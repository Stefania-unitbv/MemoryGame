using MemoryGame.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MemoryGame.Services
{
    public class UserService
    {
        private readonly string _usersFilePath; 
        private List<User> _users;

        public UserService(string usersFilePath = "users.json")
        {
            _usersFilePath = usersFilePath;
            _users = new List<User>();
            LoadUsers();
        }

        private void LoadUsers()
        {
            if (File.Exists(_usersFilePath))
            {
                try
                {
                    string json = File.ReadAllText(_usersFilePath);
                    _users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
                   
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine($"Error loading users: {ex.Message}");
                    _users = new List<User>();
                }
            }
        }

        private void SaveUsers()
        {
            try
            {
                string json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_usersFilePath, json);

            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error saving users: {ex.Message}");
            }
        }

        public List<User> GetAllUsers()
        {
            return _users.ToList();
        }

        public User GetUserByUsername(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username);
        }

        public bool AddUser(User user)
        {
            if (_users.Any(u => u.Username == user.Username))
            {
                return false; 
            }

            _users.Add(user);
            SaveUsers();
            return true;
        }

        public bool DeleteUser(string username)
        {
            User user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return false;
            }

            _users.Remove(user);
            SaveUsers();
            return true;
        }

        public void UpdateUserStatistics(string username, bool gameWon)
        {
            User user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return; 
            }

            user.GamesPlayed++;
            if (gameWon)
            {
                user.GamesWon++;
            }

            SaveUsers();
        }
    }
}