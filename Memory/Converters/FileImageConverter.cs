using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MemoryGame.Converters
{
    public class FileImageConverter : IValueConverter
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
                  
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(imagePath);
                    bitmap.EndInit();
                    bitmap.Freeze(); 

                    System.Diagnostics.Debug.WriteLine($"Successfully loaded image from URI: {imagePath}");
                    return bitmap;
                }
                else
                {
                    
                    string fullPath = Path.IsPathRooted(imagePath) ?
                        imagePath : Path.GetFullPath(imagePath);

                    if (File.Exists(fullPath))
                    {
                        string uriPath = "file:///" + fullPath.Replace('\\', '/');

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.UriSource = new Uri(uriPath);
                        bitmap.EndInit();
                        bitmap.Freeze(); 

                        System.Diagnostics.Debug.WriteLine($"Successfully loaded image from path: {fullPath}");
                        return bitmap;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Image file not found: {fullPath}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading image {imagePath}: {ex.Message}");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}