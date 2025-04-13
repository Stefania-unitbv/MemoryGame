using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MemoryGame.Converters
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            string imagePath = value.ToString();

            try
            {
                
                if (imagePath.StartsWith("file:///"))
                {
                  
                    return new BitmapImage(new Uri(imagePath));
                }
                else if (Path.IsPathRooted(imagePath))
                {
                   
                    string uriPath = "file:///" + imagePath.Replace('\\', '/');
                    return new BitmapImage(new Uri(uriPath));
                }
                else
                {
                    
                    string fullPath = Path.GetFullPath(imagePath);
                    string uriPath = "file:///" + fullPath.Replace('\\', '/');
                    return new BitmapImage(new Uri(uriPath));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}