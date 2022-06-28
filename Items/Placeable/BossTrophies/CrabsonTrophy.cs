using Aequus.Tiles.Furniture;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.BossTrophies
{
    public class CrabsonTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Trophies>(), Trophies.Crabson);
            Item.maxStack = 99;
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
        }
    }
}