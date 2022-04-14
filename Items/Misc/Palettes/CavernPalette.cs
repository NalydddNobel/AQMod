using Aequus.Items.Accessories;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Palettes
{
    public class CavernPalette : PaletteBase
    {
        public static List<int> CavernChestLoot { get; private set; }

        public override void Load()
        {
            CavernChestLoot = new List<int>()
                {
                    ItemID.BandofRegeneration,
                    ItemID.MagicMirror,
                    ItemID.CloudinaBottle,
                    ItemID.HermesBoots,
                    ItemID.LuckyHorseshoe,
                    ItemID.ShoeSpikes,
                    ItemID.FlareGun,
                    ItemID.Mace,
                };
        }

        public override void Unload()
        {
            CavernChestLoot?.Clear();
            CavernChestLoot = null;
        }

        protected override List<int> LootTable => CavernChestLoot;

        public override void RightClick(Player player)
        {
            var source = player.GetItemSource_OpenItem(Type);
            base.RightClick(player);
            if (GetItem() == ItemID.FlareGun)
            {
                player.QuickSpawnItem(source, ItemID.Flare, Main.rand.Next(25) + 25);
            }

            player.QuickSpawnItem(source, ItemID.SilverCoin, Main.rand.Next(50, 80));
            if (Main.rand.NextBool())
            {
                player.QuickSpawnItem(source, ItemID.Extractinator);
            }
            if (Main.rand.NextBool())
            {
                player.QuickSpawnItem(source, ItemID.Torch, Main.rand.Next(15, 50));
                if (Main.rand.NextBool(3))
                {
                    player.QuickSpawnItem(source, ModContent.ItemType<GlowCore>());
                }
            }

            PoolPotions(player, PaletteDataProvider.DefaultPotions);
            Split_PoolArmorPolish(player);
        }
    }
}