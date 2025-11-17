using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OPTech
{
    static class ImageHelpers
    {
        private static readonly string[] _imageExtensions = new string[] { ".bmp", ".png", ".jpg" };

        public static bool IsImageFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            if (filePath.StartsWith("pack://"))
            {
                return true;
            }

            return _imageExtensions.Contains(System.IO.Path.GetExtension(filePath), StringComparer.OrdinalIgnoreCase);
        }

        public static bool ImageFilePathExists(string filePath)
        {
            return !string.IsNullOrEmpty(GetExistingImageFilePath(filePath));
        }

        public static string GetExistingImageFilePath(string filePath)
        {
            if (!IsImageFilePath(filePath))
            {
                return null;
            }

            if (filePath.StartsWith("pack://"))
            {
                return filePath;
            }

            if (System.IO.File.Exists(filePath))
            {
                return filePath;
            }

            foreach (string extension in _imageExtensions)
            {
                string path = System.IO.Path.ChangeExtension(filePath, extension);

                if (System.IO.File.Exists(path))
                {
                    return path;
                }
            }

            return null;
        }

        public static IEnumerable<string> EnumerateImages(string path)
        {
            IEnumerable<string> images = System.IO.Directory
                .EnumerateFiles(Global.opzpath, "*.*")
                .Where(t => IsImageFilePath(t))
                .Select(t => System.IO.Path.GetFileName(t));

            return images;
        }

        public static ImageSource LoadImage(string filePath)
        {
            filePath = GetExistingImageFilePath(filePath);
            var image = new BitmapImage(new Uri(filePath));
            var format = new FormatConvertedBitmap(image, PixelFormats.Bgr32, null, 0);
            return new WriteableBitmap(format);
        }

        public static int GetBitsPerPixel(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return 0;
            }

            try
            {
                filePath = GetExistingImageFilePath(filePath);
                var image = new BitmapImage(new Uri(filePath));
                return image.Format.BitsPerPixel;
            }
            catch (System.IO.FileFormatException)
            {
                return 0;
            }
        }

        public static int GetPixelWidth(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return 0;
            }

            try
            {
                filePath = GetExistingImageFilePath(filePath);
                var image = new BitmapImage(new Uri(filePath));
                return image.PixelWidth;
            }
            catch (System.IO.FileFormatException)
            {
                return 0;
            }
        }

        public static int GetPixelWidth(this ImageSource image)
        {
            var bitmap = (BitmapSource)image;
            return bitmap.PixelWidth;
        }

        public static int GetPixelHeight(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return 0;
            }

            try
            {
                filePath = GetExistingImageFilePath(filePath);
                var image = new BitmapImage(new Uri(filePath));
                return image.PixelHeight;
            }
            catch (System.IO.FileFormatException)
            {
                return 0;
            }
        }

        public static int GetPixelHeight(this ImageSource image)
        {
            var bitmap = (BitmapSource)image;
            return bitmap.PixelHeight;
        }

        public static Color CopyPixel(this ImageSource image, int x, int y)
        {
            var bitmap = (WriteableBitmap)image;

            if (x < 0 || x >= bitmap.PixelWidth || y < 0 || y >= bitmap.PixelHeight)
            {
                return Colors.Black;
            }

            byte[] pixel = new byte[4];
            bitmap.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, bitmap.PixelWidth * 4, 0);

            return Color.FromRgb(pixel[2], pixel[1], pixel[0]);
        }

        public static void WritePixel(this ImageSource image, int x, int y, Color color)
        {
            var bitmap = (WriteableBitmap)image;

            if (x < 0 || x >= bitmap.PixelWidth || y < 0 || y >= bitmap.PixelHeight)
            {
                return;
            }

            byte[] pixel = new byte[4];
            pixel[0] = color.B;
            pixel[1] = color.G;
            pixel[2] = color.R;

            bitmap.WritePixels(new Int32Rect(x, y, 1, 1), pixel, bitmap.PixelWidth * 4, 0);
        }

        public static Tuple<int, int> GetMousePosition(this Image image, MouseEventArgs e)
        {
            var source = (BitmapSource)image.Source;
            Point position = e.GetPosition(image);
            int positionX = Math.Max(Math.Min((int)(position.X * source.PixelWidth / image.ActualWidth), source.PixelWidth - 1), 0);
            int positionY = Math.Max(Math.Min((int)(position.Y * source.PixelHeight / image.ActualHeight), source.PixelHeight - 1), 0);
            return Tuple.Create(positionX, positionY);
        }

        public static System.IO.Stream GetFileStream(string filePath)
        {
            filePath = GetExistingImageFilePath(filePath);

            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            if (filePath.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) && GetBitsPerPixel(filePath) == 8)
            {
                return new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            }

            using var file = new System.Drawing.Bitmap(filePath);
            var rectangle = new System.Drawing.Rectangle(0, 0, file.Width, file.Height);
            using var bitmap = file.Clone(rectangle, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var ms = new System.IO.MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Seek(0, System.IO.SeekOrigin.Begin);
            return ms;
        }
    }
}
