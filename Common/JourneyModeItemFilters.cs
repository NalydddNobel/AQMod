using Aequus.Items.Accessories.Utility;
using Aequus.Items.Tools;
using Aequus.Items.Tools.Misc;
using Aequus.Items.Weapons.Summon.Necro.Candles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Aequus.Common
{
    public class JourneyModeItemFilters : ILoadable
    {
        void ILoadable.Load(Mod mod)
        {
            On.Terraria.GameContent.Creative.ItemFilters.Weapon.FitsFilter += Weapon_FitsFilter;
            On.Terraria.GameContent.Creative.ItemFilters.Tools.FitsFilter += Tools_FitsFilter;
            On.Terraria.GameContent.Creative.ItemFilters.MiscAccessories.FitsFilter += MiscAccessories_FitsFilter;
        }

        private static bool Weapon_FitsFilter(On.Terraria.GameContent.Creative.ItemFilters.Weapon.orig_FitsFilter orig, ItemFilters.Weapon self, Item entry)
        {
            return orig(self, entry) || entry.ModItem is BaseSoulCandle;
        }
        private static bool MiscAccessories_FitsFilter(On.Terraria.GameContent.Creative.ItemFilters.MiscAccessories.orig_FitsFilter orig, ItemFilters.MiscAccessories self, Item entry)
        {
            return orig(self, entry) || entry.ModItem is RichMansMonocle || entry.ModItem is ForgedCard || entry.ModItem is FaultyCoin;
        }
        private static bool Tools_FitsFilter(On.Terraria.GameContent.Creative.ItemFilters.Tools.orig_FitsFilter orig, ItemFilters.Tools self, Item entry)
        {
            return orig(self, entry) || entry.ModItem is PhysicsGun || entry.ModItem is Bellows || entry.ModItem is GhostlyGrave || entry.ModItem is Pumpinator;
        }

        void ILoadable.Unload()
        {
        }
    }
}