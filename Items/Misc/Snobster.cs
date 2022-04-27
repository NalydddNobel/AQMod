using Aequus.NPCs;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Aequus.Items.Misc
{
    public class Snobster : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToCapturedCritter((short)ModContent.NPCType<SnobsterCritter>());
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 5);
        }
    }
}