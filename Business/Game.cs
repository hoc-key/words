using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Корнеев_Угадай_слово.Business
{
    public class Game
    {
        // Событие теперь nullable, чтобы избежать ошибки
        public event EventHandler<GuessEventArgs>? GuessProcessed;

        // Инициализируем, чтобы избежать null-значений
        public string WordToGuess { get; private set; } = string.Empty;
        public HashSet<char> GuessedLetters { get; private set; } = new HashSet<char>();
        public int RemainingAttempts { get; private set; }
        public int Score { get; private set; }
        public bool IsGameOver { get; private set; }

        // Список слов для угадывания.
        private readonly List<string> _wordList = new()
        {
            "apple", "banana", "orange", "grape", "mango"
        };

        private readonly Random _random = new();

        public Game()
        {
            StartNewGame();
        }

        public void StartNewGame()
        {
            WordToGuess = _wordList[_random.Next(_wordList.Count)];
            GuessedLetters = new HashSet<char>();
            RemainingAttempts = GameConstants.MaxAttemptsDefault;
            Score = 0;
            IsGameOver = false;
        }

        public void SaveGame(string fileName = GameConstants.SaveFileName)
        {
            var gameState = new GameState
            {
                WordToGuess = this.WordToGuess,
                GuessedLetters = this.GuessedLetters.ToList(),
                RemainingAttempts = this.RemainingAttempts,
                Score = this.Score,
                IsGameOver = this.IsGameOver
            };

            string json = JsonSerializer.Serialize(gameState);
            File.WriteAllText(fileName, json);
        }

        public bool LoadGame(string fileName = GameConstants.SaveFileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }
            try
            {
                string json = File.ReadAllText(fileName);
                var gameState = JsonSerializer.Deserialize<GameState>(json);
                if (gameState == null)
                    return false;

                this.WordToGuess = gameState.WordToGuess ?? string.Empty;
                this.GuessedLetters = new HashSet<char>(gameState.GuessedLetters ?? new List<char>());
                this.RemainingAttempts = gameState.RemainingAttempts;
                this.Score = gameState.Score;
                this.IsGameOver = gameState.IsGameOver;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public GuessResult ProcessGuess(char guess, bool updateScore = true)
        {
            guess = char.ToLower(guess);

            if (!char.IsLetter(guess))
            {
                OnGuessProcessed(new GuessEventArgs(guess, GuessResult.Invalid, GetCurrentWordState()));
                return GuessResult.Invalid;
            }

            if (GuessedLetters.Contains(guess))
            {
                OnGuessProcessed(new GuessEventArgs(guess, GuessResult.AlreadyGuessed, GetCurrentWordState()));
                return GuessResult.AlreadyGuessed;
            }

            GuessedLetters.Add(guess);

            if (WordToGuess.Contains(guess))
            {
                if (updateScore)
                    Score += 10;

                OnGuessProcessed(new GuessEventArgs(guess, GuessResult.Correct, GetCurrentWordState()));

                if (IsWordGuessed())
                {
                    IsGameOver = true;
                }
                return GuessResult.Correct;
            }
            else
            {
                RemainingAttempts--;
                OnGuessProcessed(new GuessEventArgs(guess, GuessResult.Incorrect, GetCurrentWordState()));

                if (RemainingAttempts <= 0)
                {
                    IsGameOver = true;
                }
                return GuessResult.Incorrect;
            }
        }

        public bool IsWordGuessed()
        {
            foreach (char ch in WordToGuess)
            {
                if (!GuessedLetters.Contains(ch))
                    return false;
            }
            return true;
        }

        public string GetCurrentWordState()
        {
            char[] display = new char[WordToGuess.Length];
            for (int i = 0; i < WordToGuess.Length; i++)
            {
                char ch = WordToGuess[i];
                display[i] = GuessedLetters.Contains(ch) ? ch : '_';
            }
            return new string(display);
        }

        protected virtual void OnGuessProcessed(GuessEventArgs e)
        {
            GuessProcessed?.Invoke(this, e);
        }

        public void UpdateScore(ref int additionalPoints)
        {
            Score += additionalPoints;
        }

        public bool TryParseGuess(string input, out char guess)
        {
            guess = ' ';
            if (string.IsNullOrEmpty(input))
                return false;
            guess = input[0];
            return true;
        }
    }
}
