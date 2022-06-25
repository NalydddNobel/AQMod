using Aequus.Items.Accessories.Summon.Sentry;
using Aequus.Items.Consumables.CursorDyes;
using Aequus.Items.Misc.Pets;
using Terraria;
using Terraria.GameContent.Events;
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
                if (Aequus.HardmodeTier)
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
            else if (type == NPCID.DyeTrader)
            {
                int removerSlot = nextSlot;
                if (Main.LocalPlayer.statLifeMax >= 200)
                {
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<HealthCursorDye>());
                }
                if (Main.LocalPlayer.statManaMax >= 100)
                {
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ManaCursorDye>());
                }
                if (LanternNight.LanternsUp)
                {
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SwordCursorDye>());
                }
                if (AequusWorld.downedEventDemon)
                {
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<DemonicCursorDye>());
                }
                if (nextSlot != removerSlot)
                {
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CursorDyeRemover>());
                }
            }
            else if (type == NPCID.Mechanic)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SantankSentry>());
            }
        }
    }
}