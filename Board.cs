using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minesweeperWinForms
{
    internal class Board
    {
        public int n;
        public int m;
        public int bombs;
        public int[,] board;
        public Board(int rows, int columns)
        {
            n = rows;
            m = columns;
            bombs = rows * columns / 6;
            board = new int[rows, columns];
            fillBoard();

        }

        public void showBoard()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (board[i, j] == -1)
                    {
                        Console.Write("{0} ", board[i, j]);
                    }
                    else
                    {
                        Console.Write(" {0} ", board[i, j]);
                    }
                }
                Console.WriteLine();
            }
        }

        public void fillBoard()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    board[i, j] = 0;
                }
            }

            Random rnd = new Random();
            for (int k = 0; k < bombs; k++)
            {
                board[rnd.Next(n), rnd.Next(m)] = -1;
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (board[i, j] != -1)
                    {
                        board[i, j] = countBombsInNeighbour(i, j);
                    }
                }
            }

        }

        public int countBombsInNeighbour(int i, int j)
        {   
            int counter = 0;
            for (int k = i - 1; k <= i + 1; k++)
            {
                for (int l = j - 1; l <= j + 1; l++)
                {
                    if (k < 0 || k >= n)
                    {
                        continue;
                    }
                    if (l < 0 || l >= m)
                    {
                        continue;
                    }
                    if (board[k, l] == -1)
                    {
                        counter++;
                    }
                }
            }
            return counter;
        }
    }
}
