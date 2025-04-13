using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MemoryGame.Helpers
{
    public static class ImageHelper
    {
        /// <summary>
        /// Attempts to load an image from a file path and returns a BitmapImage if successful.
        /// </summary>
        /// <param name="imagePath">The path to the image file</param>
        /// <returns>A BitmapImage or null if the image could not be loaded</returns>
        public static BitmapImage TryLoadImage(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                    return null;

                if (!File.Exists(imagePath))
                {
                   
                    string fullPath = Path.GetFullPath(imagePath);
                    if (!File.Exists(fullPath))
                    {
                        Console.WriteLine($"Image file not found: {imagePath}");
                        return null;
                    }
                    imagePath = fullPath;
                }

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                bitmap.EndInit();
                bitmap.Freeze(); 
                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading image {imagePath}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Checks if a file path exists and returns diagnostic information about it.
        /// </summary>
        /// <param name="imagePath">The path to check</param>
        /// <returns>A string with diagnostic information</returns>
        public static string GetImagePathInfo(string imagePath)
        {
            string info = $"Image Path: {imagePath}\n";

            try
            {
                if (string.IsNullOrEmpty(imagePath))
                {
                    info += "Path is null or empty";
                    return info;
                }

               
                bool isAbsolute = Path.IsPathRooted(imagePath);
                info += $"Is absolute path: {isAbsolute}\n";

               
                string fullPath = isAbsolute ? imagePath : Path.GetFullPath(imagePath);
                info += $"Full path: {fullPath}\n";

               
                bool fileExists = File.Exists(fullPath);
                info += $"File exists: {fileExists}\n";

                if (fileExists)
                {
                   
                    FileInfo fileInfo = new FileInfo(fullPath);
                    info += $"File size: {fileInfo.Length / 1024.0:F2} KB\n";
                    info += $"Last modified: {fileInfo.LastWriteTime}\n";

                  
                    try
                    {
                        using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                            info += $"Image format: {decoder.CodecInfo.FriendlyName}\n";
                            info += $"Image dimensions: {decoder.Frames[0].Width}x{decoder.Frames[0].Height}\n";
                        }
                    }
                    catch
                    {
                        info += "Not a valid image file or unable to read image format\n";
                    }
                }
                else
                {
                    
                    string directory = Path.GetDirectoryName(fullPath);
                    bool directoryExists = Directory.Exists(directory);
                    info += $"Parent directory exists: {directoryExists}\n";

                    if (directoryExists)
                    {
                        int fileCount = Directory.GetFiles(directory).Length;
                        info += $"Files in directory: {fileCount}\n";
                    }
                }
            }
            catch (Exception ex)
            {
                info += $"Error analyzing path: {ex.Message}";
            }

            return info;
        }
    }
}