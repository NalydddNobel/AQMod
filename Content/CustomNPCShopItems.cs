using Aequus.Items.Misc.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common
{
    public sealed class CustomNPCShopItems : GlobalNPC
    {
        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (type == NPCID.Clothier)
            {
                if (AequusWorld.HardmodeTier)
                {
                    int slot = -1;
                    for (int i = 0; i < Chest.maxItems - 1; i++)
                    {
                        if (shop.item[i].type == ItemID.FamiliarWig || shop.item[i].type == ItemID.FamiliarShirt || shop.item[i].type == ItemID.FamiliarPants) // at the very end of the paintings, and will intercept the slot for any walls or blank slots
                        {
                            slot = i + 1;
                        }
                    }
                    if (slot != -1 && slot != Chest.maxItems - 1)
                    {
                        shop.Insert(ModContent.ItemType<FamiliarPickaxe>(), slot);
                    }
                    nextSlot++;
                }
            }
        }
    }
}