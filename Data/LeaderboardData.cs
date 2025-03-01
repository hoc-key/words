using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TicTacToe.Data
{
    public class LeaderboardData
    {
        private const string LeaderboardFilePath = "leaderboard.txt";

        // Сохраняем результат: если имя уже есть, добавляем очки; иначе создаём новую запись.
        public static void SaveScore(string playerName, int score)
        {
            Dictionary<string, int> leaderboard = LoadLeaderboardDictionary();
            if (leaderboard.ContainsKey(playerName))
            {
                leaderboard[playerName] += score;
            }
            else
            {
                leaderboard[playerName] = score;
            }
            WriteLeaderboard(leaderboard);
        }

        private static Dictionary<string, int> LoadLeaderboardDictionary()
        {
            var leaderboard = new Dictionary<string, int>();
            if (File.Exists(LeaderboardFilePath))
            {
                foreach (var line in File.ReadAllLines(LeaderboardFilePath))
                {
                    // Ожидается формат "Имя:Очки"
                    var parts = line.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int points))
                    {
                        leaderboard[parts[0]] = points;
                    }
                }
            }
            return leaderboard;
        }

        private static void WriteLeaderboard(Dictionary<string, int> leaderboard)
        {
            var lines = leaderboard.Select(kvp => $"{kvp.Key}:{kvp.Value}");
            File.WriteAllLines(LeaderboardFilePath, lines);
        }

        public static List<string> LoadLeaderboard()
        {
            var leaderboardList = new List<string>();
            if (File.Exists(LeaderboardFilePath))
            {
                leaderboardList.AddRange(File.ReadAllLines(LeaderboardFilePath));
            }
            return leaderboardList;
        }
    }
}
