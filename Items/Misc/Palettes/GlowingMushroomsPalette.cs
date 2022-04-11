using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Misc.Palettes
{
    public class GlowingMushroomsPalette : PaletteBase
    {
        public static List<int> GlowingMushroomsChestLoot { get; private set; }

        public override void Load()
        {
            GlowingMushroomsChestLoot = new List<int>()
            {
            };
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            GlowingMushroomsChestLoot.AddRange(CavernPalette.CavernChestLoot);
        }

        public override void Unload()
        {
            GlowingMushroomsChestLoot?.Clear();
            GlowingMushroomsChestLoot = null;
        }

        protected override List<int> LootTable => GlowingMushroomsChestLoot;

        public override void RightClick(Player player)
        {
            var source = player.GetItemSource_OpenItem(Type);
            player.QuickSpawnItem(source, ItemID.SilverCoin, Main.rand.Next(50, 80));
            if (Main.rand.NextBool(8))
            {
                player.QuickSpawnItem(source, ItemID.MushroomHat);
                player.QuickSpawnItem(source, ItemID.MushroomVest);
                player.QuickSpawnItem(source, ItemID.MushroomPants);
            }
            if (Main.rand.NextBool(8))
            {
                player.QuickSpawnItem(source, ItemID.ShroomMinecart);
            }
            base.RightClick(player);
            if (GetItem() == ItemID.FlareGun)
            {
                player.QuickSpawnItem(source, ItemID.Flare, Main.rand.Next(25) + 25);
            }

            PoolPotions(player, PaletteDataProvider.DefaultPotions);
            Split_PoolArmorPolish(player);
        }
    }
}