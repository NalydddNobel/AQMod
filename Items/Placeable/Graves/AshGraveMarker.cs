using Aequus.Tiles.Furniture;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Graves
{
    public class AshGraveMarker : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(2);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AshTombstones>(), AshTombstones.Style_AshGraveMarker);
            Item.width = 12;
            Item.height = 12;
            Item.rare = ItemRarityID.Green;
        }
    }
}