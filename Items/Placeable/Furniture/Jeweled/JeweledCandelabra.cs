using Aequus.Content;
using Aequus.Tiles.Furniture.Jeweled;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.Jeweled
{
    public class JeweledCandelabra : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            ExporterQuests.QuestItems.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<JeweledCandelabraTile>());
            Item.width = 16;
            Item.height = 24;
            Item.rare = ItemRarityID.Quest;
            Item.questItem = true;
            Item.maxStack = 99;
        }
    }
}