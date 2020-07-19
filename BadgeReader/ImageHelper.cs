using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BadgeReader
{
    public static class ImageHelper
    {

        public static Color GetPixel(this FastBitmap fbitmap, int y, int x)
        {
            unsafe
            {
                byte* row = (byte*)fbitmap.Scan0, bb = row;
                for (var yy = 0; yy < y; yy++, bb = row += fbitmap.Stride)
                {
                    for (var xx = 0; xx < x; xx++, bb += fbitmap.PixelSize)
                    {
                        // *(bb + 0) is B (Blue ) component of the pixel
                        // *(bb + 1) is G (Green) component of the pixel
                        // *(bb + 2) is R (Red  ) component of the pixel
                        // *(bb + 3) is A (Alpha) component of the pixel ( for 32bpp )
                    }
                }

                return Color.FromArgb(*(bb + 0), *(bb + 1), *(bb + 2));
            }
        }

        public static Bitmap ToGrayscale(this Bitmap original)
        {
            //create a blank bitmap the same size as original
            var bmp = new Bitmap(original);

            using (var fbitmap = new FastBitmap(bmp, 0, 0, bmp.Width, bmp.Height))
            {
                unsafe
                {
                    byte* row = (byte*)fbitmap.Scan0, bb = row;
                    for (var yy = 0; yy < fbitmap.Height; yy++, bb = row += fbitmap.Stride)
                    {
                        for (var xx = 0; xx < fbitmap.Width; xx++, bb += fbitmap.PixelSize)
                        {
                            // *(bb + 0) is B (Blue ) component of the pixel
                            // *(bb + 1) is G (Green) component of the pixel
                            // *(bb + 2) is R (Red  ) component of the pixel
                            // *(bb + 3) is A (Alpha) component of the pixel ( for 32bpp )
                            var gray = (byte)((1140 * *(bb + 0) + 5870 * *(bb + 1) + 2989 * *(bb + 2)) / 10000);
                            *(bb + 0) = *(bb + 1) = *(bb + 2) = gray;
                        }
                    }
                }
            }
            return bmp;
        }

        public static int[,] GetGrayScaleMatrix(this Bitmap original)
        {
            var matrix = new int[original.Height, original.Width];
            using (var fbitmap = new FastBitmap(original, 0, 0, original.Width, original.Height))
            {
                unsafe
                {
                    byte* row = (byte*)fbitmap.Scan0, bb = row;
                    for (var yy = 0; yy < fbitmap.Height; yy++, bb = row += fbitmap.Stride)
                    {
                        for (var xx = 0; xx < fbitmap.Width; xx++, bb += fbitmap.PixelSize)
                        {
                            var gray = (byte)((1140 * *(bb + 0) + 5870 * *(bb + 1) + 2989 * *(bb + 2)) / 10000);
                            if (gray == 254)
                                gray = 255;
                            matrix[yy, xx] = gray;
                        }
                    }
                }
            }

            return matrix;
        }


        public static Bitmap CropAtRect(this Bitmap b, Rectangle r)
        {
            var nb = new Bitmap(r.Width, r.Height);
            using (var g = Graphics.FromImage(nb))
                g.DrawImage(b, -r.X, -r.Y);
            return nb;
        }

        public static Bitmap Binarilization(this Bitmap grayScaledBitmap, int threshold)
        {
            var bmp = new Bitmap(grayScaledBitmap); // new Bitmap(grayScaledBitmap.Width, grayScaledBitmap.Height);
            using (var fbitmap = new FastBitmap(bmp, 0, 0, bmp.Width, bmp.Height))
            {
                unsafe
                {
                    byte* row = (byte*)fbitmap.Scan0, bb = row;
                    for (var yy = 0; yy < fbitmap.Height; yy++, bb = row += fbitmap.Stride)
                    {
                        for (var xx = 0; xx < fbitmap.Width; xx++, bb += fbitmap.PixelSize)
                        {
                            var gray = (byte)((1140 * *(bb + 0) + 5870 * *(bb + 1) + 2989 * *(bb + 2)) / 10000);
                            *(bb + 0) = *(bb + 1) = *(bb + 2) = (byte)(gray > threshold ? 0 : 255);
                        }
                    }
                }
            }
            return bmp;
        }

        public static Bitmap Binarilization(this Bitmap grayScaledBitmap, int thresholdLeft, int thresholdRight, int thresholdX)
        {
            var bmp = new Bitmap(grayScaledBitmap); // new Bitmap(grayScaledBitmap.Width, grayScaledBitmap.Height);
            using (var fbitmap = new FastBitmap(bmp, 0, 0, bmp.Width, bmp.Height))
            {
                unsafe
                {
                    byte* row = (byte*)fbitmap.Scan0, bb = row;
                    for (var yy = 0; yy < fbitmap.Height; yy++, bb = row += fbitmap.Stride)
                    {
                        for (var xx = 0; xx < fbitmap.Width; xx++, bb += fbitmap.PixelSize)
                        {
                            int threshold = xx < thresholdX ? thresholdLeft : thresholdRight;
                            var gray = (byte)((1140 * *(bb + 0) + 5870 * *(bb + 1) + 2989 * *(bb + 2)) / 10000);
                            *(bb + 0) = *(bb + 1) = *(bb + 2) = (byte)(gray > threshold ? 0 : 255);
                        }
                    }
                }
            }
            return bmp;
        }

        public static Bitmap ReverseBinarilization(this Bitmap grayScaledBitmap, int threshold, bool textInWhite = false)
        {
            var bmp = new Bitmap(grayScaledBitmap); // new Bitmap(grayScaledBitmap.Width, grayScaledBitmap.Height);
            using (var fbitmap = new FastBitmap(bmp, 0, 0, bmp.Width, bmp.Height))
            {
                unsafe
                {
                    byte* row = (byte*)fbitmap.Scan0, bb = row;
                    for (var yy = 0; yy < fbitmap.Height; yy++, bb = row += fbitmap.Stride)
                    {
                        for (var xx = 0; xx < fbitmap.Width; xx++, bb += fbitmap.PixelSize)
                        {
                            var gray = (byte)((1140 * *(bb + 0) + 5870 * *(bb + 1) + 2989 * *(bb + 2)) / 10000);
                            *(bb + 0) = *(bb + 1) = *(bb + 2) = (byte)(gray > threshold ? (textInWhite ? 0 : 255) : (textInWhite ? 255 : 0));
                        }
                    }
                }
            }
            return bmp;
        }
    }
}
