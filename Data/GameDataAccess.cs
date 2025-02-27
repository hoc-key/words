using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Корнеев_Угадай_слово.Data;
using Корнеев_Угадай_слово.Models;

using Npgsql;

namespace Корнеев_Угадай_слово.Data
{
    public class GameDataAccess : IGameDataAccess
    {
        // Строка подключения к PostgreSQL(MongoDB). Измените параметры (Host, Username, Password, Database) согласно вашей конфигурации.
        private const string ConnectionString = "Host=localhost;Username=postgres;Password='';Database=guess_word_game";

        public GameDataAccess()
        {
            InitializeDatabase();
        }

        /// <summary>
        /// Инициализация базы данных: создание таблицы рейтинга, если её нет.
        /// </summary>
        private void InitializeDatabase()
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText =
                    @"CREATE TABLE IF NOT EXISTS Ratings (
                        Id SERIAL PRIMARY KEY,
                        PlayerName TEXT NOT NULL,
                        Score INTEGER NOT NULL,
                        DateAchieved TIMESTAMP NOT NULL
                      );";
                using (var command = new NpgsqlCommand(commandText, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        public bool TestConnection()
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine("Подключение к PostgreSQL успешно!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка подключения: " + ex.Message);
                return false;
            }
        }


        public void SavePlayerScore(Player player)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                string commandText =
                    @"INSERT INTO Ratings (PlayerName, Score, DateAchieved)
                      VALUES (@name, @score, @date);";
                using (var command = new NpgsqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("name", player.Name);
                    command.Parameters.AddWithValue("score", player.Score);
                    command.Parameters.AddWithValue("date", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Player> GetBestPlayers()
        {
            List<Player> players = new List<Player>();
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();
                // Получаем максимальный счет из таблицы
                string maxScoreQuery = "SELECT MAX(Score) FROM Ratings;";
                int maxScore = 0;
                using (var command = new NpgsqlCommand(maxScoreQuery, connection))
                {
                    var result = command.ExecuteScalar();
                    if (result == DBNull.Value || result == null)
                    {
                        return players;
                    }
                    maxScore = Convert.ToInt32(result);
                }

                // Получаем игроков, набравших максимальный счет
                string query = "SELECT PlayerName, Score FROM Ratings WHERE Score = @maxScore;";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("maxScore", maxScore);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Player player = new Player
                            {
                                Name = reader.GetString(0),
                                Score = reader.GetInt32(1)
                            };
                            players.Add(player);
                        }
                    }
                }
            }
            return players;
        }
    }
}
