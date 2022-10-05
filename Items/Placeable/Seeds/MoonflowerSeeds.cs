using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Seeds
{
    public class MoonflowerSeeds : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GoreNestTile>());
            Item.value = Item.sellPrice(silver: 2);
            Item.maxStack = 9999;
            Item.createTile = ModContent.TileType<MoonflowerTile>();
            Item.rare = ItemRarityID.Blue;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }
    }
}