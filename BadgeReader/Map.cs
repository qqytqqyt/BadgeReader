using System.Collections.Generic;
using System.Text;

namespace BadgeReader
{
    public class Position
    {
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set; }

        public int Y { get; set; }
    }

    public class Map
    {
        public char[] Characters = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public List<int> StartPosCol = new List<int> {4, 10, 16, 22, 28, 34, 45};

        public const int MaxRow = 29;
        public const int MaxColumn = 45;

        public int[,] MapMatrix = new int[29, 45];

        public int[,] SmallBadgeMatrix = new int[9, 5];

        public Map()
        {
            InitializeSmallBadgeMatrix();
            InitializeMapMatrix();
        }

        public int Possibilities { get; set; } = 0;

        public List<Position> GetPosition(string filePath)
        {
            return new PosRetriever().RetrieveCroppedImg(filePath);
        }

        public string CheckSignatureText(string filePath)
        {
            var result = new PosRetriever().RetrieveCroppedImg(filePath);
            return GetChars(result);
        }

        public string GetChars(List<Position> results)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 6; ++i)
            {
                Possibilities = 0;
                sb.Append(Try(0, StartPosCol[i], 1, 29, StartPosCol[i + 1], results[i].X, results[i].Y));
            }

            return sb.ToString();
        }

        public List<Position> GetPos(string text)
        {
            var results = new List<Position>();
            for (int i = 0; i < 6; ++i)
            {
                Possibilities = 0;
                var result = GetPos(0, StartPosCol[i], 29, StartPosCol[i + 1], text[i]);
                results.Add(result);
            }

            return results;
        }


        public Position GetPos(int currentRow, int currentCol, int maxRow, int maxColumn, char c)
        {
            for (int row = currentRow; row < maxRow; ++row)
            {
                for (int col = currentCol; col < maxColumn; ++col)
                {
                    if (CanFitSmallBadge(row, col, maxRow, maxColumn))
                    {
                        if (Characters[Possibilities] == c)
                            return new Position(col, row);

                        Possibilities++;
                    }
                }
            }

            return new Position(0, 0);
        }


        public char Try(int currentRow, int currentCol, int badgeLeft, int maxRow, int maxColumn, int x, int y)
        {
            for (int row = currentRow; row < maxRow; ++row)
            {
                for (int col = currentCol; col < maxColumn; ++col)
                {
                    if (CanFitSmallBadge(row, col, maxRow, maxColumn))
                    {
                        if (row == y && col == x)
                            return Characters[Possibilities];

                        Possibilities++;
                    }
                }
            }

            return ' ';
        }

        public bool CanFitSmallBadge(int currentRow, int currentCol, int maxRow, int maxColumn)
        {
            if (currentRow >= maxRow - 8 || currentCol >= maxColumn - 4)
                return false;

            for (int row = 0; row < 9; ++row)
            {
                for (int col = 0; col < 5; ++col)
                {
                    if (SmallBadgeMatrix[row, col] == 0)
                        continue;

                    if (MapMatrix[currentRow + row, currentCol + col] != 1)
                        return false;
                }
            }

            return true;
        }


        public void InitializeSmallBadgeMatrix()
        {
            for (int row = 0; row < 9; ++row)
            {
                for (int col = 0; col < 5; ++col)
                {
                    if ((row + col) % 2 == 0)
                        SmallBadgeMatrix[row, col] = 1;
                }
            }

            SmallBadgeMatrix[0, 0] = 0;
            SmallBadgeMatrix[0, 1] = 0;
            SmallBadgeMatrix[0, 3] = 0;
            SmallBadgeMatrix[0, 4] = 0;
            SmallBadgeMatrix[1, 0] = 0;
            SmallBadgeMatrix[1, 4] = 0;
            SmallBadgeMatrix[7, 0] = 0;
            SmallBadgeMatrix[7, 4] = 0;
            SmallBadgeMatrix[8, 0] = 0;
            SmallBadgeMatrix[8, 1] = 0;
            SmallBadgeMatrix[8, 3] = 0;
            SmallBadgeMatrix[8, 4] = 0;
        }

        public void InitializeMapMatrix()
        {
            // top rectangle
            for (int row = 4; row <= 12; ++row)
            {
                for (int col = 0; col <= 40; ++col)
                {
                    if ((row + col) % 2 == 0)
                        MapMatrix[row, col] = 1;
                    else
                        MapMatrix[row, col] = 2;
                }
            }

            // bottom rectangle
            for (int row = 16; row <= 24; ++row)
            {
                for (int col = 4; col <= 4 + 40; ++col)
                {
                    if ((row + col) % 2 == 0)
                        MapMatrix[row, col] = 1;
                    else
                        MapMatrix[row, col] = 2;
                }
            }

            // middle rectangle
            for (int row = 13; row <= 15; ++row)
            {
                for (int col = row - 12; col <= row - 12 + 40; ++col)
                {
                    if ((row + col) % 2 == 0)
                        MapMatrix[row, col] = 1;
                    else
                        MapMatrix[row, col] = 2;
                }
            }

            // top triangles
            for (int col = 4; col <= 36; col += 8)
                MapMatrix[0, col] = 1;

            for (int col = 3; col <= 35; col += 8)
            {
                MapMatrix[1, col] = 1;
                MapMatrix[1, col + 1] = 2;
                MapMatrix[1, col + 2] = 1;
            }

            for (int col = 2; col <= 34; col += 8)
            {
                MapMatrix[2, col] = 1;
                MapMatrix[2, col + 1] = 2;
                MapMatrix[2, col + 2] = 1;
                MapMatrix[2, col + 3] = 2;
                MapMatrix[2, col + 4] = 1;
            }

            for (int col = 1; col <= 33; col += 8)
            {
                MapMatrix[3, col] = 1;
                MapMatrix[3, col + 1] = 2;
                MapMatrix[3, col + 2] = 1;
                MapMatrix[3, col + 3] = 2;
                MapMatrix[3, col + 4] = 1;
                MapMatrix[3, col + 5] = 2;
                MapMatrix[3, col + 6] = 1;
            }

            // bottom triangles
            for (int col = 8; col <= 40; col += 8)
                MapMatrix[28, col] = 1;

            for (int col = 7; col <= 39; col += 8)
            {
                MapMatrix[27, col] = 1;
                MapMatrix[27, col + 1] = 2;
                MapMatrix[27, col + 2] = 1;
            }

            for (int col = 6; col <= 38; col += 8)
            {
                MapMatrix[26, col] = 1;
                MapMatrix[26, col + 1] = 2;
                MapMatrix[26, col + 2] = 1;
                MapMatrix[26, col + 3] = 2;
                MapMatrix[26, col + 4] = 1;
            }

            for (int col = 5; col <= 37; col += 8)
            {
                MapMatrix[25, col] = 1;
                MapMatrix[25, col + 1] = 2;
                MapMatrix[25, col + 2] = 1;
                MapMatrix[25, col + 3] = 2;
                MapMatrix[25, col + 4] = 1;
                MapMatrix[25, col + 5] = 2;
                MapMatrix[25, col + 6] = 1;
            }

            int count = 0;
            for (int row = 0; row < MaxRow; ++row)
            {
                for (int col = 0; col < MaxColumn; ++col)
                {
                    if (MapMatrix[row, col] != 1 && MapMatrix[row, col] != 2)
                        MapMatrix[row, col] = 0;
                    else
                    {
                        count++;
                    }
                }
            }
        }
    }
}
