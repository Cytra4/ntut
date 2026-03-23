using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WK3
{
    class HW3
    {
        static void Main(string[] args)
        {
            Game1A2B game = new Game1A2B(4);
        }
    }

    class Game1A2B
    {
        private string[] nos = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        private List<string> guessHistory = [];
        private List<int[]> abHistory = [];
        private int numberLen;
        private string answer = "";

        // 建構函式，決定遊戲數字的位數(N):
        public Game1A2B(int N)
        {
            numberLen = N;
            Shuffle(nos);
            answer = GenNumber(N);
            Console.WriteLine(answer);
            Play();
        }

        // 回傳指定長度且符合規則的數字:
        public string GenNumber(int n)
        {
            string answer = "";
            for (int i = 0; i < n; i++)
            {
                answer += nos[i];
            }
            return answer;
        }

        // 檢視給定的number字串，是不是符合規則的數字字串:
        public bool IsAvailable(string number)
        {
            if (number.Length < numberLen || number.Length > numberLen)
            {
                return false;
            }
            else
            {
                HashSet<char> hash = new HashSet<char>();
                for (int i = 0; i < number.Length; i++)
                {
                    if (!hash.Add(number[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // 回傳比較 guess 與 answer 是 幾A幾B 的結果:
        public int[] GetAB(string guess, string answer)
        {
            int[] ab = { 0, 0 };  // ab[0] for a; ab[1] for b.
            for (int i = 0; i < numberLen; i++)
            {
                if (guess[i] == answer[i])
                {
                    ab[0] = ab[0] + 1;
                }
                else
                {
                    if (answer.Contains(guess[i]))
                    {
                        ab[1] = ab[1] + 1;
                    }
                }
            }
            return ab;
        }

        // 洗牌的程式，協助生成符合規則的數字:
        public void Shuffle(string[] number)
        {
            Random rand = new Random();
            for (int i = number.Length - 1; i > 0; i--)
            {
                int j = rand.Next(0, i + 1);
                string tmp = number[i];
                number[i] = number[j];
                number[j] = tmp;
            }
        }

        public void Play()
        {
            bool over = false;
            string guess = "";

            Console.WriteLine("Game number 1A2B!(4-digits)");
            while (!over)
            {
                Console.Write("Your guess (or 0 to exit): ");
                guess = Console.ReadLine();

                if (guess == "0")
                {
                    Console.WriteLine("Game over");
                    break;
                }
                else if (IsAvailable(guess))
                {
                    int[] ab = GetAB(guess, answer);
                    guessHistory.Add(guess);
                    abHistory.Add(ab);
                    for (int i = 0; i < guessHistory.Count(); i++)
                    {
                        string result = "";
                        if (abHistory[i][0] != 0)
                        {
                            result += $"{abHistory[i][0]}A";
                        }
                        if (abHistory[i][1] != 0)
                        {
                            result += $"{abHistory[i][1]}B";
                        }
                        if (result.Length == 0)
                        {
                            result = "0";
                        }

                        Console.WriteLine($"{guessHistory[i]} => {result}");
                    }

                    if (ab[0] == 4)
                    {
                        Console.WriteLine("You've got the answer!");
                        over = true;
                    }
                }
                else
                {
                    Console.WriteLine("Please enter valid guess");
                }
            }
        }
    }
}
