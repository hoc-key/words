using System;
using TicTacToe.Business;
using TicTacToe.Data;

namespace TicTacToe.Program
{
    public class GameUI
    {
        private TicTacToeGame game;

        public GameUI()
        {
            game = new TicTacToeGame();
            // Подписка на событие завершения игры.
            game.GameOver += OnGameOver;
        }

        private void OnGameOver(object sender, string result)
        {
            Console.WriteLine($"Игра завершена: {result}");
        }

        public void Start()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1. Новая игра");
                Console.WriteLine("2. Загрузить игру");
                Console.WriteLine("3. Показать таблицу лидеров");
                Console.WriteLine("4. Выход");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        PlayGame();
                        break;
                    case "2":
                        if (game.LoadGameState())
                        {
                            Console.WriteLine("Игра загружена.");
                            PlayGame();
                        }
                        else
                        {
                            Console.WriteLine("Не удалось загрузить игру.");
                            Console.ReadKey();
                        }
                        break;
                    case "3":
                        ShowLeaderboard();
                        break;
                    case "4":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Некорректный выбор. Нажмите любую клавишу для продолжения.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void PlayGame()
        {
            // Если игра уже завершена или начинается новая, сбрасываем состояние.
            if (game.Status != GameStatus.InProgress)
            {
                game.ResetGame();
            }

            while (game.Status == GameStatus.InProgress)
            {
                Console.Clear();
                Console.WriteLine($"Текущий игрок: {game.CurrentPlayer}");
                game.PrintBoard();
                Console.WriteLine("Введите номер клетки (1-9) или 'S' для сохранения:");
                string input = Console.ReadLine();

                // Если ввод равен "S" или "s" – сохраняем игру и продолжаем.
                if (input.Equals("S", StringComparison.OrdinalIgnoreCase))
                {
                    game.SaveGameState();
                    Console.WriteLine("Игра сохранена. Нажмите любую клавишу для продолжения.");
                    Console.ReadKey();
                    continue;
                }

                if (int.TryParse(input, out int move))
                {
                    int moveRef = move;
                    if (!game.MakeMove(ref moveRef))
                    {
                        Console.WriteLine("Некорректный ход, попробуйте снова.");
                        Console.ReadKey();
                    }
                }
                else
                {
                    Console.WriteLine("Введите корректное число или 'S' для сохранения.");
                    Console.ReadKey();
                }
            }

            Console.Clear();
            game.PrintBoard();
            if (game.Status == GameStatus.Won)
            {
                if (game.TryGetWinner(out char winner))
                {
                    Console.WriteLine($"Поздравляем! Победитель: {winner}");
                    Console.WriteLine("Введите ваше имя для таблицы лидеров:");
                    string name = Console.ReadLine();
                    // За выигрыш начисляем, например, 100 очков.
                    LeaderboardData.SaveScore(name, 100);
                }
            }
            else if (game.Status == GameStatus.Draw)
            {
                Console.WriteLine("Ничья!");
            }
            Console.WriteLine("Нажмите любую клавишу для продолжения.");
            Console.ReadKey();
        }

        private void ShowLeaderboard()
        {
            var leaderboard = LeaderboardData.LoadLeaderboard();
            Console.Clear();
            Console.WriteLine("Таблица лидеров:");
            foreach (var entry in leaderboard)
            {
                Console.WriteLine(entry);
            }
            Console.WriteLine("Нажмите любую клавишу для продолжения.");
            Console.ReadKey();
        }
    }
}
