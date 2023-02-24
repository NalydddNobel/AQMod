using Aequus.Items.Accessories.Summon.Sentry;
using Aequus.Items.Accessories.Vanity.Cursors;
using Aequus.Items.Pets;
using Aequus.Items.Placeable.Furniture.CraftingStation;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public partial class AequusNPC : GlobalNPC
    {
        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            var inv = Main.LocalPlayer.inventory;
            switch (type)
            {
                case NPCID.Merchant:
                    {
                        for (int i = 0; i < Main.InventoryItemSlotsCount; i++)
                        {
                            if (inv[i].useAmmo == AmmoID.Dart)
                            {
                                AddAvoidDupes(ItemID.Seed, Item.buyPrice(copper: 3), shop, ref nextSlot);
                                break;
                            }
                        }
                    }
                    break;

                case NPCID.Clothier:
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
                    break;

                case NPCID.Mechanic:
                    {
                        shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SantankSentry>());
                    }
                    break;

                case NPCID.DyeTrader:
                    {
                        shop.item[nextSlot++].SetDefaults(ModContent.ItemType<DyableCursor>());
                    }
                    break;
            }
        }
        public static bool AddAvoidDupes(int itemID, int? customPrice, Chest shop, ref int nextSlot)
        {
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (shop.item[i].type == itemID)
                {
                    if (shop.item[i].shopCustomPrice == customPrice)
                        return false;

                    shop.item[i].shopCustomPrice = customPrice;
                    return true;
                }
            }
            shop.item[nextSlot].SetDefaults(itemID);
            shop.item[nextSlot].shopCustomPrice = customPrice;
            nextSlot++;
            return true;
        }

        public void GenericBuffs(ref float damageMult, ref int defense)
        {
            damageMult += 0.1f;
            defense += 3;
        }

        public override void BuffTownNPC(ref float damageMult, ref int defense)
        {
            if (AequusWorld.downedCrabson)
            {
                GenericBuffs(ref damageMult, ref defense);
            }
            if (AequusWorld.downedOmegaStarite)
            {
                GenericBuffs(ref damageMult, ref defense);
            }
            if (AequusWorld.downedDustDevil)
            {
                GenericBuffs(ref damageMult, ref defense);
            }
        }
    }
}