using AQMod.NPCs.Friendly;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Misc
{
    public class DwarfStariteItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.Glowmask();
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 24;
            item.consumable = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 10;
            item.useAnimation = 15;
            item.maxStack = 999;
            item.makeNPC = (short)ModContent.NPCType<DwarfStariteCritter>();
            item.value = Item.sellPrice(silver: 2);
        }
    }
}