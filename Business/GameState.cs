using System;
using System.Collections.Generic;

namespace Корнеев_Угадай_слово.Business
{
    public class GameState
    {
        public string WordToGuess { get; set; } = string.Empty;  // Инициализируем пустой строкой
        public List<char> GuessedLetters { get; set; } = new List<char>(); // Инициализируем пустым списком
        public int RemainingAttempts { get; set; }
        public int Score { get; set; }
        public bool IsGameOver { get; set; }
    }
}
