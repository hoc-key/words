using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Корнеев_Угадай_слово.Models;

namespace Корнеев_Угадай_слово.Data
{
    public interface IGameDataAccess
    {
        void SavePlayerScore(Player player);
        List<Player> GetBestPlayers();
    }
}
