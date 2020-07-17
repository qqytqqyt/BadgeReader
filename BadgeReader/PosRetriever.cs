using System;
using System.Collections.Generic;
using System.Drawing;

namespace BadgeReader
{
    public class PosRetriever
    {
        public List<Position> RetrieveCroppedImg(string filePath)
        {
            using (var bitMap =
               new Bitmap(filePath))
            {
                using (var bitMapGray = bitMap.ToGrayscale())
                {
                    using (var bitMapBi = bitMapGray.Binarilization(55))
                    {
                        using (var cropped = bitMapBi.CropAtRect(new Rectangle((int)(bitMapBi.Width * 0.5337),
                            (int)(bitMapBi.Height * 0.6529), (int)(bitMapBi.Width * 0.4198),
                            (int)(bitMapBi.Height * 0.2907))))
                        {
                            var minRow = 0;
                            var matrixGrey = cropped.GetGrayScaleMatrix();


                            var minCol = 0;
                            for (int x = 0; x < 0.3 * cropped.Width; ++x)
                            {
                                var count = 0;
                                for (int y = minRow + 1; y < cropped.Height; ++y)
                                {
                                    int grayScale = matrixGrey[y, x];
                                    if (grayScale != 255)
                                    {
                                        minCol = x;
                                        count++;
                                        break;
                                    }
                                }

                                if (count == 0)
                                    break;
                            }


                            for (int y = minCol; y < 0.05 * cropped.Height; ++y)
                            {
                                for (int x = minCol; x < cropped.Width; ++x)
                                {
                                    int grayScale = matrixGrey[y, x];
                                    if (grayScale != 255)
                                    {
                                        minRow = y;
                                        break;
                                    }
                                }
                            }

                            for (int y = minRow; y < 0.5 * cropped.Height; ++y)
                            {
                                var finish = false;
                                for (int x = (int) (minCol); x < cropped.Width; ++x)
                                {
                                    int grayScale = matrixGrey[y, x];
                                    if (grayScale != 255)
                                    {
                                        int badPixels = 0;
                                        for (int yy = y; yy < y + 15 && yy < cropped.Height; ++yy)
                                        {
                                            for (int xx = x; xx < x + 20 && xx < cropped.Width; ++xx)
                                            {
                                                int grayScale2 = matrixGrey[yy, xx];
                                                if (grayScale2 != 255)
                                                    badPixels++;
                                            }
                                        }
                                        
                                        if ((double)badPixels / 300 < 0.2)
                                            minRow = y;
                                        else
                                            finish = true;
                                            

                                        break;
                                    }
                                }

                                if (finish)
                                    break;
                            }

                            using (var final =
                                cropped.CropAtRect(new Rectangle(minCol + 1, minRow + 1, cropped.Width - minCol - 1, cropped.Height - minRow - 1)))
                            {
                                var matrix = final.GetGrayScaleMatrix();
                                // find longest vertical line
                                var maxHeight = 0;
                                int currentX = 0;
                                int pointY = 0;
                                for (int x = 0; x < matrix.GetLength(1) / 6; ++x)
                                {
                                    if (x > 94)
                                        Console.Write(true);
                                    var currentHeight = 0;
                                    for (int y = 0; y < matrix.GetLength(0); ++y)
                                    {
                                        int grayScale = matrix[y, x];
                                        if (grayScale != 255)
                                        {
                                            currentHeight++;
                                        }
                                        else
                                        {
                                            if (currentHeight != 0)
                                            {
                                                if (currentHeight >= maxHeight)
                                                    pointY = y - currentHeight;

                                                break;
                                            }
                                        }
                                    }

                                    if (currentHeight == 0 && maxHeight > 10)
                                    {
                                        break;
                                    };

                                    if (currentHeight >= maxHeight)
                                    {
                                        maxHeight = currentHeight;
                                        currentX = x;
                                    }
                                }

                                maxHeight = maxHeight + 2;

                                var startX = currentX;
                                var startY = pointY;
                                double pointHeight = (double)maxHeight / 4;
                                double pointWidth = (double)(maxHeight * 1.675 + 2) / 4;


                                var results = new List<Position>();

                                int previousX = startX;
                                int previousY = startY;
                                int lastPosX = 0;
                                int lastPosY = 2;
                                for (int i = 0; i < 6; ++i)
                                {
                                    pointY = GetNextBadgePosition(matrix, ref currentX);

                                    int badgePosX = (int)(Math.Round((lastPosX + (currentX - previousX) / pointWidth), 0));
                                    int badgePosY = (int)(Math.Round(lastPosY + (pointY - previousY) / pointHeight, 0));
                                    previousX = currentX;
                                    previousY = pointY;
                                    lastPosX = badgePosX;
                                    lastPosY = badgePosY;

                                    results.Add(new Position(badgePosX, badgePosY));
                                }
                              
                                return results;
                            }

                        }
                    }

                }
            }

        }

        private static int GetNextBadgePosition(int[,] matrix, ref int currentX)
        {
            var maxHeight = 0;
            var pointY = 0;
            for (int x = currentX + 3; x < matrix.GetLength(1); ++x)
            {
                var currentHeight = 0;
                for (int y = 0; y < matrix.GetLength(0); ++y)
                {
                    int grayScale = matrix[y, x];
                    if (grayScale != 255)
                    {
                        currentHeight++;
                    }
                    else
                    {
                        if (currentHeight != 0)
                        {
                            if (currentHeight + 2 >= maxHeight)
                            {
                                pointY = y - currentHeight;
                            }

                            break;
                        }
                    }
                }

                if (currentHeight == 0 && maxHeight > 10)
                    break;

                if (currentHeight + 2 >= maxHeight)
                {
                    maxHeight = currentHeight;
                    currentX = x;
                }
            }

            return pointY;
        }
    }
}
