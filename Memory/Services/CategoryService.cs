using MemoryGame.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MemoryGame.Services
{
    public class CategoryService
    {
        private List<Category> _categories;
        private readonly string _basePath;

        public CategoryService(string basePath = null)
        {
            _basePath = basePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Categories");
            _categories = new List<Category>();
            InitializeCategories();
        }

      
        private void InitializeCategories()
        {
            
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
                Console.WriteLine($"Created directory: {_basePath}");
            }
            else
            {
                Console.WriteLine($"Directory exists: {_basePath}");
            }

          
            Console.WriteLine("Subdirectories:");
            foreach (var dir in Directory.GetDirectories(_basePath))
            {
                Console.WriteLine($"- {dir}");
            }

          
            foreach (string categoryPath in Directory.GetDirectories(_basePath))
            {
                string categoryName = Path.GetFileName(categoryPath);
                Console.WriteLine($"Processing category: {categoryName} from {categoryPath}");

            
                List<string> imagePaths = Directory.GetFiles(categoryPath, "*.jpg")
                    .Concat(Directory.GetFiles(categoryPath, "*.jpeg"))
                    .Concat(Directory.GetFiles(categoryPath, "*.png"))
                    .Concat(Directory.GetFiles(categoryPath, "*.gif"))
                    .ToList();

                Console.WriteLine($"Found {imagePaths.Count} images in {categoryName}");

              
                foreach (var path in imagePaths.Take(3))
                {
                    Console.WriteLine($"Sample image path: {path}");
                    Console.WriteLine($"File exists: {File.Exists(path)}");
                }

            
                if (imagePaths.Any())
                {
                    _categories.Add(new Category(categoryName, imagePaths));
                }
                else
                {
                    Console.WriteLine($"WARNING: No images found in category {categoryName}");
                }
            }

         
            if (!_categories.Any())
            {
                Console.WriteLine("No categories found, creating default categories");
                CreateDefaultCategories();
            }
        }
        private void CreateDefaultCategories()
        {
            string[] categoryNames = { "Animals", "Nature", "Food" };

            foreach (string categoryName in categoryNames)
            {
                string categoryPath = Path.Combine(_basePath, categoryName);
                if (!Directory.Exists(categoryPath))
                {
                    Directory.CreateDirectory(categoryPath);
                }

              
                _categories.Add(new Category(categoryName, new List<string>()));
            }
        }

        public List<Category> GetAllCategories()
        {
            return _categories.ToList();
        }

        public Category GetCategoryByName(string name)
        {
            return _categories.FirstOrDefault(c => c.Name == name);
        }

        public List<string> GetCategoryNames()
        {
            return _categories.Select(c => c.Name).ToList();
        }
    }
}