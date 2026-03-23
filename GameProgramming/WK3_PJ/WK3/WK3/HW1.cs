using System;
using System.Drawing;
using System.Numerics;
using System.Threading;

namespace WK3
{
    class HW1
    {
        static void Problem1(string[] args)
        {
            PlayBingo();
        }

        static int ComputerPick(int[] appeared)
        {
            bool valid = false;
            Random rand = new Random();
            int computer_choice = rand.Next(1,26);
            while (!valid)
            {
                if (appeared.Contains(computer_choice))
                {
                    computer_choice = rand.Next(1, 26);
                }
                else
                {
                    valid = true;
                }
            }

            return computer_choice;
        }

        static bool CheckOver(int p_line, int c_line)
        {
            if (p_line >= 5 && c_line >= 5)
            {
                if (p_line > c_line)
                {
                    Console.WriteLine("玩家勝利，遊戲結束");
                }
                else if (p_line < c_line)
                {
                    Console.WriteLine("電腦勝利，遊戲結束");
                }
                else
                {
                    Console.WriteLine("平手，遊戲結束");
                }
                return true;
            }
            else if (p_line >= 5 && c_line < 5) 
            {
                Console.WriteLine("玩家勝利，遊戲結束");
                return true;
            }
            else if (p_line < 5 && c_line >= 5)
            {
                Console.WriteLine("電腦勝利，遊戲結束");
                return true;
            }

            return false;
        }

        static void ShowBoard(Bingo player, Bingo computer, int p_line, int c_line)
        {
            Console.WriteLine("你的Bingo表：");
            player.Show();
            Console.WriteLine("電腦的Bingo表：");
            computer.Show();
            Console.WriteLine($"你目前有{p_line}條線");
            Console.WriteLine($"電腦目前有{c_line}條線");
        }

        static void PlayBingo()
        {
            bool playing = true;
            int p_line = 0;
            int c_line = 0;
            int computer_choice;
            int[] appeared = new int[25];
            int idx = 0;

            Bingo player = new Bingo();
            Bingo computer = new Bingo();

            player.Show();
            while (playing)
            {
                Console.Write("輸入1~25，或是輸入0退出遊戲：");
                int input = Convert.ToInt32(Console.ReadLine());
                if (appeared.Contains(input))
                {
                    Console.Write("請輸入未填過的數字");
                }
                else if (input < 1 || input > 25)
                {
                    Console.WriteLine("請輸入有效範圍的數字");
                }
                else
                {
                    player.Go(input);
                    computer.Go(input);
                    appeared[idx] = input;
                    idx++;
                    p_line = player.CheckLines();
                    c_line = computer.CheckLines();
                    ShowBoard(player, computer, p_line, c_line);
                    playing = CheckOver(p_line, c_line);
                }

                Thread.Sleep(3000);

                computer_choice = ComputerPick(appeared);
                appeared[idx] = computer_choice;
                idx++;

                Console.WriteLine($"電腦選擇了：{computer_choice}");
                player.Go(computer_choice);
                computer.Go(computer_choice);
                p_line = player.CheckLines();
                c_line = computer.CheckLines();
                ShowBoard(player, computer, p_line, c_line);
                playing = CheckOver(p_line, c_line);
            }
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

        public bool Go(int input)
        {
            if (input == 0)
            {
                Console.WriteLine("退出遊戲，遊戲結束");
                return false;
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
                if (marks[i][4 - i] == 1)
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