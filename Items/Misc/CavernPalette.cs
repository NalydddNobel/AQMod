using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Misc
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
            player.QuickSpawnItem(source, ItemID.SilverCoin, Main.rand.Next(50, 80));
            if (Main.rand.NextBool())
            {
                player.QuickSpawnItem(source, ItemID.Extractinator);
            }
            base.RightClick(player);
            if (GetItem() == ItemID.FlareGun)
            {
                player.QuickSpawnItem(source, ItemID.Flare, Main.rand.Next(25) + 25);
            }

            //if (Main.rand.NextBool())
            //{
            //    int p = AQItem.PoolPotion(-1);
            //    player.QuickSpawnItem(p, Main.rand.Next(2) + 1);
            //    if (Main.rand.NextBool())
            //    {
            //        player.QuickSpawnItem(AQItem.PoolPotion(p), Main.rand.Next(2) + 1);
            //    }
            //}

            //if (AQMod.split.IsActive && Main.rand.NextBool())
            //{
            //    player.QuickSpawnItem(AQMod.split.ItemType("ArmorPolishKit"));
            //}
        }
    }
}