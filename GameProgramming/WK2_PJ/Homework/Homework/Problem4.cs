using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Homework
{
    class HW2_4 
    { 
        static void Main(string[] args)
        {
            Bingo game = new Bingo();
            game.Play();
        }
    }

    class Bingo 
    {
        int side;
        int lines = 0;

        int[] series = null;
        int[][] numbers = null;
        int[][] marks = null;

        public Bingo(int side = 5)
        {
            this.side = side;
            Reset();
        }

        public void Shuffle()
        {
            Random rand = new Random();
            for (int i = series.Length - 1; i > 0; i--)
            {
                int j = rand.Next(0, i + 1);
                int tmp = series[i];
                series[i] = series[j];
                series[j] = tmp;
            }
        }

        public void Play()
        {
            Reset();

            bool playing = true;
            while (playing)
            {
                playing = Go();
                lines = CheckLines();

                if (lines == 5)
                {
                    Show();
                    Console.WriteLine("五條連線，遊戲結束");
                    break;
                }
                else
                {
                    Console.WriteLine($"你目前有{lines}條線");
                }
            }
        }

        public bool Go()
        {
            Show();

            Console.Write($"輸入1~{side * side}，或是輸入0退出遊戲：");
            int input = Convert.ToInt32(Console.ReadLine());

            if (input == 0)
            {
                Console.WriteLine("退出遊戲，遊戲結束");
                return false;
            }

            if (input < 1 || input > side * side)
            {
                Console.WriteLine("請輸入有效範圍的數字");
            }
            else
            {
                MarkCell(input);
            }
            return true;
        }

        public void MarkCell(int k)
        {
            for (int i = 0; i < numbers.Length; i++)
            {
                for (int j = 0; j < numbers[i].Length; j++)
                {
                    if (numbers[i][j] == k)
                    {
                        marks[i][j] = 1;
                    }
                }
            }
        }

        public int CheckLines()
        {
            int lines = 0;
            for (int i = 0; i < side; i++)
            {
                if (marks[i][0] == 1 && marks[i][1] == 1 && marks[i][2] == 1 && marks[i][3] == 1 && marks[i][4] == 1)
                {
                    lines += 1;
                }
            }

            for (int i = 0; i < side; i++)
            {
                if (marks[0][i] == 1 && marks[1][i] == 1 && marks[2][i] == 1 && marks[3][i] == 1 && marks[4][i] == 1)
                {
                    lines += 1;
                }
            }

            bool pass = false;
            for (int i = 0; i < side; i++)
            {
                if (marks[i][i] == 1)
                {
                    pass = true;
                }
                else
                {
                    pass = false;
                    break;
                }
            }
            if (pass)
            {
                lines += 1;
            }

            pass = false;
            for (int i = 0; i < side; i++)
            {
                if (marks[i][4-i] == 1)
                {
                    pass = true;
                }
                else
                {
                    pass = false;
                    break;
                }
            }
            if (pass)
            {
                lines += 1;
            }

            return lines;
        }

        public void Show()
        {
            int idx = 0;
            for (int i = 0; i < numbers.Length * 2 + 1; i++)
            {
                if (i % 2 == 0)
                {
                    for (int j = 0; j < side; j++)
                    {
                        Console.Write("+----");
                    }
                    Console.Write("+\n");
                }
                else
                {
                    for (int j = 0; j < numbers[idx].Length; j++)
                    {
                        if (numbers[idx][j] < 10)
                        {
                            if (marks[idx][j] == 1)
                            {
                                Console.Write($"| {numbers[idx][j]}* ");
                            }
                            else
                            {
                                Console.Write($"| {numbers[idx][j]}  ");
                            }
                        }
                        else
                        {
                            if (marks[idx][j] == 1)
                            {
                                Console.Write($"| {numbers[idx][j]}*");
                            }
                            else
                            {
                                Console.Write($"| {numbers[idx][j]} ");
                            }
                        }
                    }
                    Console.Write("|\n");
                    idx++;
                }
            }
        }

        public void Reset()
        {
            if (series == null)
            {
                series = new int[side * side];
            }
            for (int i = 0; i < side * side; i++)
            {
                series[i] = i + 1;
            }

            if (numbers == null)
            {
                numbers = new int[side][];
                for (int i = 0; i < side; i++)
                {
                    numbers[i] = new int[side];
                    for (int j = 0; j < side; j++)
                    {
                        numbers[i][j] = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < side; i++)
                {
                    for (int j = 0; j < side; j++)
                    {
                        numbers[i][j] = 0;
                    }
                }
            }

            if (marks == null)
            {
                marks = new int[side][];
                for (int i = 0; i < side; i++)
                {
                    marks[i] = new int[side];
                    for (int j = 0; j < side; j++)
                    {
                        marks[i][j] = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < side; i++)
                {
                    for (int j = 0; j < side; j++)
                    {
                        marks[i][j] = 0;
                    }
                }
            }

            Shuffle();
            int index = 0;
            for (int r = 0; r < numbers.Length; r++)
            {
                for (int c = 0; c < numbers[r].Length; c++)
                {
                    numbers[r][c] = series[index];
                    index++;
                }
            }
        }
    }

}