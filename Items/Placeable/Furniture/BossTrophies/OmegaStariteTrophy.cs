using Aequus.Tiles.Furniture;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.BossTrophies
{
    public class OmegaStariteTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Trophies>(), Trophies.OmegaStarite);
            Item.maxStack = 9999;
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
        }
    }
}