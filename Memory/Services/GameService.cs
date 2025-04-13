using MemoryGame.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MemoryGame.Services
{
    public class GameService
    {
        private readonly string _gamesDirectory;
        private readonly CategoryService _categoryService;
        private readonly Random _random;

        public GameService(CategoryService categoryService, string gamesDirectory = null)
        {
            _gamesDirectory = gamesDirectory ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedGames");
            _categoryService = categoryService;
            _random = new Random();

          
            if (!Directory.Exists(_gamesDirectory))
            {
                Directory.CreateDirectory(_gamesDirectory);
            }
        }

       
        public Game CreateNewGame(string username, string categoryName, int rows, int columns, int timeInSeconds)
        {
            Category category = _categoryService.GetCategoryByName(categoryName);
            if (category == null || !category.ImagePaths.Any())
            {
                throw new ArgumentException($"Category {categoryName} not found or has no images.");
            }

            int totalCards = rows * columns;
            if (totalCards % 2 != 0)
            {
                throw new ArgumentException("Total number of cards must be even.");
            }

          
            int requiredImages = totalCards / 2;
            if (category.ImagePaths.Count < requiredImages)
            {
                throw new ArgumentException($"Not enough images in category {categoryName}. Need {requiredImages} but only {category.ImagePaths.Count} available.");
            }

           
            List<string> selectedImages = category.ImagePaths
                .OrderBy(x => _random.Next())
                .Take(requiredImages)
                .ToList();

            Console.WriteLine($"Selected {selectedImages.Count} images for the game");

           
            List<Card> cards = new List<Card>();
            int id = 0;
            foreach (string imagePath in selectedImages)
            {
               
                cards.Add(new Card(id++, imagePath));
                cards.Add(new Card(id++, imagePath));

                Console.WriteLine($"Added card with image: {imagePath} (Exists: {File.Exists(imagePath)})");
            }

           
            cards = cards.OrderBy(x => _random.Next()).ToList();

           
            return new Game
            {
                Username = username,
                CategoryName = categoryName,
                Rows = rows,
                Columns = columns,
                Cards = cards,
                StartTime = DateTime.Now,
                TotalTime = TimeSpan.FromSeconds(timeInSeconds),
                ElapsedTime = TimeSpan.Zero,
                IsCompleted = false
            };
        }
       
 
        public void SaveGame(Game game)
        {
            
            game.ElapsedTime = DateTime.Now - game.StartTime;

            string filePath = GetGameFilePath(game.Username);
            string json = JsonSerializer.Serialize(game, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

       
        public Game LoadGame(string username)
        {
            string filePath = GetGameFilePath(username);
            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                Game game = JsonSerializer.Deserialize<Game>(json);

             
                TimeSpan elapsed = game.ElapsedTime;
                game.StartTime = DateTime.Now - elapsed;

                return game;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading game: {ex.Message}");
                return null;
            }
        }

       
        public bool DeleteSavedGame(string username)
        {
            string filePath = GetGameFilePath(username);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }

        public bool HasSavedGame(string username)
        {
            string filePath = GetGameFilePath(username);
            return File.Exists(filePath);
        }

        private string GetGameFilePath(string username)
        {
            return Path.Combine(_gamesDirectory, $"{username}_game.json");
        }
    }
}