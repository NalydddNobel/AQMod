using Aequus.Items.Misc;
using Aequus.Items.Misc.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common
{
    public sealed class CustomNPCShop : GlobalNPC
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
                        if (shop.item[i].type == ItemID.FamiliarWig || shop.item[i].type == ItemID.FamiliarShirt || shop.item[i].type == ItemID.FamiliarPants)
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
            else if (type == NPCID.Wizard)
            {
                int slot = -1;
                for (int i = 0; i < Chest.maxItems - 1; i++)
                {
                    if (shop.item[i].type == ItemID.SpellTome)
                    {
                        slot = i + 1;
                    }
                }
                if (slot != -1 && slot != Chest.maxItems - 1)
                {
                    shop.Insert(ModContent.ItemType<UnenchantedStaff>(), slot);
                }
                nextSlot++;
            }
        }
    }
}