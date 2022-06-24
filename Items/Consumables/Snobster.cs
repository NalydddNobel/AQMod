using Aequus.NPCs.Friendly;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables
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