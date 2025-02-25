using System;
using Корнеев_Угадай_слово.Business;
using Корнеев_Угадай_слово.Data;
using Корнеев_Угадай_слово.Models;

namespace Корнеев_Угадай_слово
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Для корректного отображения кириллицы
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var dbAccess = new GameDataAccess(); // тестирование БД
            dbAccess.TestConnection(); // тестирование БД

            IGameDataAccess dataAccess = new GameDataAccess();
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== Игра 'Угадай слово' ===");
                Console.WriteLine("1. Новая игра");
                Console.WriteLine("2. Загрузить игру");
                Console.WriteLine("3. Показать рейтинг");
                Console.WriteLine("4. Выход");
                Console.Write("Выберите опцию: ");

                string? input = Console.ReadLine() ?? ""; // Гарантируем, что input не null
                switch (input)
                {
                    case "1":
                        RunNewGame(dataAccess);
                        break;
                    case "2":
                        RunLoadGame(dataAccess);
                        break;
                    case "3":
                        ShowRating(dataAccess);
                        break;
                    case "4":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу для продолжения...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void RunNewGame(IGameDataAccess dataAccess)
        {
            Game game = new Game();
            game.GuessProcessed += Game_GuessProcessed;
            PlayGame(game);
            EndGame(dataAccess, game);
        }

        static void RunLoadGame(IGameDataAccess dataAccess)
        {
            Game game = new Game();
            if (!game.LoadGame())
            {
                Console.WriteLine("Не удалось загрузить игру. Возможно, файл сохранения отсутствует или поврежден.");
                Console.WriteLine("Нажмите любую клавишу для возврата в главное меню...");
                Console.ReadKey();
                return;
            }
            game.GuessProcessed += Game_GuessProcessed;
            PlayGame(game);
            EndGame(dataAccess, game);
        }

        static void PlayGame(Game game)
        {
            while (!game.IsGameOver)
            {
                Console.WriteLine();
                Console.WriteLine("Слово: " + game.GetCurrentWordState());
                Console.WriteLine("Оставшиеся попытки: " + game.RemainingAttempts);
                Console.WriteLine("Счет: " + game.Score);
                Console.Write("Введите букву (или 'save' для сохранения игры): ");

                string? input = Console.ReadLine() ?? ""; // Защита от null
                if (input.Trim().ToLower() == "save")
                {
                    try
                    {
                        game.SaveGame();
                        Console.WriteLine("Игра сохранена.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка при сохранении игры: " + ex.Message);
                    }
                    continue;
                }
                if (game.TryParseGuess(input, out char guess))
                {
                    game.ProcessGuess(guess);
                }
                else
                {
                    Console.WriteLine("Некорректный ввод. Пожалуйста, введите букву.");
                }
            }
        }

        static void EndGame(IGameDataAccess dataAccess, Game game)
        {
            Console.WriteLine("Игра окончена!");
            Console.WriteLine("Слово: " + game.WordToGuess);
            Console.WriteLine("Ваш счет: " + game.Score);
            Console.Write("Введите ваше имя для рейтинга: ");

            string? name = Console.ReadLine()?.Trim(); // Читаем имя и обрезаем пробелы
            name = string.IsNullOrWhiteSpace(name) ? "Безымянный" : name; // Если имя пустое → ставим "Безымянный"

            Player player = new Player { Name = name, Score = game.Score };
            dataAccess.SavePlayerScore(player);
            Console.WriteLine("Результат сохранен в рейтинге. Нажмите любую клавишу для возврата в главное меню...");
            Console.ReadKey();
        }

        // Сделали `sender` nullable, чтобы соответствовать EventHandler<T>
        static void Game_GuessProcessed(object? sender, GuessEventArgs e)
        {
            switch (e.Result)
            {
                case GuessResult.Correct:
                    Console.WriteLine($"Верно! Буква '{e.GuessedLetter}' присутствует в слове.");
                    break;
                case GuessResult.Incorrect:
                    Console.WriteLine($"Неверно! Буква '{e.GuessedLetter}' отсутствует в слове.");
                    break;
                case GuessResult.AlreadyGuessed:
                    Console.WriteLine($"Вы уже вводили букву '{e.GuessedLetter}'.");
                    break;
                case GuessResult.Invalid:
                    Console.WriteLine("Некорректный ввод. Пожалуйста, введите букву.");
                    break;
            }
        }

        static void ShowRating(IGameDataAccess dataAccess)
        {
            Console.Clear();
            Console.WriteLine("=== Лучшие игроки ===");
            var bestPlayers = dataAccess.GetBestPlayers();
            if (bestPlayers.Count == 0)
            {
                Console.WriteLine("Рейтинг пуст.");
            }
            else
            {
                foreach (var player in bestPlayers)
                {
                    Console.WriteLine($"Имя: {player.Name}, Счет: {player.Score}");
                }
            }
            Console.WriteLine("Нажмите любую клавишу для возврата в главное меню...");
            Console.ReadKey();
        }
    }
}
