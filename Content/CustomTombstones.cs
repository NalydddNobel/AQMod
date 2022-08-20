using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content
{
    public class CustomTombstones : ILoadable
    {
        public static List<int> HellTombstones { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            HellTombstones = new List<int>();
        }

        void ILoadable.Unload()
        {
            HellTombstones?.Clear();
            HellTombstones = null;
        }

        public static List<int> ChooseTombstone(Player player, int coinsOwned, NetworkText deathText, int hitDirection)
        {
            return null;
        }
    }
}