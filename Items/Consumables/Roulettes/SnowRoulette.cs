using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Consumables.Roulettes
{
    public class SnowRoulette : RouletteBase
    {
        public static List<int> Table { get; private set; }

        protected override List<int> LootTable => Table;

        public override void Load()
        {
            Table = new List<int>()
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
            Table?.Clear();
            Table = null;
        }

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

            PoolPotions(player, RouletteData.DefaultPotions);
            Split_PoolArmorPolish(player);
        }
    }
}