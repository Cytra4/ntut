using System;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WK1
{
    class HW1
    {
        static void Main(string[] args)
        {
            Console.WriteLine("終極密碼猜數字遊戲");
            Random rnd = new Random();
            int answer = rnd.Next(1, 100);
            int left = 1;
            int right = 99;
            int guess = 10000;
            while (guess != answer)
            {
                Console.WriteLine($"請輸入介於 ({left}, {right}) 之間的數字:");
                guess = Convert.ToInt32(Console.ReadLine());
                if (guess == answer)
                {
                    Console.WriteLine("答對了！");
                    break;
                }
                else
                {
                    if (guess < answer)
                    {
                        left = guess;
                    }
                    else
                    {
                        right = guess;
                    }
                }
            }
        }
    }
}