using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Roulettes
{
    public sealed class RouletteData : ILoadable
    {
        public static List<int> DefaultPotions { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            DefaultPotions = new List<int>()
            {
                ItemID.ShinePotion,
                ItemID.NightOwlPotion,
                ItemID.SwiftnessPotion,
                ItemID.ArcheryPotion,
                ItemID.GillsPotion,
                ItemID.HunterPotion,
                ItemID.MiningPotion,
                ItemID.TrapsightPotion,
                ItemID.RegenerationPotion,
            };
        }

        void ILoadable.Unload()
        {
            DefaultPotions?.Clear();
            DefaultPotions = null;
        }
    }
}