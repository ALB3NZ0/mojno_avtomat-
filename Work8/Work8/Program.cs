using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

class User
{
    public string Name { get; set; }
    public int CharactersPerMinute { get; set; }
    public int CharactersPerSecond { get; set; }
}

static class Leaderboard
{
    private const string FILENAME = "leaderboard.json";
    private static List<User> users = new List<User>();

    public static void Load()
    {
        if (File.Exists(FILENAME))
        {
            string json = File.ReadAllText(FILENAME);
            users = JsonConvert.DeserializeObject<List<User>>(json);
        }
    }

    public static void Save()
    {
        string json = JsonConvert.SerializeObject(users);
        File.WriteAllText(FILENAME, json);
    }

    public static void AddUser(User user)
    {
        users.Add(user);
        Save();
    }

    public static void Show()
    {
        Console.WriteLine("Таблица рекродов:");
        Console.WriteLine("{0,-20} {1,-20} {2,-20}", "Имя", "Символов в минуту", "Символов в секунду");
        foreach (User user in users)
        {
            Console.WriteLine("{0,-20} {1,-20} {2,-20}", user.Name, user.CharactersPerMinute, user.CharactersPerSecond);
        }
    }
}

class TypingTest
{
    private const string TEXT = "Никакой писатель меня не удивит\r\nЯ воспитан мелодией огня, и может быть\r\nКогда свет подарит моим близким тепло\r\nЯ отвечу им так же, спасибо, мама\r\nВетер гонит в океан, там огромный великан\r\nОн давно уже затих, man, и просит план\r\nЯ по-тихому лечу, мою крылья тороплю\r\nБо-боюсь (Ба), что на-намочу манту\r\nРуку до небес, это — атаман и бес\r\nТам, где дети по балде-е, раскуривают вес\r\nНаши ночи напролёт, календарь и переплёт\r\nУкажи мне дорогу, хип-хопа слёт (Хип-хопа сл�)\r\nМоя-ая, твоя лесть, я — воин, во же есть\r\nПодари надежду жить, по природе колесить\r\nВдруг меня морозит звук, передай это вокруг\r\nНам напел местный гид, я пою для рук";
    private const int MINUTES = 1;

    public static void Run()
    {
        Console.Write("Введите имя: ");
        string name = Console.ReadLine();

        Console.WriteLine("Введите следующий текст:");
        Console.WriteLine(TEXT);

        Console.Write("> ");
        Console.ForegroundColor = ConsoleColor.Green;

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        string input = string.Empty;

        Thread inputThread = new Thread(() =>
        {
            input = Console.ReadLine();
        });
        inputThread.Start();

        while (stopwatch.Elapsed < TimeSpan.FromMinutes(MINUTES))
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("Оставшееся время: " + (MINUTES * 60 - stopwatch.Elapsed.TotalSeconds).ToString("N0") + " секунд");
            if (!inputThread.IsAlive)
            {
                break;
            }
            Thread.Sleep(100);
        }

        stopwatch.Stop();
        Console.ResetColor();

        int charactersTyped = input.Length;
        int secondsElapsed = (int)stopwatch.Elapsed.TotalSeconds;
        int charactersPerMinute = (int)(charactersTyped / (double)MINUTES);
        int charactersPerSecond = (int)(charactersTyped / (double)secondsElapsed);

        User user = new User { Name = name, CharactersPerMinute = charactersPerMinute, CharactersPerSecond = charactersPerSecond };
        Leaderboard.AddUser(user);

        Leaderboard.Show();
    }
}

class Program
{
    static void Main(string[] args)
    {
        Leaderboard.Load();

        while (true)
        {
            TypingTest.Run();

            Console.Write("Нажмите Enter, чтобы повторить попытку, или любую другую клавишу для выхода...");
            if (Console.ReadKey().Key != ConsoleKey.Enter)
            {
                break;
            }

            Console.Clear();
        }
    }
}