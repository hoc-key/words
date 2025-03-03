using System;
using System.IO;

namespace TicTacToe.Business
{
    // Перечисление статуса игры
    public enum GameStatus
    {
        InProgress,
        Won,
        Draw
    }

    public class TicTacToeGame : GameBase
    {
        // Константа – размер игрового поля (9 клеток)
        public const int BoardSize = 9;

        private char[] board;
        private char currentPlayer;
        private int moveCount;
        private char winningPlayer; // сохраняем победителя

        // Свойство для получения текущего игрока
        public char CurrentPlayer
        {
            get { return currentPlayer; }
        }

        // Свойство для получения статуса игры
        public GameStatus Status { get; private set; }

        // Событие, которое вызывается при завершении игры
        public event EventHandler<string> GameOver;

        // Конструктор экземпляра – инициализация доски
        public TicTacToeGame()
        {
            board = new char[BoardSize];
            ResetGame(); // использование значения по умолчанию для старта с 'X'
        }

        // Метод для сброса игры с возможностью задать начального игрока (значение по умолчанию 'X')
        public void ResetGame(char startingPlayer = 'X')
        {
            for (int i = 0; i < BoardSize; i++)
            {
                board[i] = (char)('1' + i);
            }
            currentPlayer = startingPlayer;
            moveCount = 0;
            Status = GameStatus.InProgress;
            winningPlayer = '\0';
        }

        // Переопределяем виртуальный метод для вывода игрового поля
        public override void PrintBoard()
        {
            Console.WriteLine($"{board[0]} | {board[1]} | {board[2]}");
            Console.WriteLine("--+---+--");
            Console.WriteLine($"{board[3]} | {board[4]} | {board[5]}");
            Console.WriteLine("--+---+--");
            Console.WriteLine($"{board[6]} | {board[7]} | {board[8]}");
        }

        // Реализация абстрактного метода PlayGame (не используется напрямую, логика в UI)
        public override void PlayGame()
        {
            // Логика игры реализована через MakeMove и UpdateGameStatus
        }

        // Метод для выполнения хода; позиция передается по ref.
        public bool MakeMove(ref int position)
        {
            if (position < 1 || position > BoardSize)
            {
                return false;
            }

            int index = position - 1;
            if (board[index] == 'X' || board[index] == 'O')
            {
                return false;
            }

            board[index] = currentPlayer;
            moveCount++;
            UpdateGameStatus();

            // Если игра ещё не окончена, переключаем игрока.
            if (Status == GameStatus.InProgress)
            {
                currentPlayer = currentPlayer == 'X' ? 'O' : 'X';
            }
            return true;
        }

        // Метод, использующий out-параметр для получения победителя.
        public bool TryGetWinner(out char winner)
        {
            winner = winningPlayer;
            return Status == GameStatus.Won;
        }

        // Проверка состояния игры и обновление статуса.
        private void UpdateGameStatus()
        {
            if (CheckWin())
            {
                Status = GameStatus.Won;
                winningPlayer = currentPlayer; // победитель – игрок, сделавший последний ход
                OnGameOver("Win");
            }
            else if (moveCount >= BoardSize)
            {
                Status = GameStatus.Draw;
                OnGameOver("Draw");
            }
            else
            {
                Status = GameStatus.InProgress;
            }
        }

        // Проверка выигрышных комбинаций с использованием массива.
        private bool CheckWin()
        {
            int[][] winPatterns = new int[][]
            {
                new int[] { 0, 1, 2 },
                new int[] { 3, 4, 5 },
                new int[] { 6, 7, 8 },
                new int[] { 0, 3, 6 },
                new int[] { 1, 4, 7 },
                new int[] { 2, 5, 8 },
                new int[] { 0, 4, 8 },
                new int[] { 2, 4, 6 }
            };

            foreach (var pattern in winPatterns)
            {
                if (board[pattern[0]] == board[pattern[1]] &&
                    board[pattern[1]] == board[pattern[2]])
                {
                    return true;
                }
            }
            return false;
        }

        // Метод для сохранения состояния игры в файл (с параметром по умолчанию)
        public void SaveGameState(string filePath = "gamestate.txt")
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(new string(board));
                writer.WriteLine(currentPlayer);
                writer.WriteLine(moveCount);
                writer.WriteLine(Status);
            }
        }

        // Метод для загрузки состояния игры из файла (с параметром по умолчанию)
        public bool LoadGameState(string filePath = "gamestate.txt")
        {
            if (!File.Exists(filePath))
            {
                return false;
            }
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length >= 4 && lines[0].Length == BoardSize)
                {
                    for (int i = 0; i < BoardSize; i++)
                    {
                        board[i] = lines[0][i];
                    }
                    currentPlayer = lines[1][0];
                    moveCount = int.Parse(lines[2]);
                    Status = (GameStatus)Enum.Parse(typeof(GameStatus), lines[3]);
                    // Если игра уже завершена, определяем победителя
                    if (Status == GameStatus.Won)
                    {
                        winningPlayer = currentPlayer; // здесь можно доработать логику, если нужно
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        // Виртуальный метод для вызова события завершения игры
        protected virtual void OnGameOver(string result)
        {
            GameOver?.Invoke(this, result);
        }
    }
}
