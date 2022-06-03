using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Aequus.NPCs.Friendly;

namespace Aequus.Items.Misc.Critters
{
    public class Snobster : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(5);
        }

        public override void SetDefaults()
        {
            Item.DefaultToCapturedCritter((short)ModContent.NPCType<SnobsterCritter>());
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 5);
        }
    }
}