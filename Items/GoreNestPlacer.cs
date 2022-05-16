using Aequus.Tiles;
using Aequus.Tiles.Natural;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public class GoreNestPlacer : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Spawns a gore nest biome at your cursor");
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GoreNestStalagmite>());
            Item.consumable = false;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.LightRed;
            Item.useAnimation = Item.useTime;
        }

        public override bool? UseItem(Player player)
        {
            //GoreNestTile.TryGrowGoreNest(Main.MouseWorld.ToTileCoordinates().X, Main.MouseWorld.ToTileCoordinates().Y);
            //GoreNestTile.CleanLava(Main.MouseWorld.ToTileCoordinates().X, Main.MouseWorld.ToTileCoordinates().Y);
            return true;
        }
    }
}