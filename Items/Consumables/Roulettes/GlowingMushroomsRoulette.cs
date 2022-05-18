using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Consumables.Roulettes
{
    public class GlowingMushroomsRoulette : RouletteBase
    {
        public static List<int> Table { get; private set; }

        protected override List<int> LootTable => Table;

        public override void Load()
        {
            Table = new List<int>()
            {
            };
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Table.AddRange(GoldenRoulette.Table);
        }

        public override void Unload()
        {
            Table?.Clear();
            Table = null;
        }

        public override void RightClick(Player player)
        {
            var source = player.GetSource_OpenItem(Type);
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

            PoolPotions(player, RouletteData.DefaultPotions);
            Split_PoolArmorPolish(player);
        }
    }
}