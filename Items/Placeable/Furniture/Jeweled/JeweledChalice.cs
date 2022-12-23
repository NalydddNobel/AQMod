using Aequus.Content.ExporterQuests;
using Aequus.Tiles.Furniture.Jeweled;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.Jeweled
{
    public class JeweledChalice : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            ExporterQuestSystem.QuestItems.Add(Type, new DefaultThieveryItemInfo());
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<JeweledChaliceTile>());
            Item.width = 16;
            Item.height = 24;
            Item.rare = ItemRarityID.Quest;
            Item.questItem = true;
            Item.maxStack = 9999;
        }
    }
}