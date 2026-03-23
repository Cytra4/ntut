using System;

namespace Homework
{
    class HW2_3
    {
        static void Problem3(string[] args)
        {
            int n = Convert.ToInt32(Console.ReadLine());
            BingoCreate(n);
        }

        static void BingoCreate(int n)
        {
            int[][] number = randomGen(n);
            int idx = 0;
            for (int i = 0; i < number.Length*2+1; i++)
            {
                if (i % 2 == 0)
                {
                    for (int j = 0; j < n; j++)
                    {
                        Console.Write("+----");
                    }
                    Console.Write("+\n");
                }
                else
                {
                    for (int j = 0; j < number[idx].Length; j++)
                    {
                        if (number[idx][j] < 10)
                        {
                            Console.Write($"| {number[idx][j]}  ");
                        }
                        else
                        {
                            Console.Write($"| {number[idx][j]} ");
                        }
                    }
                    Console.Write("|\n");
                    idx++;
                }
            }
        }
        static int[][] randomGen(int n)
        {
            int[][] gen = new int[n][];
            int[] appeared = new int[n*n];
            int counter = 0;
            Random rand = new Random();

            for (int i = 0; i < n; i++)
            {
                gen[i] = new int[n];
                int[] tmp = new int[n];
                int number = rand.Next(1, n * n + 1);
                for (int j = 0; j < n; j++)
                {
                    while (appeared.Contains(number))
                    {
                        number = rand.Next(1, n * n + 1);
                    }
                    tmp[j] = number;
                    appeared[counter] = number;
                    counter++;
                }
                gen[i] = tmp;
            }
            return gen;
        }
    }
}
