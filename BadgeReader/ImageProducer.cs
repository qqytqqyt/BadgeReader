using System;
using System.Collections.Generic;
using System.Drawing;

namespace BadgeReader
{
    public class ImageProducer
    {
        private readonly Random m_rnd = new Random();

        public Bitmap ProduceImage(List<Badge> badges, Bitmap originalBitMap)
        {
            if (PosRetriever.Debug)
                originalBitMap.Save(PosRetriever.DebugDir + @"\origin.jpg");
            var positions = new List<List<Position>>();
            var outputImg = new Bitmap(originalBitMap);
            int matrixY = 0;
            for (int y = 0; y < outputImg.Height; ++y)
            {
                int matrixX = 0;
                for (int x = 0; x < outputImg.Width; ++x, ++matrixX)
                {
                    var originalColour = originalBitMap.GetPixel(x, y);
                    if (!IsBlack(originalColour) && !IsWhite(originalColour))
                    {
                        outputImg.SetPixel(x, y, Color.Blue);
                        if (matrixX == 0)
                        {
                            positions.Add(new List<Position>());
                            matrixY++;
                        }

                        positions[matrixY - 1].Add(new Position(x, y));
                        matrixX++;
                    }
                    else
                    {
                        outputImg.SetPixel(x, y, Color.White);
                    }
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
