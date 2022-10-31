using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Aequus.Items.Accessories.Utility
{
    public class HyperJet : ModItem, ItemHooks.IUpdateVoidBag
    {
        public bool active;

        public HyperJet()
        {
            active = false;
        }

        public override void Load()
        {
            On.Terraria.UI.ItemSlot.RightClick_ItemArray_int_int += ItemSlot_RightClick_ItemArray_int_int;
        }
        private static void ItemSlot_RightClick_ItemArray_int_int(On.Terraria.UI.ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            int oldStack = inv[slot].stack;
            orig(inv, context, slot);
            if (Main.stackSplit <= 2 && AequusHelpers.iterations == 0)
            {
                if (Main.stackDelay > 2)
                    Main.stackDelay = Math.Max(Main.stackDelay / 2, 2);
                if (!inv[0].IsAir && oldStack != inv[0].stack)
                {
                    if (Main.LocalPlayer.Aequus().accStackSplit && Main.stackDelay < 7)
                    {
                        for (int i = 0; i < 7 - Main.stackDelay; i++)
                        {
                            AequusHelpers.iterations = i + 1;
                            Main.stackSplit = 1;
                            ItemSlot.RightClick(inv, context, slot);
                        }
                        AequusHelpers.iterations = 0;
                    }
                }
                return;
            }
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.LifeformAnalyzer);
        }

        public override void UpdateInventory(Player player)
        {
            if (active)
                player.Aequus().accStackSplit = true;
        }

        void ItemHooks.IUpdateVoidBag.UpdateBank(Player player, AequusPlayer aequus, int slot, int bank)
        {
            if (active)
                player.Aequus().accStackSplit = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (active)
                player.Aequus().accStackSplit = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (var t in tooltips)
            {
                if (t.Name == "ItemName")
                {
                    t.Text += $" ({AequusText.GetText(active ? "Active" : "Inactive")})";
                }
            }
        }

        public override void RightClick(Player player)
        {
            active = !active;
        }

        public override bool ConsumeItem(Player player)
        {
            return false;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["active"] = active;
        }

        public override void LoadData(TagCompound tag)
        {
            active = tag.Get<bool>("active");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(active);
        }

        public override void NetReceive(BinaryReader reader)
        {
            active = reader.ReadBoolean();
        }

        public override bool CanRightClick()
        {
            return true;
        }
    }
}