using System;
using System.Collections.Generic;
using System.Drawing;

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

    public class PointsResult
    {
        public Bitmap Image { get; set; }

        public int[,] DotMatrix { get; set; }
    }

    public class PosRetriever
    {
        public bool Debug { get; set; }

        private string DebugDir { get; } =
            @"C:\Users\qqytqqyt\OneDrive\Documents\OneDrive\OwnProjects\Combination\freetest\";

        private static bool VerifyBadge(int[,] processedMatrix, int col, int row, int size)
        {
            try
            {
                col = col - size;
                if (col < 0 || col + size >= processedMatrix.GetLength(1) ||
                    row + size * 1.5 >= processedMatrix.GetLength(0))
                    return false;

                var invalidCells = 0;
                for (var x = col; x <= col + size / 2; ++x)
                for (var y = row - (x - col); y <= row + size + (x - col); ++y)
                    if (!IsCovered(processedMatrix[y, x]) && processedMatrix[y, x] != 7)
                        invalidCells++;

                for (var x = col + size / 2 + 1; x <= col + size; ++x)
                for (var y = row - (size - x + col); y <= row + size + (size - x) + col; ++y)
                    if (!IsCovered(processedMatrix[y, x]) && processedMatrix[y, x] != 7)
                        invalidCells++;

                if (invalidCells >= 2)
                    return false;

                return true;
            }
            catch (Exception)
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

            for (var x = processedMatrix.GetLength(1) - 1; x >= 0; --x)
            for (var y = 0; y < processedMatrix.GetLength(0); ++y)
            {
                if (x == 16 && y == 12)
                    Console.Write(true);

                if (IsCovered(processedMatrix[y, x]))
                    for (var k = y; k <= y + 8 && k < processedMatrix.GetLength(0); ++k)
                    {
                        if (k - y == 8)
                        {
                            if (VerifyBadge(processedMatrix, x, y, 8))
                            {
                                var badge = new Badge {BadgeType = BadgeType.Large, Position = new Position(x - 8, y)};
                                PutBadge(processedMatrix, x, y, 8);
                                badges.Add(badge);
                            }

                            break;
                        }

                        if (!IsCovered(processedMatrix[k, x]))
                        {
                            if (k - y == 7)
                                if (VerifyBadge(processedMatrix, x, y, 6))
                                {
                                    var badge = new Badge
                                        {BadgeType = BadgeType.Median, Position = new Position(x - 6, y)};
                                    PutBadge(processedMatrix, x, y, 6);
                                    badges.Add(badge);
                                }


                            if (k - y == 5)
                                if (VerifyBadge(processedMatrix, x, y, 4))
                                {
                                    var badge = new Badge
                                        {BadgeType = BadgeType.Small, Position = new Position(x - 4, y)};
                                    PutBadge(processedMatrix, x, y, 4);
                                    badges.Add(badge);
                                }

                            break;
                        }
                    }
            }

            badges.Reverse();
            return badges;
        }

        private static void PutBadge(int[,] processedMatrix, int col, int row, int size)
        {
            for (var y = row; y <= row + (size - 1); ++y) processedMatrix[y, col] = 5;


            for (var x = col - 1; x >= col - size / 2; --x)
            for (var y = row - (col - x - 1); y <= row + size + (col - x - 1); ++y)
                processedMatrix[y, x] = 5;

            for (var x = col - (size / 2 + 1); x >= col - (size - 1); --x)
            for (var y = row - (size - 1 - col + x); y <= row + size + (size - 1 - col) + x; ++y)
                processedMatrix[y, x] = 5;

            for (var y = row; y <= row + size; ++y)
                if (processedMatrix[y, col - size] == 6)
                    processedMatrix[y, col - size] = 7;
        }

        private static bool IsCovered(int value)
        {
            return value == 1 || value == 4 || value == 6;
        }


        public int[,] PrintDots(Bitmap bitmap, int[,] mapMatrix)
        {
            var colouredMapMatrix = new int[mapMatrix.GetLength(0), mapMatrix.GetLength(1)];

            var pixelHeight = (double) bitmap.Height / (mapMatrix.GetLength(0) - 1);
            var pixelWidth = (double) bitmap.Width / (mapMatrix.GetLength(1) - 1);
            var matrixGrey = bitmap.GetGrayScaleMatrix();

            // check occupied available points
            for (var y = 0; y < mapMatrix.GetLength(0); ++y)
            for (var x = 0; x < mapMatrix.GetLength(1); ++x)
            {
                var posY = (int) Math.Round(y * pixelHeight, 0);
                var posX = (int) Math.Round(x * pixelWidth, 0);
                if (posX >= bitmap.Width)
                    posX = bitmap.Width - 1;
                if (posY >= bitmap.Height)
                    posY = bitmap.Height - 1;

                if (mapMatrix[y, x] == 1)
                {
                    var occupiedPixels = 0;
                    // check top
                    for (var m = posY - pixelHeight; m <= posY + pixelHeight; m++)
                    for (var n = posX - pixelWidth; n <= posX + pixelWidth; n++)
                    {
                        var posM = (int) Math.Round(m, 0);
                        var posN = (int) Math.Round(n, 0);
                        if (posM < 0 || posM >= bitmap.Height || posN < 0 || posN >= bitmap.Width)
                            continue;

                        if (matrixGrey[posM, posN] == 0) occupiedPixels++;
                    }

                    if (occupiedPixels > pixelHeight * pixelWidth * 4 * 0.075)
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

            // Re-mark red points
            for (var y = 0; y < mapMatrix.GetLength(0); ++y)
            for (var x = 0; x < mapMatrix.GetLength(1); ++x)
            {
                var posY = (int) Math.Round(y * pixelHeight, 0);
                var posX = (int) Math.Round(x * pixelWidth, 0);
                if (posX >= bitmap.Width)
                    posX = bitmap.Width - 1;
                if (posY >= bitmap.Height)
                    posY = bitmap.Height - 1;

                if (y == 12 && x == 39)
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

                    if (y == 0 || y == mapMatrix.GetLength(0) - 1)
                        continue;

                    if (colouredMapMatrix[y + 1, x] == 2 && colouredMapMatrix[y - 1, x] == 2 &&
                        colouredMapMatrix[y, x + 1] == 2 && colouredMapMatrix[y, x - 1] == 2)
                        continue;

                    var occupiedPixelsTopLeft = 0;
                    // Top Left Triangle
                    for (var m = posY - pixelHeight; m < posY; m++)
                    for (var n = posX - pixelWidth; n < posX; n++)
                    {
                        if (Math.Abs((m - posY) / (n - posX + pixelWidth)) > ratio)
                            continue; // Not in triangle

                        var posM = (int) Math.Round(m, 0);
                        var posN = (int) Math.Round(n, 0);
                        if (posM < 0 || posM >= bitmap.Height || posN < 0 || posN >= bitmap.Width)
                            continue;

                        if (matrixGrey[posM, posN] == 0) occupiedPixelsTopLeft++;
                    }

                    if (occupiedPixelsTopLeft > pixelHeight * pixelWidth * 0.28)
                    {
                        bitmap.SetPixel(posX, posY, Color.Orange);
                        colouredMapMatrix[y, x] = 4; // covered
                        continue;
                    }

                    // Top Right Triangle
                    var occupiedPixelsTopRight = 0;
                    for (var m = posY - pixelHeight; m < posY; m++)
                    for (double n = posX + 1; n < posX + pixelWidth; n++)
                    {
                        if (Math.Abs((m - posY) / (n - posX - pixelWidth)) > ratio)
                            continue; // Not in triangle

                        var posM = (int) Math.Round(m, 0);
                        var posN = (int) Math.Round(n, 0);
                        if (posM < 0 || posM >= bitmap.Height || posN < 0 || posN >= bitmap.Width)
                            continue;

                        if (matrixGrey[posM, posN] == 0) occupiedPixelsTopRight++;
                    }

                    if (occupiedPixelsTopRight > pixelHeight * pixelWidth * 0.28)
                    {
                        bitmap.SetPixel(posX, posY, Color.Orange);
                        colouredMapMatrix[y, x] = 4; // covered
                        continue;
                    }

                    // Bottom Left Triangle
                    var occupiedPixelsBottomLeft = 0;
                    for (double m = posY + 1; m < posY + pixelHeight; m++)
                    for (var n = posX - pixelWidth; n < posX; n++)
                    {
                        if (Math.Abs((m - posY) / (n - posX + pixelWidth)) > ratio)
                            continue; // Not in triangle

                        var posM = (int) Math.Round(m, 0);
                        var posN = (int) Math.Round(n, 0);
                        if (posM < 0 || posM >= bitmap.Height || posN < 0 || posN >= bitmap.Width)
                            continue;

                        if (matrixGrey[posM, posN] == 0) occupiedPixelsBottomLeft++;
                    }

                    if (occupiedPixelsBottomLeft > pixelHeight * pixelWidth * 0.28)
                    {
                        bitmap.SetPixel(posX, posY, Color.Orange);
                        colouredMapMatrix[y, x] = 4; // covered
                        continue;
                    }

                    if (occupiedPixelsTopLeft + occupiedPixelsBottomLeft >
                        pixelHeight * pixelWidth * 2 * 0.15 && colouredMapMatrix[y - 1, x] == 1
                                                            && colouredMapMatrix[y, x - 1] == 1 &&
                                                            colouredMapMatrix[y + 1, x] == 1)
                    {
                        bitmap.SetPixel(posX, posY, Color.Orange);
                        colouredMapMatrix[y, x] = 4; // covered
                    }

                    // Bottom Right Triangle
                    var occupiedPixelsBottomRight = 0;
                    for (double m = posY + 1; m < posY + pixelHeight; m++)
                    for (double n = posX + 1; n < posX + pixelWidth; n++)
                    {
                        if (Math.Abs((m - posY) / (n - posX - pixelWidth)) > ratio)
                            continue; // Not in triangle

                        var posM = (int) Math.Round(m, 0);
                        var posN = (int) Math.Round(n, 0);
                        if (posM < 0 || posM >= bitmap.Height || posN < 0 || posN >= bitmap.Width)
                            continue;

                        if (matrixGrey[posM, posN] == 0) occupiedPixelsBottomRight++;
                    }

                    if (occupiedPixelsBottomRight > pixelHeight * pixelWidth * 0.28)
                    {
                        bitmap.SetPixel(posX, posY, Color.Orange);
                        colouredMapMatrix[y, x] = 4; // covered
                        continue;
                    }

                    if (occupiedPixelsTopLeft + occupiedPixelsBottomRight + occupiedPixelsBottomLeft +
                        occupiedPixelsBottomRight > pixelHeight * pixelWidth * 4 * 0.1)
                    {
                        bitmap.SetPixel(posX, posY, Color.Orange);
                        colouredMapMatrix[y, x] = 4; // covered
                    }
                }
            }

            // special marking for non-shared left edge
            for (var y = 1; y < mapMatrix.GetLength(0); ++y)
            for (var x = 4; x < mapMatrix.GetLength(1); ++x)
            {
                var posY = (int) Math.Round(y * pixelHeight, 0);
                var posX = (int) Math.Round(x * pixelWidth, 0);
                if (posX >= bitmap.Width)
                    posX = bitmap.Width - 1;
                if (posY >= bitmap.Height)
                    posY = bitmap.Height - 1;


                if (IsCovered(colouredMapMatrix[y, x]))
                {
                    var occupiedPixelsTopLeft = 0;
                    // Top Left Triangle
                    for (var m = posY - pixelHeight; m < posY; m++)
                    for (var n = posX - pixelWidth; n < posX; n++)
                    {
                        if (Math.Abs((m - posY) / (n - posX + pixelWidth)) > ratio)
                            continue; // Not in triangle

                        var posM = (int) Math.Round(m, 0);
                        var posN = (int) Math.Round(n, 0);
                        if (posM < 0 || posM >= bitmap.Height || posN < 0 || posN >= bitmap.Width)
                            continue;

                        if (matrixGrey[posM, posN] == 0) occupiedPixelsTopLeft++;
                    }

                    if (x == 31 && y == 19)
                        Console.Write(true);


                    // Bottom Left Triangle
                    var occupiedPixelsBottomLeft = 0;
                    for (double m = posY + 1; m < posY + pixelHeight; m++)
                    for (var n = posX - pixelWidth; n < posX; n++)
                    {
                        if (Math.Abs((m - posY) / (n - posX + pixelWidth)) > ratio)
                            continue; // Not in triangle

                        var posM = (int) Math.Round(m, 0);
                        var posN = (int) Math.Round(n, 0);
                        if (posM < 0 || posM >= bitmap.Height || posN < 0 || posN >= bitmap.Width)
                            continue;

                        if (matrixGrey[posM, posN] == 0) occupiedPixelsBottomLeft++;
                    }


                    if (occupiedPixelsTopLeft + occupiedPixelsBottomLeft < pixelHeight * pixelWidth * 0.2)
                    {
                        bitmap.SetPixel(posX, posY, Color.Blue);
                        colouredMapMatrix[y, x] = 6; // dedicated
                    }
                }
            }

            if (Debug)
                bitmap.Save(DebugDir + @"\points.jpg");

            return colouredMapMatrix;
        }

        public Bitmap RetrievePanel(string filePath)
        {
            using (var bitMap =
                new Bitmap(filePath))
            {
                using (var bitMapGray = bitMap.ToGrayscale())
                {
                    var panelTopX = 0;
                    var panelTopY = 0;
                    var panelWidth = 0;
                    var panelHeight = 0;
                    using (var bitMapBi = bitMapGray.Binarilization(32))
                    {
                        if (Debug)
                            bitMapBi.Save(DebugDir + @"\bi1.jpg");

                        var matrixGrey = bitMapBi.GetGrayScaleMatrix();

                        var width = matrixGrey.GetLength(1);
                        var height = matrixGrey.GetLength(0);

                        var startX = width - 1;
                        for (var x = width - 1; x >= width * 0.7; --x)
                        {
                            var currentPoints = 0;
                            for (var y = 0; y < height; ++y)
                            {
                                var grayScale = matrixGrey[y, x];
                                if (grayScale == 0)
                                    currentPoints++;
                                else
                                    break;
                            }

                            if (currentPoints >= height - 5)
                            {
                                startX = x;
                                break;
                            }
                        }


                        var boardTopRightX = 0;
                        var boardTopRightY = 0;
                        var boardHeight = 0;

                        for (var x = startX; x >= width * 0.7; --x)
                        {
                            var currentPoints = 0;
                            var currentY = 0;
                            for (var y = (int) (height * 0.5); y < height; ++y)
                            {
                                var grayScale = matrixGrey[y, x];
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
                        for (var y = (int) (height * 0.5); y < height; ++y)
                        {
                            var grayScale = matrixGrey[y, boardTopRightX - 1];
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
                        var bitMapBi = cropped.Binarilization(55, 55, panelWidth / 25);
                        if (Debug)
                            bitMapBi.Save(DebugDir + @"\croppedBi.jpg");


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
                        using (var cropped = bitMapBi.CropAtRect(new Rectangle((int) (bitMapBi.Width * 0.5337),
                            (int) (bitMapBi.Height * 0.6529), (int) (bitMapBi.Width * 0.4198),
                            (int) (bitMapBi.Height * 0.2907))))
                        {
                            var minRow = 0;
                            var matrixGrey = cropped.GetGrayScaleMatrix();


                            var minCol = 0;
                            for (var x = 0; x < 0.3 * cropped.Width; ++x)
                            {
                                var count = 0;
                                for (var y = minRow + 1; y < cropped.Height; ++y)
                                {
                                    var grayScale = matrixGrey[y, x];
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


                            for (var y = minCol; y < 0.05 * cropped.Height; ++y)
                            for (var x = minCol; x < cropped.Width; ++x)
                            {
                                var grayScale = matrixGrey[y, x];
                                if (grayScale != 255)
                                {
                                    minRow = y;
                                    break;
                                }
                            }

                            for (var y = minRow; y < 0.5 * cropped.Height; ++y)
                            {
                                var finish = false;
                                for (var x = minCol; x < cropped.Width; ++x)
                                {
                                    var grayScale = matrixGrey[y, x];
                                    if (grayScale != 255)
                                    {
                                        var badPixels = 0;
                                        for (var yy = y; yy < y + 15 && yy < cropped.Height; ++yy)
                                        for (var xx = x; xx < x + 20 && xx < cropped.Width; ++xx)
                                        {
                                            var grayScale2 = matrixGrey[yy, xx];
                                            if (grayScale2 != 255)
                                                badPixels++;
                                        }

                                        if ((double) badPixels / 300 < 0.2)
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
                                cropped.CropAtRect(new Rectangle(minCol + 1, minRow + 1, cropped.Width - minCol - 1,
                                    cropped.Height - minRow - 1)))
                            {
                                var matrix = final.GetGrayScaleMatrix();
                                // find longest vertical line
                                var maxHeight = 0;
                                var currentX = 0;
                                var pointY = 0;
                                for (var x = 0; x < matrix.GetLength(1) / 6; ++x)
                                {
                                    if (x > 94)
                                        Console.Write(true);
                                    var currentHeight = 0;
                                    for (var y = 0; y < matrix.GetLength(0); ++y)
                                    {
                                        var grayScale = matrix[y, x];
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

                                    if (currentHeight == 0 && maxHeight > 10) break;
                                    ;

                                    if (currentHeight >= maxHeight)
                                    {
                                        maxHeight = currentHeight;
                                        currentX = x;
                                    }
                                }

                                maxHeight = maxHeight + 2;

                                var startX = currentX;
                                var startY = pointY;
                                var pointHeight = (double) maxHeight / 4;
                                var pointWidth = (maxHeight * 1.675 + 2) / 4;


                                var results = new List<Position>();

                                var previousX = startX;
                                var previousY = startY;
                                var lastPosX = 0;
                                var lastPosY = 2;
                                for (var i = 0; i < 6; ++i)
                                {
                                    pointY = GetNextBadgePosition(matrix, ref currentX);

                                    var badgePosX = (int) Math.Round(lastPosX + (currentX - previousX) / pointWidth, 0);
                                    var badgePosY = (int) Math.Round(lastPosY + (pointY - previousY) / pointHeight, 0);
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
            for (var x = currentX + 3; x < matrix.GetLength(1); ++x)
            {
                var currentHeight = 0;
                for (var y = 0; y < matrix.GetLength(0); ++y)
                {
                    var grayScale = matrix[y, x];
                    if (grayScale != 255)
                    {
                        currentHeight++;
                    }
                    else
                    {
                        if (currentHeight != 0)
                        {
                            if (currentHeight + 2 >= maxHeight) pointY = y - currentHeight;

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