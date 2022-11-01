using Aequus.Tiles.Ambience;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Nature.MossMushrooms
{
    public class XenonMushroom : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GlowingMossMushrooms>(), GlowingMossMushrooms.Xenon * 3);
            Item.value = Item.sellPrice(silver: 2, copper: 50);
            Item.maxStack = 9999;
        }
    }
}