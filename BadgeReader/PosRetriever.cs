using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Schema;

namespace BadgeReader
{
    public enum BadgeType
    {
        Small,
        Median,
        Large
    }

    public class Badge
    {
        public Position Position { get; set; }

        public BadgeType BadgeType { get; set; }
    }

    public class PosRetriever
    {
        public bool VerifyLargeBadge(int[,] processedMatrix, int col, int row)
        {
            try
            {
                if (col + 8 >= processedMatrix.GetLength(1) || row + 12 >= processedMatrix.GetLength(0))
                    return false;

                var invalidCells = 0;
                for (int x = col; x <= col + 4; ++x)
                {
                    for (int y = row - (x - col); y <= row + 8 + (x - col); ++y)
                    {
                        if (!IsCovered(processedMatrix[y, x]) && processedMatrix[y, x] != 7)
                            invalidCells++;
                    }
                }

                for (int x = col + 5; x <= col + 7; ++x)
                {
                    for (int y = row - (8 - x + col); y <= row + 8 + (8 - x + col); ++y)
                    {
                        if (!IsCovered(processedMatrix[y, x]) && processedMatrix[y, x] != 7)
                            invalidCells++;
                    }
                }

                if (invalidCells >= 2)
                    return false;

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        public bool VerifyMedianBadge(int[,] processedMatrix, int col, int row)
        {
            try
            {
                if (col + 6 >= processedMatrix.GetLength(1) || row + 9 >= processedMatrix.GetLength(0))
                    return false;

                var invalidCells = 0;
                for (int x = col; x <= col + 3; ++x)
                {
                    for (int y = row - (x - col); y <= row + 6 + (x - col); ++y)
                    {
                        if (!IsCovered(processedMatrix[y, x]) && processedMatrix[y, x] != 7)
                            invalidCells++;
                    }
                }

                for (int x = col + 4; x <= col + 5; ++x)
                {
                    for (int y = row - (6 - x + col); y <= row + 6 + (6 - x + col); ++y)
                    {
                        if (!IsCovered(processedMatrix[y, x]) && processedMatrix[y, x] != 7)
                            invalidCells++;
                    }
                }

                if (invalidCells >= 2)
                    return false;

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool VerifySmallBadge(int[,] processedMatrix, int col, int row)
        {
            try
            {
                if (col + 6 >= processedMatrix.GetLength(1) || row + 6 >= processedMatrix.GetLength(0))
                    return false;


                var invalidCells = 0;
                for (int x = col; x <= col + 2; ++x)
                {
                    for (int y = row - (x - col); y <= row + 4 + (x - col); ++y)
                    {
                        if (!IsCovered(processedMatrix[y, x]) && processedMatrix[y, x] != 7)
                            invalidCells++;
                    }
                }

                for (int x = col + 3; x <= col + 3; ++x)
                {
                    for (int y = row - (4 - x + col); y <= row + 4 + (4 - x + col); ++y)
                    {
                        if (!IsCovered(processedMatrix[y, x]) && processedMatrix[y, x] != 7)
                            invalidCells++;
                    }
                }

                if (invalidCells >= 2)
                    return false;

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public List<Badge> ReadDots(int[,] processedMatrix)
        {
            // 0 -> grey
            // 1/4 -> orange
            // 3 -> red
            // 2 -> green
            // 5 -> used
            // 6 -> shared / do not mark as invalid
            // 7 -> edged & used
            var badges = new List<Badge>();

            for (int x = 0; x < processedMatrix.GetLength(1); ++x)
            {
                for (int y = 0; y < processedMatrix.GetLength(0); ++y)
                {
                    if (x == 34 && y == 4)
                        Console.Write(true);
                    
                    if (IsCovered(processedMatrix[y, x]))
                    {
                        // start vertical scan
                        for (int k = y; k <= y + 8 && k < processedMatrix.GetLength(0); ++k)
                        {
                            if (k - y == 8)
                            {
                                if (VerifyLargeBadge(processedMatrix, x, y))
                                {
                                    var badge = new Badge()
                                        { BadgeType = BadgeType.Large, Position = new Position(x, y) };
                                    PutLargeBadge(processedMatrix, x, y);
                                    badges.Add(badge);
                                }
                                break;
                            }

                            if (!IsCovered(processedMatrix[k, x]))
                            {
                                if (k - y == 7)
                                {
                                    if (VerifyMedianBadge(processedMatrix, x, y))
                                    {
                                        var badge = new Badge()
                                            { BadgeType = BadgeType.Median, Position = new Position(x, y) };
                                        PutMedianBadge(processedMatrix, x, y);
                                        badges.Add(badge);
                                    }
                                }


                                if (k - y == 5)
                                {
                                    if (VerifySmallBadge(processedMatrix, x, y))
                                    {
                                        var badge = new Badge()
                                            { BadgeType = BadgeType.Small, Position = new Position(x, y) };
                                        PutSmallBadge(processedMatrix, x, y);
                                        badges.Add(badge);
                                    }
                                }

                                break;
                            }

                            //if (k > y && processedMatrix[k, x] == 6 && processedMatrix[y, x] != 6)
                            //{
                            //    if (k - y == 6)
                            //    {
                            //        if (VerifyMedianBadge(processedMatrix, x, y))
                            //        {
                            //            possibleBadge = new Badge()
                            //                { BadgeType = BadgeType.Median, Position = new Position(x, y) };
                            //        }
                            //    }


                            //    if (k - y == 4)
                            //    {
                            //        if (VerifySmallBadge(processedMatrix, x, y))
                            //        {
                            //            possibleBadge = new Badge()
                            //                { BadgeType = BadgeType.Small, Position = new Position(x, y) };
                            //        }
                            //    }
                            // }
                        }

                        //if (possibleBadge != null)
                        //{
                        //    if (possibleBadge.BadgeType == BadgeType.Median)
                        //    {
                        //        PutMedianBadge(processedMatrix, x, y);
                        //    }
                        //    else
                        //    {
                        //        PutSmallBadge(processedMatrix, x, y);
                        //    }
                        //    badges.Add(possibleBadge);
                        //}
                    }
                }
            }

            return badges;
        }

        public void PutMedianBadge(int[,] processedMatrix, int col, int row)
        {
            for (int y = row; y <= row + 5; ++y)
            {
                processedMatrix[y, col] = 5;
            }


            for (int x = col + 1; x <= col + 3; ++x)
            {
                for (int y = row - (x - col - 1); y <= row + 6 + (x - col - 1); ++y)
                {
                    processedMatrix[y, x] = 5;
                }
            }

            for (int x = col + 4; x <= col + 5; ++x)
            {
                for (int y = row - (5 - x + col); y <= row + 6 + (5 - x + col); ++y)
                {
                    processedMatrix[y, x] = 5;
                }
            }

            for (int y = row; y <= row + 6; ++y)
            {
                if (processedMatrix[y, col + 6] == 6)
                    processedMatrix[y, col + 6] = 7;
            }
        }

        public void PutSmallBadge(int[,] processedMatrix, int col, int row)
        {
            for (int y = row; y <= row + 3; ++y)
            {
                processedMatrix[y, col] = 5;
            }

            for (int x = col + 1; x <= col + 2; ++x)
            {
                for (int y = row - (x - col - 1); y <= row + 4 + (x - col - 1); ++y)
                {
                    processedMatrix[y, x] = 5;
                }
            }

            for (int x = col + 3; x <= col + 3; ++x)
            {
                for (int y = row - (3 - x + col); y <= row + 4 + (3 - x + col); ++y)
                {
                    processedMatrix[y, x] = 5;
                }
            }

            for (int y = row; y <= row + 4; ++y)
            {
                if (processedMatrix[y, col + 4] == 6)
                    processedMatrix[y, col + 4] = 7;
            }
        }



        public void PutLargeBadge(int[,] processedMatrix, int col, int row)
        {
            for (int y = row; y <= row + 7; ++y)
            {
                processedMatrix[y, col] = 5;
            }

            for (int x = col + 1; x <= col + 4; ++x)
            {
                for (int y = row - (x - col - 1); y <= row + 8 + (x - col - 1); ++y)
                {
                    processedMatrix[y, x] = 5;
                }
            }

            for (int x = col + 5; x <= col + 7; ++x)
            {
                for (int y = row - (7 - x + col); y <= row + 8 + (7 - x + col); ++y)
                {
                    processedMatrix[y, x] = 5;
                }
            }

            for (int y = row; y <= row + 8; ++y)
            {
                if (processedMatrix[y, col + 8] == 6)
                    processedMatrix[y, col + 8] = 7;
            }
        }

        public static bool IsCovered(int value)
        {
            return value == 1 || value == 4 || value == 6;
        }


        public int[,] PrintDots(Bitmap bitmap, int[,] mapMatrix)
        {
            int[,] colouredMapMatrix = new int[mapMatrix.GetLength(0), mapMatrix.GetLength(1)];

            var pixelHeight = (double) bitmap.Height / (mapMatrix.GetLength(0) - 1);
            var pixelWidth = (double) bitmap.Width / (mapMatrix.GetLength(1) - 1);
            var matrixGrey = bitmap.GetGrayScaleMatrix();

            for (int y = 0; y < mapMatrix.GetLength(0); ++y)
            for (int x = 0; x < mapMatrix.GetLength(1); ++x)
            {
                var posY = (int) Math.Round(y * pixelHeight, 0);
                var posX = (int) Math.Round(x * pixelWidth, 0);
                if (posX >= bitmap.Width)
                    posX = bitmap.Width - 1;
                if (posY >= bitmap.Height)
                    posY = bitmap.Height - 1;

                if (mapMatrix[y, x] == 1)
                {
                    int occupiedPixels = 0;
                    // check top
                    for (double m = posY - pixelHeight; m <= posY + pixelHeight; m++)
                    {
                        for (double n = posX - pixelWidth; n <= posX + pixelWidth; n++)
                        {
                            var posM = (int) (Math.Round(m, 0));
                            var posN = (int) (Math.Round(n, 0));
                            if (posM < 0 || posM >= bitmap.Height || posN < 0 || posN >= bitmap.Width)
                                continue;

                            if (matrixGrey[posM, posN] == 0)
                            {
                                occupiedPixels++;
                            }
                        }
                    }

                    if (occupiedPixels > (pixelHeight * pixelWidth * 4) * 0.075)
                    {
                        colouredMapMatrix[y, x] = 1; // occupied
                        bitmap.SetPixel(posX, posY, Color.Orange);
                    }
                    else
                    {
                        colouredMapMatrix[y, x] = 2; // unoccupied
                        bitmap.SetPixel(posX, posY, Color.GreenYellow);
                    }
                }
                else
                {
                    // unavailable
                    if (mapMatrix[y, x] == 0)
                    {
                        colouredMapMatrix[y, x] = 0; // not available
                        bitmap.SetPixel(posX, posY, Color.Gray);
                    }
                    else
                    {
                        colouredMapMatrix[y, x] = 3; // coverable
                        bitmap.SetPixel(posX, posY, Color.Red);
                    }
                }
            }


            var ratio = pixelHeight / pixelWidth;

            for (int y = 0; y < mapMatrix.GetLength(0); ++y)
            for (int x = 0; x < mapMatrix.GetLength(1); ++x)
            {
                var posY = (int) Math.Round(y * pixelHeight, 0);
                var posX = (int) Math.Round(x * pixelWidth, 0);
                if (posX >= bitmap.Width)
                    posX = bitmap.Width - 1;
                if (posY >= bitmap.Height)
                    posY = bitmap.Height - 1;

                if (y == 13 && x == 32)
                    Console.Write(true);

                if (mapMatrix[y, x] == 0)
                    continue;

                if (colouredMapMatrix[y, x] == 3)
                {
                    if ((y == 0 || y == mapMatrix.GetLength(0) - 1) && (x == 0 || x == mapMatrix.GetLength(1) - 1))
                        continue;

                    if (x == 0)
                    {
                        if (colouredMapMatrix[y + 1, x] == 1 && colouredMapMatrix[y - 1, x] == 1 &&
                            colouredMapMatrix[y, x + 1] == 1)
                        {
                            bitmap.SetPixel(posX, posY, Color.Orange);
                            colouredMapMatrix[y, x] = 4; // covered
                        }

                        continue;
                    }

                    if (x == mapMatrix.GetLength(1) - 1)
                    {
                        if (colouredMapMatrix[y + 1, x] == 1 &&
                            colouredMapMatrix[y - 1, x] == 1 &&
                            colouredMapMatrix[y, x - 1] == 1)
                        {
                            bitmap.SetPixel(posX, posY, Color.Orange);
                            colouredMapMatrix[y, x] = 4; // covered
                        }

                        continue;
                    }

                    if (y == 0 || (y == mapMatrix.GetLength(0) - 1))
                        continue;

                    if (colouredMapMatrix[y + 1, x] == 2 && colouredMapMatrix[y - 1, x] == 2 &&
                        colouredMapMatrix[y, x + 1] == 2 && colouredMapMatrix[y, x - 1] == 2)
                        continue;

                    int occupiedPixelsTopLeft = 0;
                    // Top Left Triangle
                    for (double m = posY - pixelHeight; m < posY; m++)
                    {
                        for (double n = posX - pixelWidth; n < posX; n++)
                        {
                            if (Math.Abs((m - posY) / (n - posX + pixelWidth)) > ratio)
                                continue; // Not in triangle

                            var posM = (int) (Math.Round(m, 0));
                            var posN = (int) (Math.Round(n, 0));
                            if (posM < 0 || posM >= bitmap.Height || posN < 0 || posN >= bitmap.Width)
                                continue;

                            if (matrixGrey[posM, posN] == 0)
                            {
                                occupiedPixelsTopLeft++;
                            }
                        }
                    }

                    if (occupiedPixelsTopLeft > (pixelHeight * pixelWidth) * 0.25)
                    {
                        bitmap.SetPixel(posX, posY, Color.Orange);
                        colouredMapMatrix[y, x] = 4; // covered
                        continue;
                    }

                    // Top Right Triangle
                    int occupiedPixelsTopRight = 0;
                    for (double m = posY - pixelHeight; m < posY; m++)
                    {
                        for (double n = posX + 1; n < posX + pixelWidth; n++)
                        {
                            if (Math.Abs((m - posY) / (n - posX - pixelWidth)) > ratio)
                                continue; // Not in triangle

                            var posM = (int) (Math.Round(m, 0));
                            var posN = (int) (Math.Round(n, 0));
                            if (posM < 0 || posM >= bitmap.Height || posN < 0 || posN >= bitmap.Width)
                                continue;

                            if (matrixGrey[posM, posN] == 0)
                            {
                                occupiedPixelsTopRight++;
                            }
                        }
                    }

                    if (occupiedPixelsTopRight > (pixelHeight * pixelWidth) * 0.25)
                    {
                        bitmap.SetPixel(posX, posY, Color.Orange);
                        colouredMapMatrix[y, x] = 4; // covered
                        continue;
                    }

                    // Bottom Left Triangle
                    int occupiedPixelsBottomLeft = 0;
                    for (double m = posY + 1; m < posY + pixelHeight; m++)
                    {
                        for (double n = posX - pixelWidth; n < posX; n++)
                        {
                            if (Math.Abs((m - posY) / (n - posX + pixelWidth)) > ratio)
                                continue; // Not in triangle

                            var posM = (int) (Math.Round(m, 0));
                            var posN = (int) (Math.Round(n, 0));
                            if (posM < 0 || posM >= bitmap.Height || posN < 0 || posN >= bitmap.Width)
                                continue;

                            if (matrixGrey[posM, posN] == 0)
                            {
                                occupiedPixelsBottomLeft++;
                            }
                        }
                    }

                    if (occupiedPixelsBottomLeft > (pixelHeight * pixelWidth) * 0.25)
                    {
                        bitmap.SetPixel(posX, posY, Color.Orange);
                        colouredMapMatrix[y, x] = 4; // covered
                        continue;
                    }


                    // Bottom Right Triangle
                    int occupiedPixelsBottomRight = 0;
                    for (double m = posY + 1; m < posY + pixelHeight; m++)
                    {
                        for (double n = posX + 1; n < posX + pixelWidth; n++)
                        {
                            if (Math.Abs((m - posY) / (n - posX - pixelWidth)) > ratio)
                                continue; // Not in triangle

                            var posM = (int) (Math.Round(m, 0));
                            var posN = (int) (Math.Round(n, 0));
                            if (posM < 0 || posM >= bitmap.Height || posN < 0 || posN >= bitmap.Width)
                                continue;

                            if (matrixGrey[posM, posN] == 0)
                            {
                                occupiedPixelsBottomRight++;
                            }
                        }
                    }

                    if (occupiedPixelsBottomRight > (pixelHeight * pixelWidth) * 0.25)
                    {
                        bitmap.SetPixel(posX, posY, Color.Orange);
                        colouredMapMatrix[y, x] = 4; // covered
                        continue;
                    }

                    if (occupiedPixelsTopLeft + occupiedPixelsBottomRight + occupiedPixelsBottomLeft +
                        occupiedPixelsBottomRight > (pixelHeight * pixelWidth * 4) * 0.0445)
                    {
                        bitmap.SetPixel(posX, posY, Color.Orange);
                        colouredMapMatrix[y, x] = 4; // covered
                    }
                }
            }

            for (int y = 1; y < mapMatrix.GetLength(0); ++y)
            for (int x = 4; x < mapMatrix.GetLength(1); ++x)
            {
                var posY = (int) Math.Round(y * pixelHeight, 0);
                var posX = (int) Math.Round(x * pixelWidth, 0);
                if (posX >= bitmap.Width)
                    posX = bitmap.Width - 1;
                if (posY >= bitmap.Height)
                    posY = bitmap.Height - 1;


                if (IsCovered(colouredMapMatrix[y, x]))
                {
                    int occupiedPixelsTopRight = 0;
                    for (double m = posY - pixelHeight; m < posY; m++)
                    {
                        for (double n = posX + 1; n < posX + pixelWidth; n++)
                        {
                            if (Math.Abs((m - posY) / (n - posX - pixelWidth)) > ratio)
                                continue; // Not in triangle

                                var posM = (int)(Math.Round(m, 0));
                            var posN = (int)(Math.Round(n, 0));
                            if (posM < 0 || posM >= bitmap.Height || posN < 0 || posN >= bitmap.Width)
                                continue;

                            if (matrixGrey[posM, posN] == 0)
                            {
                                occupiedPixelsTopRight++;
                            }
                        }
                    }

                    if (x == 34 && y == 11)
                        Console.Write(true);
                        // Bottom Right Triangle
                        int occupiedPixelsBottomRight = 0;
                    for (double m = posY + 1; m < posY + pixelHeight; m++)
                    {
                        for (double n = posX + 1; n < posX + pixelWidth; n++)
                        {
                            if (Math.Abs((m - posY) / (n - posX - pixelWidth)) > ratio)
                                continue; // Not in triangle

                            var posM = (int)(Math.Round(m, 0));
                            var posN = (int)(Math.Round(n, 0));
                            if (posM < 0 || posM >= bitmap.Height || posN < 0 || posN >= bitmap.Width)
                                continue;

                            if (matrixGrey[posM, posN] == 0)
                            {
                                occupiedPixelsBottomRight++;
                            }
                        }
                    }


                   if (occupiedPixelsTopRight + occupiedPixelsBottomRight < (pixelHeight * pixelWidth) * 0.25)
                    {
                        bitmap.SetPixel(posX, posY, Color.Blue);
                        colouredMapMatrix[y, x] = 6; // shared
                    }
                }
            }



            bitmap.Save(@"C:\Users\qqytqqyt\OneDrive\Documents\OneDrive\OwnProjects\Combination\freetest\points.jpg");
            return colouredMapMatrix;
        }
        
        public Bitmap RetrieveFreePositions(string filePath)
        {
            using (var bitMap =
                new Bitmap(filePath))
            {
                using (var bitMapGray = bitMap.ToGrayscale())
                {
                    bitMapGray.Save(
                        @"C:\Users\qqytqqyt\OneDrive\Documents\OneDrive\OwnProjects\Combination\freetest\gray.jpg");

                    var panelTopX = 0;
                    var panelTopY = 0;
                    var panelWidth = 0;
                    var panelHeight = 0;
                    using (var bitMapBi = bitMapGray.Binarilization(32))
                    {
                        bitMapBi.Save(
                            @"C:\Users\qqytqqyt\OneDrive\Documents\OneDrive\OwnProjects\Combination\freetest\123.jpg");

                        var matrixGrey = bitMapBi.GetGrayScaleMatrix();

                        var width = matrixGrey.GetLength(1);
                        var height = matrixGrey.GetLength(0);

                        var boardTopRightX = 0;
                        var boardTopRightY = 0;
                        var boardHeight = 0;
                        for (int x = width - 1; x >= width * 0.7; --x)
                        {
                            if (x == 1877)
                                Console.Write(true);
                            var currentPoints = 0;
                            var currentY = 0;
                            for (int y = (int) (height * 0.5); y < height; ++y)
                            {
                                int grayScale = matrixGrey[y, x];
                                if (grayScale != 0)
                                {
                                    currentPoints++;
                                }
                                else if (currentPoints > 0)
                                {
                                    currentY = y;
                                    break;
                                }
                            }

                            if (currentPoints > height * 0.1)
                            {
                                boardTopRightY = currentY - currentPoints;
                                boardTopRightX = x;
                                boardHeight = currentPoints;
                                break;
                            }
                        }

                        var boardHeightConfirmed = 0;
                        for (int y = (int)(height * 0.5); y < height; ++y)
                        {
                            int grayScale = matrixGrey[y, boardTopRightX - 1];
                            if (grayScale != 0)
                            {
                                boardHeightConfirmed++;
                            }
                            else if (boardHeightConfirmed > 0)
                            {
                                if (boardHeightConfirmed > boardHeight)
                                {
                                    boardTopRightY = y - boardHeightConfirmed;
                                    boardHeight = boardHeightConfirmed;
                                }
                                break;
                            }
                        }


                        panelTopX = (int) Math.Round(boardTopRightX - boardHeight * 2.693, 0);
                        panelTopY = (int) Math.Round(boardTopRightY + boardHeight * 0.026, 0);
                        panelHeight = (int) Math.Round(boardHeight * 0.947, 0);
                        panelWidth = (int) Math.Round(boardHeight * 2.5675 + 2, 0);
                    }

                    using (var cropped = bitMapGray.CropAtRect(new Rectangle(panelTopX,
                        panelTopY, panelWidth, panelHeight)))
                    {
                        var bitMapBi = cropped.Binarilization(75, 55, panelWidth / 25);
                        bitMapBi.Save(
                                @"C:\Users\qqytqqyt\OneDrive\Documents\OneDrive\OwnProjects\Combination\freetest\croppedBi.jpg");


                        return bitMapBi;
                    }
                }
            }
        }

        #region obsolete

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
                                for (int x = (int)(minCol); x < cropped.Width; ++x)
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

        #endregion

    }
}
