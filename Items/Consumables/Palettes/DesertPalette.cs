using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Consumables.Palettes
{
    public class DesertPalette : PaletteBase
    {
        public static List<int> DesertChestLoot { get; private set; }

        public override void Load()
        {
            DesertChestLoot = new List<int>()
            {
                ItemID.PharaohsMask,
                ItemID.SandstorminaBottle,
                ItemID.FlyingCarpet,
                ItemID.AncientChisel,
                ItemID.SandBoots,
                ItemID.ThunderSpear,
                ItemID.ThunderStaff,
                ItemID.MagicConch,
                ItemID.MysticCoilSnake,
            };
        }

        public override void Unload()
        {
            DesertChestLoot?.Clear();
            DesertChestLoot = null;
        }

        protected override List<int> LootTable => DesertChestLoot;

        public override void RightClick(Player player)
        {
            var source = player.GetSource_OpenItem(Type);
            player.QuickSpawnItem(source, ItemID.SilverCoin, Main.rand.Next(50, 80));
            if (Main.rand.NextBool(8))
            {
                player.QuickSpawnItem(source, ItemID.EncumberingStone);
            }
            if (Main.rand.NextBool(8))
            {
                player.QuickSpawnItem(source, ItemID.DesertMinecart);
            }
            if (Main.rand.NextBool(4))
            {
                player.QuickSpawnItem(source, ItemID.CatBast);
            }
            base.RightClick(player);
            if (GetItem() == ItemID.PharaohsMask)
            {
                player.QuickSpawnItem(source, ItemID.PharaohsRobe);
            }

            PoolPotions(player, PaletteCatalogue.DefaultPotions);
            Split_PoolArmorPolish(player);
        }
    }
}