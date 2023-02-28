using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.SpaceSquidMiniboss.Rewards
{
    public class SpaceSquidTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Trophies>(), Trophies.SpaceSquid);
            Item.maxStack = 9999;
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
        }
    }
}