using System;
using System.Text;

namespace AdventOfCode2020
{
    public static class CharGridExtensions
    {
        public static string Render(this char[,] grid)
        {
            var height = grid.GetLength(0);
            var width = grid.GetLength(1);

            var txt = new StringBuilder(height * width + height);

            for (int row = 0; row < width; row++)
            {
                for (int col = 0; col < height; col++)
                {
                    txt.Append(grid[row, col]);
                }

                txt.Append('\n');
            }

            return txt.ToString();
        }

        public static char[,] Rotate(this char[,] grid)
        {
            int n = grid.GetLength(0);

            if (n != grid.GetLength(1))
            {
                throw new NotImplementedException();
            }

            var transposed = new char[n, n];

            for (int x = 0; x < n; ++x)
            {
                for (int y = 0; y < n; ++y)
                {
                    transposed[x, y] = grid[n - y - 1, x];
                }
            }

            return transposed;
        }

        public static char[,] FlipVertical(this char[,] grid)
        {
            int n = grid.GetLength(0);

            if (n != grid.GetLength(1))
            {
                throw new NotImplementedException();
            }

            var transposed = new char[n, n];

            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < n; y++)
                {
                    transposed[x, y] = grid[x, n - 1 - y];
                }
            }

            return transposed;
        }

        public static char[,] FlipHorizontal(this char[,] grid)
        {
            int n = grid.GetLength(0);

            if (n != grid.GetLength(1))
            {
                throw new NotImplementedException();
            }

            var transposed = new char[n, n];

            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < n; y++)
                {
                    transposed[x, y] = grid[n - 1 - x, y];
                }
            }

            return transposed;
        }
    }
}