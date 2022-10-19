using Aequus.Common;
using Aequus.Content.Necromancy;
using Aequus.Items;
using System;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal class CalamityModSupport : IAddRecipes, ILoadBefore
    {
        public static Mod CalamityMod { get; private set; }
        public static Mod CatalystMod { get; private set; }

        Type ILoadBefore.LoadBefore => typeof(NecromancyDatabase);

        void ILoadable.Load(Mod mod)
        {
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
            CalamityMod = null;
            CatalystMod = null;
            if (ModLoader.TryGetMod("CalamityMod", out var calamityMod))
            {
                CalamityMod = calamityMod;
            }
            if (ModLoader.TryGetMod("CatalystMod", out var catalystMod))
            {
                CatalystMod = catalystMod;
                if (catalystMod.TryFind<ModRarity>("SuperbossRarity", out var rarity))
                {
                    AequusItem.RarityNames.Add(rarity.Type, "Mods.Aequus.ItemRarity.Catalyst_Crystal");
                }
            }
        }

        void ILoadable.Unload()
        {
            CalamityMod = null;
            CatalystMod = null;
        }
    }
}