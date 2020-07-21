using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using BadgeReader.Properties;

namespace BadgeReader
{
    public class ImageProducer
    {
        private readonly Random m_rnd = new Random();

        public Bitmap ProduceImage(List<Badge> badges, int[,] mapMatrix)
        {
            var positions = new List<List<Position>>();
            using (Stream stream = new MemoryStream(Resources.Origin))
            {
                var outputImg = new Bitmap(stream);
                var pixelHeight = (double)outputImg.Height / (mapMatrix.GetLength(0) - 1);
                var pixelWidth = (double)outputImg.Width / (mapMatrix.GetLength(1) - 1);
                int matrixY = 0;
                for (var y = 0; y < mapMatrix.GetLength(0); ++y)
                {
                    int matrixX = 0;
                    for (var x = 0; x < mapMatrix.GetLength(1); ++x, ++matrixX)
                    {
                        var posY = (int)Math.Round(y * pixelHeight, 0);
                        var posX = (int)Math.Round(x * pixelWidth, 0);
                        if (posX >= outputImg.Width)
                            posX = outputImg.Width - 1;
                        if (posY >= outputImg.Height)
                            posY = outputImg.Height - 1;

                        if (matrixX == 0)
                        {
                            positions.Add(new List<Position>());
                            matrixY++;
                        }

                        positions[matrixY - 1].Add(new Position(posX, posY));
                    }

                }

                foreach (var badge in badges)
                {
                    if (badge.BadgeType == BadgeType.Large)
                    {
                        var col = badge.Position.X;
                        var row = badge.Position.Y;
                        WriteDots(positions, outputImg, col, row, 8);
                    }
                    if (badge.BadgeType == BadgeType.Median)
                    {
                        var col = badge.Position.X;
                        var row = badge.Position.Y;
                        WriteDots(positions, outputImg, col, row, 6);
                    }
                    if (badge.BadgeType == BadgeType.Small)
                    {
                        var col = badge.Position.X;
                        var row = badge.Position.Y;
                        WriteDots(positions, outputImg, col, row, 4);
                    }
                }

                return outputImg;
            }
               
        }
        
        private void WriteDots(List<List<Position>> positions, Bitmap outputImg, int col, int row, int size)
        {
            var randomColor = Color.FromArgb(m_rnd.Next(256), m_rnd.Next(256), m_rnd.Next(256));

            for (var x = positions[row][col].X; x <= positions[row][col + size].X; ++x)
            {
                for (var y = positions[row][col].Y; y <= positions[row + size][col].Y; ++y)
                {
                    outputImg.SetPixel(x, y, randomColor);
                }
            }

            var deltaX = positions[row][col + 1].X - positions[row][col].X;
            var deltaY = positions[row + 1][col].Y - positions[row][col].Y;
            var ratio = (double)deltaY / deltaX;

            var startX = positions[row][col].X;
            var startY = positions[row][col].Y;
            for (var y = positions[row - size / 2][col].Y; y < positions[row][col].Y; ++y)
            {
                for (var x = positions[row][col].X + 1 ; x <= positions[row][col + size / 2].X; ++x)
                {
                    if (Math.Abs((double)(y - startY) / (x - startX)) <= ratio)
                        outputImg.SetPixel(x, y, randomColor);
                }
            }

            startY = positions[row + size][col].Y;

            for (var y = positions[row + size][col].Y + 1; y < positions[row + size + size / 2][col].Y; ++y)
            {
                for (var x = positions[row][col].X + 1; x <= positions[row][col + size / 2].X; ++x)
                {
                    if (Math.Abs((double)(y - startY) / (x - startX)) <= ratio)
                        outputImg.SetPixel(x, y, randomColor);
                }
            }

            startX = positions[row][col + size].X;

            startY = positions[row][col].Y;
            for (var y = positions[row - size / 2][col].Y; y < positions[row][col].Y; ++y)
            {
                for (var x = positions[row][col + size / 2].X; x < positions[row][col + size].X; ++x)
                {
                    if (Math.Abs((double)(y - startY) / (x - startX)) <= ratio)
                        outputImg.SetPixel(x, y, randomColor);
                }
            }

            startY = positions[row + size][col].Y;
            for (var y = positions[row + size][col].Y + 1; y < positions[row + size + size / 2][col].Y; ++y)
            {
                for (var x = positions[row][col + size / 2].X; x < positions[row][col + size].X; ++x)
                {
                    if (Math.Abs((double)(y - startY) / (x - startX)) <= ratio)
                        outputImg.SetPixel(x, y, randomColor);
                }
            }
        }

        private static bool IsBlack(Color colour)
        {
            return colour.R == 0 && colour.G == 0 && colour.B == 0;
        }

        private static bool IsWhite(Color colour)
        {
            return colour.R == 255 && colour.G == 255 && colour.B == 255;
        }
    }
}
