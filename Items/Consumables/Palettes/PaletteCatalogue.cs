using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Palettes
{
    public sealed class PaletteCatalogue : ILoadable
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