using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Vanities.Critters
{
    public class Snobster : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 24;
            item.consumable = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 10;
            item.useAnimation = 15;
            item.maxStack = 999;
            item.makeNPC = (short)ModContent.NPCType<NPCs.Friendly.Snobster>();
        }
    }
}