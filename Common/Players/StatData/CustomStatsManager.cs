using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Common.Players.Stats
{
    public sealed class CustomStatsManager
    {
        private static List<PlayerStat> _registeredStats;
        private List<PlayerStat> _playerStats;

        public void Initialize(AequusPlayer aequus)
        {
            _playerStats = new List<PlayerStat>();
            foreach (var stat in _registeredStats)
            {
                var instance = stat.GetNewInstance();
                stat.Initalize(aequus.Player, aequus);
                _playerStats.Add(stat);
            }
        }

        public void ResetEffects(AequusPlayer aequus)
        {
            foreach (var stat in _playerStats)
            {
                stat.ResetEffects(aequus.Player, aequus);
            }
        }

        public T GetStat<T>(AequusPlayer aequus) where T : PlayerStat
        {
            return (T)_playerStats[ModContent.GetInstance<T>().Type];
        }

        public static int RegisterStat(PlayerStat stat)
        {
            if (_registeredStats == null)
            {
                _registeredStats = new List<PlayerStat>();
            }
            _registeredStats.Add(stat);
            return _registeredStats.Count - 1;
        }
    }
}