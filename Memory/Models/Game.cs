using System;
using System.Collections.Generic;

namespace MemoryGame.Models
{
    public class Game
    {
        public string Username { get; set; }
        public string CategoryName { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<Card> Cards { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan TotalTime { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public bool IsCompleted { get; set; }

        public Game()
        {
            Cards = new List<Card>();
            StartTime = DateTime.Now;
            IsCompleted = false;
        }

      
        public bool IsWon()
        {
            return Cards.TrueForAll(c => c.IsMatched);
        }

        public TimeSpan RemainingTime()
        {
            TimeSpan remaining = TotalTime - ElapsedTime;
            return remaining.TotalSeconds > 0 ? remaining : TimeSpan.Zero;
        }
    }
}