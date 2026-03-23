using System;

namespace Homework
{
    class WK2_2
    {
        static void Problem2(string[] args)
        {
            int[] data = randomGen();
            Console.WriteLine("初始資料：");
            PrintArray(data);

            for (int i = 0; i < 3; i++)
            {
                data = FisherYatesShuffle(data);
                Console.WriteLine($"轉換第{i + 1}次：");
                PrintArray(data);
            }
        }

        static int[] randomGen()
        {
            int[] gen = new int[20];
            int number;
            Random rand = new Random();
            for (int i = 0; i < 20; i++)
            {
                number = rand.Next(1, 51);
                while (gen.Contains(number))
                {
                    number = rand.Next(1, 51);
                }
                gen[i] = number;
            }
            return gen;
        }
        static int[] FisherYatesShuffle(int[] data)
        {
            int[] data_cpy = data;
            Random rand = new Random();
            for (int i = data_cpy.Length - 1; i > 0; i--)
            {
                int j = rand.Next(0, i+1);
                int tmp = data_cpy[i];
                data_cpy[i] = data_cpy[j];
                data_cpy[j] = tmp;
            }
            return data_cpy;
        }

        static void PrintArray(int[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (i != data.Length - 1)
                {
                    Console.Write($"{data[i]},");
                }
                else
                {
                    Console.Write(data[i]);
                }
            }
            Console.WriteLine();
        }
    }
}