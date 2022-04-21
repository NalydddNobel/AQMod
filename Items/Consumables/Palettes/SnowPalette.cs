using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Consumables.Palettes
{
    public class SnowPalette : PaletteBase
    {
        public static List<int> FrozenChestLoot { get; private set; }

        public override void Load()
        {
            FrozenChestLoot = new List<int>()
            {
                ItemID.IceBoomerang,
                ItemID.IceBlade,
                ItemID.IceSkates,
                ItemID.SnowballCannon,
                ItemID.BlizzardinaBottle,
                ItemID.FlurryBoots,
                ItemID.Fish,
            };
        }

        public override void Unload()
        {
            FrozenChestLoot?.Clear();
            FrozenChestLoot = null;
        }

        protected override List<int> LootTable => FrozenChestLoot;

        public override void RightClick(Player player)
        {
            var source = player.GetSource_OpenItem(Type);
            player.QuickSpawnItem(source, ItemID.SilverCoin, Main.rand.Next(50, 80));
            if (Main.rand.NextBool(4))
            {
                player.QuickSpawnItem(source, ItemID.IceMirror);
            }
            if (Main.rand.NextBool())
            {
                player.QuickSpawnItem(source, ItemID.IceMachine);
            }
            if (Main.rand.NextBool(8))
            {
                player.QuickSpawnItem(source, ItemID.Extractinator);
            }
            base.RightClick(player);

            PoolPotions(player, PaletteCatalogue.DefaultPotions);
            Split_PoolArmorPolish(player);
        }
    }
}