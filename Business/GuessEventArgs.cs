using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Корнеев_Угадай_слово.Business
{
    public class GuessEventArgs : EventArgs
    {
        public char GuessedLetter { get; }
        public GuessResult Result { get; }
        public string CurrentWordState { get; }

        public GuessEventArgs(char guessedLetter, GuessResult result, string currentWordState)
        {
            GuessedLetter = guessedLetter;
            Result = result;
            CurrentWordState = currentWordState;
        }
    }
}
