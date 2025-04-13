using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.Models
{
    public class User
    {
        public string Username { get; set; }
        public string ImagePath { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }

        public User() // este folosit pt deserliazare ---->>> reflectie
        {

        }

        public User(string username, string imagePath)
        {
            Username = username;
            ImagePath = imagePath;
            GamesPlayed = 0;
            GamesWon = 0;
        }
    }
}
