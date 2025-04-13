using System;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MemoryGame.Models;
namespace MemoryGame.Models
{
    public class Card : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public int Id { get; set; }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (value != null && !value.StartsWith("file:///"))
                {
                    string path = value;

                   
                    if (!Path.IsPathRooted(path))
                    {
                        path = Path.GetFullPath(path);
                    }

                    // Convert to proper URI format (file:///)
                    string formattedPath = "file:///" + path.Replace('\\', '/');
                    SetProperty(ref _imagePath, formattedPath);
                }
                else
                {
                    SetProperty(ref _imagePath, value);
                }
            }
        }

        private bool _isRevealed;
        public bool IsRevealed
        {
            get => _isRevealed;
            set => SetProperty(ref _isRevealed, value);
        }

        private bool _isMatched;
        public bool IsMatched
        {
            get => _isMatched;
            set => SetProperty(ref _isMatched, value);
        }

        public Card()
        {
            IsRevealed = false;
            IsMatched = false;
        }

        public Card(int id, string imagePath)
        {
            Id = id;
            ImagePath = imagePath;
            IsRevealed = false;
            IsMatched = false;
        }

      
        public void Flip()
        {
            IsRevealed = !IsRevealed;
        }

      
        public void Match()
        {
            IsMatched = true;
        }

      
        public void Reset()
        {
            IsRevealed = false;
            IsMatched = false;
        }
    }
}