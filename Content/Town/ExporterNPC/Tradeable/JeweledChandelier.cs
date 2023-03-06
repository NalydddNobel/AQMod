using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Town.ExporterNPC.Tradeable
{
    public class JeweledChandelier : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileID.Chandeliers/*ModContent.TileType<JeweledChandelierTile>()*/);
            Item.width = 16;
            Item.height = 24;
            Item.rare = ItemRarityID.Quest;
            Item.questItem = true;
            Item.maxStack = 9999;
        }
    }
}