using System;
using System.Collections.Generic;

namespace MemoryGame.Models
{
    public class Category
    {
        public string Name { get; set; }
        public List<string> ImagePaths { get; set; }

        public Category()
        {
            ImagePaths = new List<string>();
        }

        public Category(string name, List<string> imagePaths) 
                                                              

        {
            Name = name;
            ImagePaths = imagePaths ?? new List<string>();
        }
    }
}