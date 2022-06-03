using Aequus.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Graves
{
    public class AshObelisk : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(2);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tombstones>(), Tombstones.AshObeliskStyle);
            Item.width = 12;
            Item.height = 12;
            Item.rare = ItemRarityID.Green;
        }
    }
}