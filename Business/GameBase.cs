using System;

namespace TicTacToe.Business
{
    public abstract class GameBase
    {
        // Абстрактный метод для запуска игры.
        public abstract void PlayGame();

        // Виртуальный метод для вывода игрового поля.
        public virtual void PrintBoard()
        {
            Console.WriteLine("Метод PrintBoard базового класса");
        }
    }
}
