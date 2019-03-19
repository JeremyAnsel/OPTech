using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OPTech
{
    static class ImageHelpers
    {
        public static ImageSource LoadImage(string filePath)
        {
            var image = new BitmapImage(new Uri(filePath));
            var format = new FormatConvertedBitmap(image, PixelFormats.Bgr32, null, 0);
            return new WriteableBitmap(format);
        }

        public static int GetBitsPerPixel(string filePath)
        {
            try
            {
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
            try
            {
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
            try
            {
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
    }
}
