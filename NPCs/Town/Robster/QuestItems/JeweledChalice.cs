using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Town.Robster.QuestItems
{
    public class JeweledChalice : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 24;
            item.rare = ItemRarityID.Quest;
            item.questItem = true;
        }
    }
}