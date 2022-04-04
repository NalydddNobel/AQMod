using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Misc
{
    public class SkyPalette : PaletteBase
    {
        public static List<int> SkyChestLoot { get; private set; }

        public override string Texture => "Terraria/Images/Item_" + ItemID.FloatingIslandFishingCrate;

        protected override List<int> LootTable => SkyChestLoot;

        public override void Load()
        {
            SkyChestLoot = new List<int>()
                {
                    ItemID.ShinyRedBalloon,
                    ItemID.CreativeWings,
                    ItemID.Starfury,
                    //ModContent.ItemType<DreamCatcher>(),
                };
        }

        public override void Unload()
        {
            SkyChestLoot?.Clear();
            SkyChestLoot = null;
        }

        public override void RightClick(Player player)
        {
            var source = player.GetItemSource_OpenItem(Type);
            player.QuickSpawnItem(source, ItemID.SilverCoin, Main.rand.Next(50, 80));

            if (Main.rand.NextBool())
            {
                player.QuickSpawnItem(source, ItemID.SkyMill);
            }

            base.RightClick(player);

            //if (Main.rand.NextBool())
            //{
            //    int p = AQItem.PoolPotion(-1);
            //    player.QuickSpawnItem(p, Main.rand.Next(2) + 1);
            //    if (Main.rand.NextBool())
            //    {
            //        player.QuickSpawnItem(AQItem.PoolPotion(p), Main.rand.Next(2) + 1);
            //    }
            //}
        }
    }
}