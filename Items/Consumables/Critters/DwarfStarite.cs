using Aequus.NPCs.Friendly;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Critters
{
    public class DwarfStarite : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToCapturedCritter((short)ModContent.NPCType<DwarfStariteCritter>());
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 10);
        }
    }
}