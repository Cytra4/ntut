using System;

namespace Homework
{
    class WK2
    {
        static void Problem1(string[] args)
        {
            int counter = 1;
            int[][] jagged = new int[10][];
            for (int i = 0; i < 10; i++)
            {
                jagged[i] = new int[i+1];
                for (int j = 0; j < i + 1; j++)
                {
                    jagged[i][j] = counter;
                    counter++;
                }
            }

            for (int i = 0; i < jagged.Length; i++)
            {
                for (int j = 0; j < jagged[i].Length; j++)
                {
                    Console.Write($"ary({i},{j})={jagged[i][j],2} ");
                }
                Console.WriteLine();
            }
        }
    }
}