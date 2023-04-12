using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Vanity
{
    [AutoloadEquip(EquipType.Back, EquipType.Front)]
    public class PumpkingCloak : ModItem
    {
        public static int FrontID { get; private set; }
        public static int BackID { get; private set; }

        public override void SetStaticDefaults()
        {
            FrontID = Item.frontSlot;
            BackID = Item.backSlot;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(20, 20);
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(silver: 20);
            Item.vanity = true;
        }
    }
}
